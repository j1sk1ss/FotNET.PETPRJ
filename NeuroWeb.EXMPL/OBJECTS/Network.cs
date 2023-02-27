using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;

using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Configuration configuration) {
            try {
                Configuration = configuration;                
                CNNInitialization();
                FNNInitialization();
            }
            catch (OverflowException e) {
                MessageBox.Show($"{e}","Неккоректная конфигурация!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой инициализации сети!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        private void CNNInitialization() {
            ConvolutionLayers = new ConvolutionLayer[Configuration.ConvolutionLayouts];
            for (var i = 0; i < ConvolutionLayers.Length; i++) {
                ConvolutionLayers[i] = new ConvolutionLayer(Configuration, Configuration.ConvolutionConfigurations[i]);
            }   
        }
        
        private void FNNInitialization() {
            ForwardLayouts = Configuration.ForwardLayout;
            ForwardNeurons = new int[ForwardLayouts];
            for (var i = 0; i < ForwardLayouts; i++) ForwardNeurons[i] = Configuration.NeuronsLayer[i];
                
            ForwardWeights = new Matrix[ForwardLayouts - 1];
            ForwardBias    = new double[ForwardLayouts - 1][];

            for (var i = 0; i < ForwardLayouts - 1; i++) {
                ForwardBias[i]    = new double[ForwardNeurons[i + 1]];
                ForwardWeights[i] = new Matrix(ForwardNeurons[i + 1], ForwardNeurons[i]);

                ForwardWeights[i].FillRandom();

                for (var j = 0; j < ForwardNeurons[i + 1]; j++)
                    ForwardBias[i][j] = new Random().Next() % 50 * .06 / (ForwardNeurons[i] + 15);
            }

            ForwardNeuronsValue = new double[ForwardLayouts][];
            ForwardNeuronsError = new double[ForwardLayouts][];

            for (var i = 0; i < ForwardLayouts; i++) {
                ForwardNeuronsValue[i] = new double[ForwardNeurons[i]];
                ForwardNeuronsError[i] = new double[ForwardNeurons[i]];
            }

            ForwardNeuronsBios = new double[ForwardLayouts - 1];
            for (var i = 0; i < ForwardNeuronsBios.Length; i++) ForwardNeuronsBios[i] = 1;
        }
        
        public Tensor DataTensor { get; set; }
        public ConvolutionLayer[] ConvolutionLayers { get; set; }
        private int ConvolutionLayouts { get; }
       


        public Configuration Configuration { get; }
        private int ForwardLayouts { get; set; }
        private int[] ForwardNeurons { get; set; }
        private Matrix[] ForwardWeights { get; set; }
        private double[][] ForwardBias { get; set; }
        public double[][] ForwardNeuronsValue { get; set; }
        private double[][] ForwardNeuronsError { get; set; }
        private double[] ForwardNeuronsBios { get; set; }

        public void InsertInformation(Number number) {
            DataTensor = new Tensor(number.GetAsMatrix());
        }
        
        private int GetMaxIndex(IReadOnlyList<double> values) {
            try {
                var max = values[0];
                var prediction = 0;

                for (var j = 1; j < ForwardNeurons[ForwardLayouts - 1]; j++) {
                    var temp = values[j];
                    if (!(temp > max)) continue;
                
                    prediction = j;
                    max = temp;
                }

                return prediction;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой получения максимального индекса!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public double ConvolutionFeed() {
            try {
                for (var i = 0; i < ConvolutionLayouts - 1; i++) {
                    DataTensor = ConvolutionLayers[i].GetNextLayer(DataTensor);
                }
                
                ForwardNeuronsValue[0] = new double[Tensors[^1].Body[0].GetAsList().Count];
                for (var i = 0; i < Tensors[^1].GetValues().Count; i++) ForwardNeuronsValue[0][i] = Tensors[^1].GetValues()[i];
                return ForwardFeed();
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой активации нейронов в слоях свёртки!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
        
        public double ForwardFeed() {
            try {
                for (var k = 1; k < ForwardLayouts; ++k) {
                    ForwardNeuronsValue[k] = new Vector(ForwardWeights[k - 1] * ForwardNeuronsValue[k - 1]) + new Vector(ForwardBias[k - 1]);
                    ForwardNeuronsValue[k] = NeuronActivate.Activation(ForwardNeuronsValue[k]);
                }

                return GetMaxIndex(ForwardNeuronsValue[ForwardLayouts - 1]);
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой активации нейронов в прямо-связанных слоях!", MessageBoxButton.OK,
                     MessageBoxImage.Error);
                throw;
            }
        }

        public void ForwardBackPropagation(double expectedAnswer) {
            try {
                for (var i = 0; i < ForwardNeurons[ForwardLayouts - 1]; i++) 
                    if (i != (int)expectedAnswer) 
                        ForwardNeuronsError[ForwardLayouts - 1][i] = -ForwardNeuronsValue[ForwardLayouts - 1][i] * 
                                                       NeuronActivate.GetDerivative(ForwardNeuronsValue[ForwardLayouts - 1][i]);
                    else ForwardNeuronsError[ForwardLayouts - 1][i] = (1.0 - ForwardNeuronsValue[ForwardLayouts - 1][i]) * 
                                                        NeuronActivate.GetDerivative(ForwardNeuronsValue[ForwardLayouts - 1][i]);
                
                for (var i = ForwardLayouts - 2; i > 0; i--) {
                    ForwardNeuronsError[i] = ForwardWeights[i].GetTranspose() * ForwardNeuronsError[i + 1];
                    for (var j = 0; j < ForwardNeurons[i]; j++)
                        ForwardNeuronsError[i][j] *= NeuronActivate.GetDerivative(ForwardNeuronsValue[i][j]);
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой обратного обучения!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public void SetForwardWeights(double learningRange) {
            for (var i = 0; i < ForwardLayouts - 1; ++i)
                for (var j = 0; j < ForwardNeurons[i + 1]; ++j)
                    for (var k = 0; k < ForwardNeurons[i]; ++k)
                        ForwardWeights[i].Body[j, k] += ForwardNeuronsValue[i][k] * ForwardNeuronsError[i + 1][j] * learningRange;

            for (var i = 0; i < ForwardLayouts - 1; i++)
                for (var j = 0; j < ForwardNeurons[i + 1]; j++)
                    ForwardBias[i][j] += ForwardNeuronsError[i + 1][j] * learningRange;
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

    public struct ConvolutionConfiguration
    {
        public int FilterColumn;
        public int FilterRow;

        public int FilterCount;
        public int PoolSize;
        public int Stride;
    }
}