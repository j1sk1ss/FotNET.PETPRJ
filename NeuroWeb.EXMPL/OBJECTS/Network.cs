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
                ForwardLayouts = configuration.Layout;
                Neurons = new int[ForwardLayouts];
                for (var i = 0; i < ForwardLayouts; i++) Neurons[i] = configuration.NeuronsLayer[i];                
                
                Filters = new Tensor(new List<Matrix>(5));
                Tensors = new List<Tensor>();
                for (var i = 0; i < ConvolutionLayouts; i++) Tensors.Add(new Tensor(new Matrix(null)));
                
                Configuration = configuration;

                Weights = new Matrix[ForwardLayouts - 1];
                Bias = new double[ForwardLayouts - 1][];

                for (var i = 0; i < ForwardLayouts - 1; i++)
                {
                    Bias[i] = new double[Neurons[i + 1]];
                    Weights[i] = new Matrix(Neurons[i + 1], Neurons[i]);

                    Weights[i].FillRandom();

                    for (var j = 0; j < Neurons[i + 1]; j++)
                        Bias[i][j] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
                }

                NeuronsValue = new double[ForwardLayouts][];
                NeuronsError = new double[ForwardLayouts][];

                for (var i = 0; i < ForwardLayouts; i++)
                {
                    NeuronsValue[i] = new double[Neurons[i]];
                    NeuronsError[i] = new double[Neurons[i]];
                }

                NeuronsBios = new double[ForwardLayouts - 1];
                for (var i = 0; i < NeuronsBios.Length; i++) NeuronsBios[i] = 1;
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
        
        public Configuration Configuration { get; }
        private int ForwardLayouts { get; }
        private int ConvolutionLayouts { get; }
        private int[] Neurons { get; }
        private Matrix[] Weights { get; }
        private double[][] Bias { get; }
        public double[][] NeuronsValue { get; }
        private double[][] NeuronsError { get; }
        private Tensor Filters { get; }
        private List<Tensor> Tensors { get; }
        private double[] NeuronsBios { get; }

        public void InsertInformation(Number number) {
            Tensors[0] = new Tensor(number.GetAsMatrix());            
        }
        
        private int GetMaxIndex(IReadOnlyList<double> values) {
            try {
                var max = values[0];
                var prediction = 0;

                for (var j = 1; j < Neurons[ForwardLayouts - 1]; j++) {
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
        
        public double ForwardFeed() {
            try {
                for (var i = 0; i < ConvolutionLayouts - 1; i++) {
                    Tensors[i + 1] = Convolution.GetConvolution(Tensors[i], Filters);
                    Tensors[i + 1] = Pooling.MaxPool(Tensors[i + 1]);
                }
                
                for (var i = 0; i < Tensors[^1].GetValues().Count; i++) NeuronsValue[0][i] = Tensors[^1].GetValues()[i];
                
                for (var k = 1; k < ForwardLayouts; ++k) {
                    NeuronsValue[k] = new Vector(Weights[k - 1] * NeuronsValue[k - 1]) + new Vector(Bias[k - 1]);
                    NeuronsValue[k] = NeuronActivate.Activation(NeuronsValue[k]);
                }

                return GetMaxIndex(NeuronsValue[ForwardLayouts - 1]);
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой активации нейронов!", MessageBoxButton.OK,
                     MessageBoxImage.Error);
                throw;
            }
        }

        public void BackPropagation(double expectedAnswer) {
            try {
                for (var i = 0; i < Neurons[ForwardLayouts - 1]; i++) 
                    if (i != (int)expectedAnswer) 
                        NeuronsError[ForwardLayouts - 1][i] = -NeuronsValue[ForwardLayouts - 1][i] * 
                                                       NeuronActivate.GetDerivative(NeuronsValue[ForwardLayouts - 1][i]);
                    else NeuronsError[ForwardLayouts - 1][i] = (1.0 - NeuronsValue[ForwardLayouts - 1][i]) * 
                                                        NeuronActivate.GetDerivative(NeuronsValue[ForwardLayouts - 1][i]);
                
                for (var i = ForwardLayouts - 2; i > 0; i--) {
                    NeuronsError[i] = Weights[i].GetTranspose() * NeuronsError[i + 1];
                    for (var j = 0; j < Neurons[i]; j++)
                        NeuronsError[i][j] *= NeuronActivate.GetDerivative(NeuronsValue[i][j]);
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой обратного обучения!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public void SetWeights(double learningRange) {
            for (var i = 0; i < ForwardLayouts - 1; ++i)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                    for (var k = 0; k < Neurons[i]; ++k)
                        Weights[i].Body[j, k] += NeuronsValue[i][k] * NeuronsError[i + 1][j] * learningRange;

            for (var i = 0; i < ForwardLayouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; j++)
                    Bias[i][j] += NeuronsError[i + 1][j] * learningRange;
        }

        private static string _weights;
        private static string GetWeights() {
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

        public void SaveWeights() {
            try {
                MessageBox.Show("Начата запись весов!");
                var temp = Weights.Aggregate("", (current, weight) => current + weight.GetValues());

                for (var i = 0; i < ForwardLayouts - 1; i++)
                    for (var j = 0; j < Neurons[i + 1]; ++j)
                            temp += Bias[i][j] + " ";
                
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

        public void ReadWeights() {
            try {
                var tempValues = GetWeights().Split(" ", 
                    StringSplitOptions.RemoveEmptyEntries);
                
                if (tempValues.Length < 10) {
                    MessageBox.Show("Веса не загружены!", "Внимание!", MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return;
                }
                
                var position = 0;
                for (var l = 0; l < ForwardLayouts - 1; l++) 
                    for (var i = 0; i < Weights[l].Body.GetLength(0); i++) 
                        for (var j = 0; j < Weights[l].Body.GetLength(1); j++) 
                            Weights[l].SetValues(tempValues[position++], i, j);

                for (var l = 0; l < ForwardLayouts - 1; l++)
                    for (var i = 0; i < Neurons[l + 1]; i++)
                        Bias[l][i] = double.Parse(tempValues[position++], CultureInfo.InvariantCulture);

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
        public int Layout;
        public int[] NeuronsLayer;
    }
}