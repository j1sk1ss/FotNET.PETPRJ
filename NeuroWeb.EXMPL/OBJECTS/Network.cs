using System;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;
using NeuroWeb.EXMPL.OBJECTS.FORWARD;
using NeuroWeb.EXMPL.SCRIPTS;
using NeuroWeb.EXMPL.SCRIPTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Configuration configuration) {
            Configuration = configuration;                
            CNNInitialization();
            FNNInitialization();
        }

        private void CNNInitialization() {
            ConvolutionLayers = new ConvolutionLayer[Configuration.ConvolutionLayouts];
            for (var i = 0; i < ConvolutionLayers.Length; i++) 
                ConvolutionLayers[i] = 
                    new ConvolutionLayer(Configuration.ConvolutionConfigurations[i]);
        }
        
        private void FNNInitialization() {
            PerceptronLayers = new PerceptronLayer[Configuration.ForwardLayout];
            for (var i = 0; i < Configuration.ForwardLayout - 1; i++) 
                PerceptronLayers[i] =
                    new PerceptronLayer(Configuration.NeuronsLayer[i], Configuration.NeuronsLayer[i + 1]);
            PerceptronLayers[^1] = new PerceptronLayer(Configuration.NeuronsLayer[^1]);
        }

        public Configuration Configuration { get; }        
        private Tensor DataTensor { get; set; }
        private ConvolutionLayer[] ConvolutionLayers { get; set; }
        public PerceptronLayer[] PerceptronLayers { get; private set; }        
        
        public void InsertInformation(Number number) {
            DataTensor = new Tensor(number.GetAsMatrix());
        }
        
        public void InsertInformation(Tensor tensor) {
            DataTensor = new Tensor(tensor.Channels);
        }

        public void ForwardFeed() {
            try {
                for (var i = 0; i < ConvolutionLayers.Length; i++) {
                    DataTensor = ConvolutionLayers[i].GetNextLayer(DataTensor);
                }

                var perceptronInput = DataTensor.GetValues().ToArray();
                for (var i = 0; i < PerceptronLayers.Length - 1; i++) {
                    PerceptronLayers[i].Neurons = perceptronInput;
                    perceptronInput = PerceptronLayers[i].GetNextLayer();
                }
                
                PerceptronLayers[^1].Neurons = perceptronInput;
                MessageBox.Show(perceptronInput.Length + " Выведено нейронов");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        public void BackPropagation(double expectedAnswer, double learningRange) {
            try {
                MessageBox.Show("Start back propagation");
                for (var i = 0; i < PerceptronLayers[^1].Neurons.Length - 1; i++) 
                    if (i != (int)expectedAnswer) 
                        PerceptronLayers[^1].NeuronsError[i] = -PerceptronLayers[^1].Neurons[i] * 
                                                                  NeuronActivate.GetDerivative(PerceptronLayers[^1].Neurons[i]);
                    else PerceptronLayers[^1].NeuronsError[i] = (1.0 - PerceptronLayers[^1].Neurons[i]) * 
                                                                NeuronActivate.GetDerivative(PerceptronLayers[^1].Neurons[i]);

                for (var i = PerceptronLayers.Length - 2; i >= 0; i--) {
                    PerceptronLayers[i].NeuronsError = PerceptronLayers[i].Weights.GetTranspose() * PerceptronLayers[i + 1].NeuronsError;
                    for (var j = 0; j < PerceptronLayers[i].Neurons.Length; j++)
                        PerceptronLayers[i].NeuronsError[j] *= NeuronActivate.GetDerivative(PerceptronLayers[i].NeuronsError[j]);
                }
                
                for (var i = 0; i < PerceptronLayers.Length - 1; ++i)
                    PerceptronLayers[i].SetWeights(learningRange);

                MessageBox.Show("Perceptron end back propagation");
                MessageBox.Show(new Vector(PerceptronLayers[0].NeuronsError).Print() + " Нейроны ошибки в первом слое персептрона");

                var inputTensor = ConvolutionLayers[^1].Output;
                var errorTensor = new Tensor(new Vector(PerceptronLayers[0].NeuronsError)
                    .AsMatrix(inputTensor.Channels[0].Body.GetLength(0), inputTensor.Channels[0].Body.GetLength(1)));

                MessageBox.Show(ConvolutionLayers[0].Filters[0].Channels[0].Print() + " Фильтр первого слоя развертки до обр. распр.");
                MessageBox.Show(ConvolutionLayers[0].Filters[0].Bias + " Смешение");

                for (var i = ConvolutionLayers.Length - 1; i >= 0; i--) {
                    var prevErrorTensor      = Convolution.GetConvolution(errorTensor, new[] { ConvolutionLayers[i].Filters[0].GetFlipped() }, 1); // После получения Тензора ошибок, его глубина равна единице. Фильтр последнео свёрточного слоя имеет глубину 8
                    var filterGradientTensor = Convolution.GetConvolution(new Tensor(inputTensor.Channels[0]), new[] { prevErrorTensor.GetFlipped() }, 1); // Разные глубина фильтра и глубина отданного персептрона

                    for (var f = 0; f < ConvolutionLayers[i].Filters.Length; f++) {
                        ConvolutionLayers[i].Filters[f]      = (Filter)(ConvolutionLayers[i].Filters[f] - filterGradientTensor * learningRange); // Необходимо решить проблему с разной глубиной фильтров и тензоров
                        ConvolutionLayers[i].Filters[f].Bias = ConvolutionLayers[i].Filters[f].Bias - errorTensor.TensorSum() * learningRange;
                    }
                   
                    errorTensor = prevErrorTensor;
                }

                MessageBox.Show(ConvolutionLayers[0].Filters[0].Channels[0].Print() + " Фильтр первого слоя развертки после обр. распр.");
                MessageBox.Show(ConvolutionLayers[0].Filters[0].Bias + " Смешение");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
            
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
        
        /*
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
        */
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
        public int FilterDepth;

        public int FilterCount;
        public int PoolSize;
        public int Stride;
    }
}