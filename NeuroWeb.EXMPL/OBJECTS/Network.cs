using System;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;
using NeuroWeb.EXMPL.OBJECTS.FORWARD;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Configuration configuration) {
            Configuration = configuration;                
            CNNInitialization();
            FNNInitialization();
        }

        private void CNNInitialization() {
            ConvolutionLayers = new ConvolutionLayer[Configuration.ConvolutionLayouts];
            for (var i = 0; i < ConvolutionLayers.Length; i++) {
                ConvolutionLayers[i] = 
                    new ConvolutionLayer(4, 4 * i + 1, Configuration.ConvolutionConfigurations[i]);
            }   
        }
        
        private void FNNInitialization() {
            PerceptronLayers = new PerceptronLayer[Configuration.ForwardLayout];
            for (var i = 0; i < Configuration.ForwardLayout - 1; i++) {
                PerceptronLayers[i] =
                    new PerceptronLayer(Configuration.NeuronsLayer[i], Configuration.NeuronsLayer[i + 1]);
            }
        }

        private Tensor DataTensor { get; set; }
        private ConvolutionLayer[] ConvolutionLayers { get; set; }
        private int ConvolutionLayouts { get; }
        public Configuration Configuration { get; }
        public PerceptronLayer[] PerceptronLayers { get; private set; }


        public void InsertInformation(Number number) {
            DataTensor = new Tensor(number.GetAsMatrix());
        }
        
        private int GetMaxIndex(IReadOnlyList<double> values) {
            var max = values[0];
            var prediction = 0;

            for (var j = 1; j < values.Count; j++) {
                var temp = values[j];
                if (!(temp > max)) continue;
            
                prediction = j;
                max = temp;
            }

            return prediction;
        }

        public double ConvolutionFeed() {
            for (var i = 0; i < ConvolutionLayouts - 1; i++) {
                DataTensor = ConvolutionLayers[i].GetNextLayer(DataTensor);
            }

            PerceptronLayers[0].Neurons = DataTensor.GetValues().ToArray();
            return ForwardFeed();
        }
        
        public double ForwardFeed() {
            for (var k = 1; k < PerceptronLayers.Length; ++k) {
                PerceptronLayers[k].Neurons = PerceptronLayers[k - 1].GetNextLayer();
            }

            return GetMaxIndex(PerceptronLayers[^1].Neurons);
        }

        public void ForwardBackPropagation(double expectedAnswer) {
            try {
                for (var i = 0; i < PerceptronLayers.Length - 1; i++) 
                    if (i != (int)expectedAnswer) 
                        PerceptronLayers[^1].NeuronsError[i] = -PerceptronLayers[^1].NeuronsError[i] * 
                                                                  NeuronActivate.GetDerivative(PerceptronLayers[^1].NeuronsError[i]);
                    else PerceptronLayers[^1].NeuronsError[i] = (1.0 - PerceptronLayers[^1].NeuronsError[i]) * 
                                                                NeuronActivate.GetDerivative(PerceptronLayers[^1].NeuronsError[i]);
                
                for (var i = PerceptronLayers.Length - 2; i >= 0; i--) {
                    PerceptronLayers[i].NeuronsError = PerceptronLayers[i].Weights.GetTranspose() * PerceptronLayers[i + 1].NeuronsError;
                    for (var j = 0; j < PerceptronLayers[i].Neurons.Length; j++)
                        PerceptronLayers[i].NeuronsError[j] *= NeuronActivate.GetDerivative(PerceptronLayers[i].NeuronsError[j]);
                }
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой обратного обучения!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public void SetForwardWeights(double learningRange) { 
            for (var i = 0; i < PerceptronLayers.Length - 1; ++i)
                PerceptronLayers[i].SetWeights(learningRange);
        }

        private static string _weights;
        private static string GetForwardWeights() {
            var defaultWeights = Properties.Resources.defaultWeights;

            var file = new OpenFileDialog {
                Filter = "TXT files | *.txt"
            };
            var message = MessageBox.Show("Использовать стандартные веса вместо " +
                                          "других", "Укажите файл весов!", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes) return defaultWeights;
            _weights = file.FileName;
            return file.ShowDialog() != true ? "" : File.ReadAllText(file.FileName);
        }

        public void SaveForwardWeights() {
            try {
                MessageBox.Show("Начата запись весов!");
                var temp = ForwardWeights.Aggregate("", (current, weight) => current + weight.GetValues());

                for (var i = 0; i < ForwardLayouts - 1; i++)
                    for (var j = 0; j < ForwardNeurons[i + 1]; ++j)
                            temp += ForwardBias[i][j] + " ";
                
                if (File.Exists(_weights)) File.WriteAllText(_weights, temp);
                else {
                    var file = new SaveFileDialog {
                        Filter = "TXT files | *.txt"
                    };
                    MessageBox.Show("Укажите место для сохранения весов!");
                    if (file.ShowDialog() == true) File.WriteAllText(file.FileName, temp);
                    return;
                }
                MessageBox.Show("Веса обновлены!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при записи весов!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }

        public void ReadForwardWeights() {
            try {
                var tempValues = GetForwardWeights().Split(" ", 
                    StringSplitOptions.RemoveEmptyEntries);
                
                if (tempValues.Length < 10) {
                    MessageBox.Show("Веса не загружены!", "Внимание!", MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return;
                }
                
                var position = 0;
                for (var l = 0; l < ForwardLayouts - 1; l++) 
                    for (var i = 0; i < ForwardWeights[l].Body.GetLength(0); i++) 
                        for (var j = 0; j < ForwardWeights[l].Body.GetLength(1); j++) 
                            ForwardWeights[l].SetValues(tempValues[position++], i, j);

                for (var l = 0; l < ForwardLayouts - 1; l++)
                    for (var i = 0; i < ForwardNeurons[l + 1]; i++)
                        ForwardBias[l][i] = double.Parse(tempValues[position++], CultureInfo.InvariantCulture);

                if (position < tempValues.Length) MessageBox.Show("Веса считанны некорректно или не считанны",
                    "Предупреждение!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при чтении весов!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
    
    public struct Configuration {
        public int ConvolutionLayouts;
        public int ForwardLayout;

        public ConvolutionConfiguration[] ConvolutionConfigurations;

        public int[] NeuronsLayer;
    }

    public struct ConvolutionConfiguration {
        public int FilterColumn;
        public int FilterRow;

        public int[] FilterCount;
        public int PoolSize;
        public int Stride;
    }
}