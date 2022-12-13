using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Win32;

using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Configuration configuration) {
            try {
                Layouts        = configuration.Layout;
                Neurons        = new int[Layouts];

                for (var i = 0; i < Layouts; i++) Neurons[i] = configuration.NeuronsLayer[i];

                Configuration = configuration;

                Weights = new Matrix[Layouts - 1];
                Bias    = new double[Layouts - 1][];

                for (var i = 0; i < Layouts - 1; i++) {
                    Bias[i]    = new double[Neurons[i + 1]];
                    Weights[i] = new Matrix(Neurons[i + 1], Neurons[i]);
                
                    Weights[i].FillRandom();
                
                    for (var j = 0; j < Neurons[i + 1]; j++) 
                        Bias[i][j] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
                }
            
                NeuronsValue = new double[Layouts][];
                NeuronsError = new double[Layouts][];

                for (var i = 0; i < Layouts; i++) {
                    NeuronsValue[i] = new double[Neurons[i]];
                    NeuronsError[i] = new double[Neurons[i]];
                }
            
                NeuronsBios = new double[Layouts - 1];
                for (var i = 0; i < NeuronsBios.Length; i++) NeuronsBios[i] = 1;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой инициализации сети!");
                throw;
            }
        }
        
        public Configuration Configuration { get; }
        private int Layouts { get; }
        private int[] Neurons { get; }
        private Matrix[] Weights { get; }
        private double[][] Bias { get; }
        public double[][] NeuronsValue { get; }
        private double[][] NeuronsError { get; }
        private double[] NeuronsBios { get; }

        public void InsertInformation(List<double> values) {
            for (var i = 0; i < values.Count; i++) NeuronsValue[0][i] = values[i];
        }
        
        private int GetMaxIndex(IReadOnlyList<double> values) {
            try {
                var max = values[0];
                var prediction = 0;

                for (var j = 1; j < Neurons[Layouts - 1]; j++) {
                    var temp = values[j];
                    if (!(temp > max)) continue;
                
                    prediction = j;
                    max = temp;
                }

                return prediction;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой получения максимального индекса!");
                throw;
            }
        }
        
        public double ForwardFeed() {
            try {
                for (var k = 1; k < Layouts; ++k) {
                    NeuronsValue[k] = new Vector(Weights[k - 1] * NeuronsValue[k - 1]) + new Vector(Bias[k - 1]);
                    NeuronsValue[k] = NeuronActivate.Activation(NeuronsValue[k]);
                }

                return GetMaxIndex(NeuronsValue[Layouts - 1]);
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой активации нейронов!");
                throw;
            }
        }

        public void BackPropagation(double expectedAnswer) {
            try {
                for (var i = 0; i < Neurons[Layouts - 1]; i++) 
                    if (i != (int)expectedAnswer) 
                        NeuronsError[Layouts - 1][i] = -NeuronsValue[Layouts - 1][i] * 
                                                       NeuronActivate.GetDerivative(NeuronsValue[Layouts - 1][i]);
                    else 
                        NeuronsError[Layouts - 1][i] = (1.0 - NeuronsValue[Layouts - 1][i]) * 
                                                       NeuronActivate.GetDerivative(NeuronsValue[Layouts - 1][i]);
                
                for (var i = Layouts - 2; i > 0; i--) {
                    NeuronsError[i] = Weights[i].GetTranspose() * NeuronsError[i + 1];
                    for (var j = 0; j < Neurons[i]; j++)
                        NeuronsError[i][j] *= NeuronActivate.GetDerivative(NeuronsValue[i][j]);
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой обратного обучения!");
                throw;
            }
        }

        public void SetWeights(double learningRange) {
            for (var i = 0; i < Layouts - 1; ++i)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                    for (var k = 0; k < Neurons[i]; ++k)
                        Weights[i].Body[j, k] += NeuronsValue[i][k] * NeuronsError[i + 1][j] * learningRange;

            for (var i = 0; i < Layouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; j++)
                    Bias[i][j] += NeuronsError[i + 1][j] * learningRange;
        }

        private static string GetWeightsPath() {
            const string defaultWeights = @"Weights.txt";
            var file = new OpenFileDialog();
            var message = MessageBox.Show("Использовать стандартные веса вместо " +
                                          "других", "Укажите файл весов!", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes) return defaultWeights;
            return file.ShowDialog() != true ? "" : file.FileName;
        }

        private string _weightPath;
        public void SaveWeights() {
            try {
                MessageBox.Show("Начата запись весов!");
                var temp = Weights.Aggregate("", (current, weight) => current + weight.GetValues());

                for (var i = 0; i < Layouts - 1; i++)
                    for (var j = 0; j < Neurons[i + 1]; ++j)
                            temp += Bias[i][j] + " ";
                
                if (File.Exists(_weightPath)) File.WriteAllText(_weightPath, temp);
                else {
                    var file = new SaveFileDialog();
                    MessageBox.Show("Укажите место для сохранения весов!");
                    if (file.ShowDialog() == true) File.WriteAllText(file.FileName, temp);
                    return;
                }
                MessageBox.Show("Веса обновлены!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при записи весов!");
                throw;
            }
        }

        public void ReadWeights() {
            try {
                _weightPath = GetWeightsPath();
                if (!File.Exists(_weightPath)) return;
                
                var tempValues = File.ReadAllText(_weightPath).Split(" ", 
                    StringSplitOptions.RemoveEmptyEntries);

                var position = 0;
                
                for (var l = 0; l < Layouts - 1; l++) 
                    for (var i = 0; i < Weights[l].Body.GetLength(0); i++) 
                        for (var j = 0; j < Weights[l].Body.GetLength(1); j++) 
                            Weights[l].SetValues(tempValues[position++], i, j);

                for (var l = 0; l < Layouts - 1; l++)
                    for (var i = 0; i < Neurons[l + 1]; i++)
                        Bias[l][i] = double.Parse(tempValues[position++], CultureInfo.InvariantCulture);

            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при чтении весов!");
                throw;
            }
        }
    }
    
    public struct Configuration {
        public int Layout;
        public int[] NeuronsLayer;
    }
}