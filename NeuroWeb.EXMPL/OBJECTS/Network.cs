using System;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Network {
        public Network(Data data) {
            NeuronActivate = new NeuronActivate();
            
            Layouts = data.Layout;
            Neurons = new int[Layouts];
            
            for (var i = 0; i < Layouts; i++) Neurons[i] = data.Size[i];

            Weights = new Matrix[Layouts - 1];
            Bios = new double[Layouts - 1, Neurons.Length];

            for (var i = 0; i < Layouts - 1; i++) {
                Weights[i] = new Matrix(Neurons[i + 1], Neurons[i]);
                Weights[i].FillRandom();
                for (var j = 0; j < Neurons[i + 1]; j++) {
                    Bios[i, j] = new Random().Next() % 50 * .06 / (Neurons[i] + 15);
                }
            }
            
            NeuronsValue = new double[Layouts, Neurons.Length];
            NeuronsError = new double[Layouts, Neurons.Length];
            
            NeuronsBios = new double[Layouts - 1];
            for (var i = 0; i < NeuronsBios.Length; i++) NeuronsBios[i] = 1;
        }
        private NeuronActivate NeuronActivate { get; set; }
        private int Layouts { get; set; }
        private int[] Neurons { get; set; }
        private Matrix[] Weights { get; set; }
        private double[,] Bios { get; set; }
        private double[,] NeuronsValue { get; set; }
        private double[,] NeuronsError { get; set; }
        private double[] NeuronsBios { get; set; }
        public string PrintConfiguration() {
            var temp = "***********************************************************************************\n" +
                       $"Сеть имеет {Layouts} слоёв\nРазмерность:\n";
            
            for (var i = 0; i < Layouts; i++) {
                temp += $"{Neurons[i]}";
            }
            
            return temp;
        }
        public void InsertInformation(double[] values) {
            for (var i = 0; i < Neurons[0]; i++) NeuronsValue[0, i] = values[i];
        }
        private int GetMaxIndex(double[] values)
        {
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
        public double ForwardFeed()
        {
            var matrixWorker = new MatrixWorker();
            for (var k = 1; k < Layouts; ++k)
            {
                NeuronsValue = matrixWorker.SetRow(NeuronsValue, k, Weights[k - 1] * matrixWorker.GetRow(NeuronsValue, k - 1));
                
                var temp = new Vector(matrixWorker.GetRow(NeuronsValue, k)) + new Vector(matrixWorker.GetRow(Bios, k - 1));
                NeuronsValue = matrixWorker.SetRow(NeuronsValue, k, temp);
                
                NeuronActivate.Use(matrixWorker.GetRow(NeuronsValue, k), Neurons[k]);
                NeuronsValue = matrixWorker.SetRow(NeuronsValue, k, NeuronActivate.Neurons);
            }

            var pred = GetMaxIndex(matrixWorker.GetRow(NeuronsValue, Layouts - 1));

            return pred;
        }
        public string Values(int layout) {
            var temp = "";
            for (var i = 0; i < Neurons[layout]; i++)
            {
                temp += $"{i} {NeuronsValue[layout,i]}\n";
            }

            return temp;
        }
    }


    public struct Data {
        public int Layout;
        public int[] Size;
    }
}