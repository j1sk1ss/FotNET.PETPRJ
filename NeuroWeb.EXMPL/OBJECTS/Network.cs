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
            Filters = new Filter[Configuration.FilterCount.Length];
            
            for (var i = 0; i < Configuration.FilterCount.Length; i++) {
                for (var j = 0; j < Configuration.FilterCount[i]; j++) {
                     Filters[i].Body.Add(new Matrix(new double[Configuration.FilterRow, Configuration.FilterColumn]));
                     Filters[i].Body[^1].FillRandom();                   
                }
            }
            
            Tensors = new List<Tensor>();
            for (var i = 0; i < ConvolutionLayouts; i++) Tensors.Add(new Tensor(new Matrix(null)));    
        }
        
        private void FNNInitialization() {
            ForwardLayouts = Configuration.ForwardLayout;
            Neurons = new int[ForwardLayouts];
            for (var i = 0; i < ForwardLayouts; i++) Neurons[i] = Configuration.NeuronsLayer[i];
                
            ForwardWeights = new Matrix[ForwardLayouts - 1];
            ForwardBias    = new double[ForwardLayouts - 1][];

            for (var i = 0; i < ForwardLayouts - 1; i++) {
                ForwardBias[i]    = new double[Neurons[i + 1]];
                ForwardWeights[i] = new Matrix(Neurons[i + 1], Neurons[i]);

                ForwardWeights[i].FillRandom();

                for (var j = 0; j < Neurons[i + 1]; j++)
                    ForwardBias[i][j] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
            }

            NeuronsValue = new double[ForwardLayouts][];
            NeuronsError = new double[ForwardLayouts][];

            for (var i = 0; i < ForwardLayouts; i++) {
                NeuronsValue[i] = new double[Neurons[i]];
                NeuronsError[i] = new double[Neurons[i]];
            }

            NeuronsBios = new double[ForwardLayouts - 1];
            for (var i = 0; i < NeuronsBios.Length; i++) NeuronsBios[i] = 1;
        }
        
        public Configuration Configuration { get; }
        private int ForwardLayouts { get; set; }
        private int ConvolutionLayouts { get; }
        private int[] Neurons { get; set; }
        private Matrix[] ForwardWeights { get; set; }
        private double[][] ForwardBias { get; set; }
        public double[][] NeuronsValue { get; set; }
        private double[][] NeuronsError { get; set; }
        private Filter[] Filters { get; set; }
        private List<Tensor> Tensors { get; set; }
        private double[] NeuronsBios { get; set; }

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

        public double ConvolutionFeed() {
            try {
                for (var i = 0; i < ConvolutionLayouts - 1; i++) {
                    Tensors[i + 1] = Convolution.GetConvolution(Tensors[i], Filters[i], Configuration.Stride);
                    Tensors[i + 1] = NeuronActivate.Activation(Tensors[i + 1]);
                    Tensors[i + 1] = Pooling.MaxPool(Tensors[i + 1], Configuration.PoolSize);
                }
                
                NeuronsValue[0] = new double[Tensors[^1].Body[0].GetAsList().Count];
                for (var i = 0; i < Tensors[^1].GetValues().Count; i++) NeuronsValue[0][i] = Tensors[^1].GetValues()[i];
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
                    NeuronsValue[k] = new Vector(ForwardWeights[k - 1] * NeuronsValue[k - 1]) + new Vector(ForwardBias[k - 1]);
                    NeuronsValue[k] = NeuronActivate.Activation(NeuronsValue[k]);
                }

                return GetMaxIndex(NeuronsValue[ForwardLayouts - 1]);
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой активации нейронов в прямо-связанных слоях!", MessageBoxButton.OK,
                     MessageBoxImage.Error);
                throw;
            }
        }

        public void ForwardBackPropagation(double expectedAnswer) {
            try {
                for (var i = 0; i < Neurons[ForwardLayouts - 1]; i++) 
                    if (i != (int)expectedAnswer) 
                        NeuronsError[ForwardLayouts - 1][i] = -NeuronsValue[ForwardLayouts - 1][i] * 
                                                       NeuronActivate.GetDerivative(NeuronsValue[ForwardLayouts - 1][i]);
                    else NeuronsError[ForwardLayouts - 1][i] = (1.0 - NeuronsValue[ForwardLayouts - 1][i]) * 
                                                        NeuronActivate.GetDerivative(NeuronsValue[ForwardLayouts - 1][i]);
                
                for (var i = ForwardLayouts - 2; i > 0; i--) {
                    NeuronsError[i] = ForwardWeights[i].GetTranspose() * NeuronsError[i + 1];
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

        public void SetForwardWeights(double learningRange) {
            for (var i = 0; i < ForwardLayouts - 1; ++i)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                    for (var k = 0; k < Neurons[i]; ++k)
                        ForwardWeights[i].Body[j, k] += NeuronsValue[i][k] * NeuronsError[i + 1][j] * learningRange;

            for (var i = 0; i < ForwardLayouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; j++)
                    ForwardBias[i][j] += NeuronsError[i + 1][j] * learningRange;
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
                    for (var j = 0; j < Neurons[i + 1]; ++j)
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
                    for (var i = 0; i < Neurons[l + 1]; i++)
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

        public int FilterColumn;
        public int FilterRow;
        
        public int[] FilterCount;
        public int PoolSize;
        public int Stride;
        
        public int[] NeuronsLayer;
    }
}