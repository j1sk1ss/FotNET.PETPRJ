using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Data data) {
            NeuronActivate = new NeuronActivate();
            Layouts        = data.Layout;
            Neurons        = new int[Layouts];
            
            for (var i = 0; i < Layouts; i++) Neurons[i] = data.Size[i];

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
        
        private NeuronActivate NeuronActivate { get; }
        private int Layouts { get; }
        private int[] Neurons { get; }
        private Matrix[] Weights { get; }
        private double[][] Bios { get; }
        private double[][] NeuronsValue { get; }
        private double[][] NeuronsError { get; }
        private double[] NeuronsBios { get; }
        
        public string GetConfiguration() {
            var temp = $"Сеть имеет {Layouts} слоёв\nРазмерность:\n";
            
            for (var i = 0; i < Layouts; i++) temp += $"{Neurons[i]}";
            
            return temp;
        }
        
        public void InsertInformation(double[] values) {
            for (var i = 0; i < Neurons[0]; i++) NeuronsValue[0][i] = values[i];
        }
        
        private int GetMaxIndex(IReadOnlyList<double> values) {
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
        
        public double ForwardFeed() {
            for (var k = 1; k < Layouts; ++k) {
                NeuronsValue[k] = Weights[k - 1] * NeuronsValue[k - 1];
                NeuronsValue[k] = new Vector(NeuronsValue[k]) + new Vector(Bios[k - 1]);
                
                NeuronActivate.Use(NeuronsValue[k], Neurons[k]);
                NeuronsValue[k] = NeuronActivate.Neurons;
            }

            return GetMaxIndex(NeuronsValue[Layouts - 1]);;
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
                NeuronsError[i] = Weights[i].GetTransparent() * NeuronsError[i + 1];
                for (var j = 0; j < Neurons[i]; j++)
                    NeuronsError[i][j] *= NeuronActivate.UseDer(NeuronsValue[i][j]);
            }
        }

        public void SetWeights(double lr) {
            for (var i = 0; i < Layouts - 1; ++i)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                    for (var k = 0; k < Neurons[i]; ++k)
                        Weights[i].Body[j, k] += NeuronsValue[i][k] * NeuronsError[i + 1][j] * lr;

            for (var i = 0; i < Layouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; j++)
                    Bios[i][j] += NeuronsError[i + 1][j] * lr;
        }
        
        public string Values(int layout) {
            var temp = "";
            
            for (var i = 0; i < Neurons[layout]; i++)
                temp += $"{i}) {NeuronsValue[layout][i]}\n";
            
            return temp;
        }

        public void SaveWeights() {
            var temp = Weights.Aggregate("", (current, weight) => current + weight.GetValues());

            for (var i = 0; i < Layouts - 1; i++)
                for (var j = 0; j < Neurons[i + 1]; ++j)
                        temp += Bios[i][j] + " ";
            
            File.WriteAllText("Weights.txt", temp);
            MessageBox.Show("Weights are saved!");
        }

        public void ReadWeights() {
            var count = Weights.Sum(t => t.Body.GetLength(0) * t.Body.GetLength(1));

            var tempValues = File.ReadAllText("Weights.txt").Split(" ", 
                StringSplitOptions.RemoveEmptyEntries);

            var position = 0;
            
            for (var l = 0; l < Layouts - 1; l++) {
                if (position >= count) break;
                for (var i = 0; i < Weights[i].Body.GetLength(0); i++) {
                    for (var j = 0; j < Weights[i].Body.GetLength(1); j++) {
                        Weights[i].SetValues(tempValues[position++], i, j);
                    }
                }
            }

            for (var l = 0; l < Layouts - 1; l++) {
                for (var i = 0; i < Neurons[i + 1]; i++) {
                    if (double.TryParse(tempValues[position++], out var tempDb)) {
                        Bios[l][i] = tempDb;
                    }
                }
            }
            MessageBox.Show("Weights are read!");
        }
    }
    
    public struct Data {
        public int Layout;
        public int[] Size;
    }
}