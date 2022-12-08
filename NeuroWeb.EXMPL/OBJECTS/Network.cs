using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Configuration configuration) {
            try {
                NeuronActivate = new NeuronActivate();
                Layouts        = configuration.Layout;
                Neurons        = new int[Layouts];

                for (var i = 0; i < Layouts; i++) Neurons[i] = configuration.NeuronsLayer[i];

                Configuration = configuration;
                
                Weights = new Matrix[Layouts - 1];
                Bios    = new double[Layouts - 1][];

                for (var i = 0; i < Layouts - 1; i++) {
                    Bios[i]    = new double[Neurons[i + 1]];
                    Weights[i] = new Matrix(Neurons[i + 1], Neurons[i]);
                
                    Weights[i].FillRandom();
                
                    for (var j = 0; j < Neurons[i + 1]; j++) {
                        Bios[i][j] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
                    }
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
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        public Configuration Configuration { get; }
        private NeuronActivate NeuronActivate { get; }
        private int Layouts { get; }
        private int[] Neurons { get; }
        private Matrix[] Weights { get; }
        private double[][] Bios { get; }
        public double[][] NeuronsValue { get; }
        private double[][] NeuronsError { get; }
        private double[] NeuronsBios { get; }
        
        public string GetConfiguration() {
            var temp = $"Сеть имеет {Layouts} слоёв\nРазмерность:\n";
            
            for (var i = 0; i < Layouts; i++) temp += $"{Neurons[i]}";
            
            return temp;
        }
        
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
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        public double ForwardFeed() {
            for (var k = 1; k < Layouts; ++k) {
                NeuronsValue[k] = Weights[k - 1] * NeuronsValue[k - 1];
                NeuronsValue[k] = new Vector(NeuronsValue[k]) + new Vector(Bios[k - 1]);

                NeuronActivate.Use(NeuronsValue[k], Neurons[k]);
                NeuronsValue[k] = NeuronActivate.Neurons;
            }

            return GetMaxIndex(NeuronsValue[Layouts - 1]);
        }

        public void BackPropagation(double expectedAnswer) {
            for (var i = 0; i < Neurons[Layouts - 1]; i++) {
                if (i != (int)expectedAnswer) {
                    NeuronsError[Layouts - 1][i] = -NeuronsValue[Layouts - 1][i] * 
                                                   NeuronActivate.UseDer(NeuronsValue[Layouts - 1][i]);
                }
                else {
                    NeuronsError[Layouts - 1][i] = (1.0 - NeuronsValue[Layouts - 1][i]) * 
                                                   NeuronActivate.UseDer(NeuronsValue[Layouts - 1][i]);
                }
            }

            for (var i = Layouts - 2; i > 0; i--) {
                NeuronsError[i] = Weights[i].GetTranspose() * NeuronsError[i + 1];
                for (var j = 0; j < Neurons[i]; j++)
                    NeuronsError[i][j] *= NeuronActivate.UseDer(NeuronsValue[i][j]);
            }
        }

        public void SetWeights(double learningRange) {
            for (var i = 0; i < Layouts - 1; ++i)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                    for (var k = 0; k < Neurons[i]; ++k)
                        Weights[i].Body[j, k] += NeuronsValue[i][k] * NeuronsError[i + 1][j] * learningRange;

            for (var i = 0; i < Layouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; j++)
                    Bios[i][j] += NeuronsError[i + 1][j] * learningRange;
        }

        private const string WeightsPath =
            @"C:\\Users\\j1sk1ss\\RiderProjects\\NeuroWeb.EXMPL\\NeuroWeb.EXMPL\\DATA\\Weights.txt";
        
        public void SaveWeights() {
            var temp = Weights.Aggregate("", (current, weight) => current + weight.GetValues());

            for (var i = 0; i < Layouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                        temp += Bios[i][j] + " ";
            
            File.WriteAllText(WeightsPath, temp);
            MessageBox.Show("Weights are saved!");
        }

        public void ReadWeights() {
            try {
                var tempValues = File.ReadAllText(WeightsPath).Split(" ", 
                    StringSplitOptions.RemoveEmptyEntries);

                var position = 0;
                
                for (var l = 0; l < Layouts - 1; l++) {
                    for (var i = 0; i < Weights[l].Body.GetLength(0); i++) {
                        for (var j = 0; j < Weights[l].Body.GetLength(1); j++) {
                            Weights[l].SetValues(tempValues[position++], i, j);
                        }
                    }
                }

                for (var l = 0; l < Layouts - 1; l++) {
                    for (var i = 0; i < Neurons[l + 1]; i++) {
                        if (double.TryParse(tempValues[position++], out var tempDb)) {
                            Bios[l][i] = tempDb;
                        }
                    }
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
    }
    
    public struct Configuration {
        public int Layout;
        public int[] NeuronsLayer;
    }
}