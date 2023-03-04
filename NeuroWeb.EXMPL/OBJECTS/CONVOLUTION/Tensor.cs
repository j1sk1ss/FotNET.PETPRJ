using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuroWeb.EXMPL.OBJECTS.CONVOLUTION {
    public class Tensor {
        public Tensor(Matrix matrix) => Channels = new List<Matrix> { matrix };
        
        public Tensor(List<Matrix> matrix) => Channels = matrix;

        public Tensor(int x, int y, int z) {
            Channels = new List<Matrix>();
            for (var i = 0; i < z; i++)
                Channels.Add(new Matrix(x,y));
        }
        
        public List<Matrix> Channels { get; set; }
        
        public List<double> Flatten() {
            var a = new List<double>();
            foreach (var matrix in Channels) a.AddRange(matrix.GetAsList());
            return a;
        }
        
        public Filter GetFlipped() {
            var tensor = Channels;
            
            foreach (var matrix in tensor)
                matrix.GetFlip();
            
            return new Filter(tensor);
        }

        public Tensor GetSameChannels(Tensor reference) {
            var newTensor = this;

            if (newTensor.Channels.Count != reference.Channels.Count) {
                newTensor = newTensor.Channels.Count < reference.Channels.Count
                    ? newTensor.IncreaseChannels(reference.Channels.Count - newTensor.Channels.Count)
                    : newTensor.CropChannels(reference.Channels.Count);
            }

            return newTensor;
        }

        private Tensor IncreaseChannels(int channels) {
            var tensor = new Tensor(Channels);
            
            for (var i = 0; i < channels; i++) tensor.Channels.Add(tensor.Channels[^1]);
            
            return tensor;
        }
        
        private Tensor CropChannels(int channels) {
            var matrix = new List<Matrix>();

            for (var i = 0; i < channels * 2; i += 2) {
                matrix.Add(Channels[i]);
                for (var x = 0; x < Channels[i].Body.GetLength(0); x++) {
                    for (var y = 0; y < Channels[i].Body.GetLength(1); y++) {
                        matrix[^1].Body[x,y] = Math.Min(Channels[i].Body[x, y], Channels[i + 1].Body[x, y]);
                    }
                }
            }
            
            return new Tensor(matrix);
        }
        
        public double TensorSum() => Channels.Sum(matrix => matrix.GetSum());
        
        public static Tensor operator +(Tensor tensor1, Tensor tensor2) {
            var endTensor = new Tensor(tensor1.Channels);
            
            for (var i = 0; i < endTensor.Channels.Count; i++) 
                 for (var j = 0; j < endTensor.Channels[i].Body.GetLength(0); j++)
                     for (var k = 0; k < endTensor.Channels[i].Body.GetLength(1); k++)
                         endTensor.Channels[i].Body[j, k] += tensor2.Channels[i].Body[j, k];
            
            return endTensor;
        }
        
        public static Tensor operator -(Tensor tensor1, Tensor tensor2) {
            var endTensor = new Tensor(tensor1.Channels);
            
            for (var i = 0; i < endTensor.Channels.Count; i++) 
                for (var j = 0; j < endTensor.Channels[i].Body.GetLength(0); j++)
                    for (var k = 0; k < endTensor.Channels[i].Body.GetLength(1); k++)
                        endTensor.Channels[i].Body[j, k] -= tensor2.Channels[i].Body[j, k];
            
            return endTensor;
        }
        
        public static Tensor operator *(Tensor tensor1, Tensor tensor2) {
            var endTensor = new Tensor(tensor1.Channels);
            
            for (var i = 0; i < endTensor.Channels.Count; i++) 
                for (var j = 0; j < tensor1.Channels[i].Body.GetLength(0); j++)
                    for (var k = 0; k < tensor1.Channels[i].Body.GetLength(1); k++)
                        endTensor.Channels[i].Body[j, k] *= tensor2.Channels[i].Body[j,k];

            return endTensor;
        }
        
        public static Tensor operator *(Tensor tensor1, double value) {
            var endTensor = new Tensor(tensor1.Channels);
            
            foreach (var t in endTensor.Channels)
                for (var j = 0; j < t.Body.GetLength(0); j++)
                    for (var k = 0; k < t.Body.GetLength(1); k++)
                        t.Body[j, k] *= value;

            return endTensor;
        }
        
        public static Tensor operator -(Tensor tensor1, double value) {
            var endTensor = new Tensor(tensor1.Channels);
            
            foreach (var t in endTensor.Channels)
                for (var j = 0; j < t.Body.GetLength(0); j++)
                    for (var k = 0; k < t.Body.GetLength(1); k++)
                        t.Body[j, k] -= value;

            return endTensor;
        }

        public string GetInfo() {
            return $"x: {Channels[0].Body.GetLength(0)}\n" +
                   $"y: {Channels[0].Body.GetLength(1)}\n" +
                   $"depth: {Channels.Count}";
        }

        public Filter AsFilter() {
            return new Filter(Channels);
        }

        public Tensor Resize(int x, int y) {
            var tensor = new Tensor(Channels);

            for (var i = 0; i < tensor.Channels.Count; i++) tensor.Channels[i] = tensor.Channels[i].Resize(x, y);;
            
            return tensor;
        }
    }
    
    public class Filter : Tensor {
        public Filter(List<Matrix> matrix) : base(matrix) {
            Bias = new List<double>();
            for (var i = 0; i < matrix.Count; i++) Bias.Add(0);
            
            Channels = matrix;
        }
        
        public List<double> Bias { get; set; }
        
        public static Filter operator -(Filter tensor1, Tensor tensor2) {
            var endTensor = new Filter(tensor1.Channels); 

            for (var i = 0; i < endTensor.Channels.Count; i++) 
            for (var j = 0; j < endTensor.Channels[i].Body.GetLength(0); j++)
            for (var k = 0; k < endTensor.Channels[i].Body.GetLength(1); k++) {
                endTensor.Channels[i].Body[j, k] -= tensor2.Channels[i].Body[j, k];
            }

            
            return endTensor;
        }
        
        public static Filter operator +(Filter tensor1, Filter tensor2) {
            var endTensor    = new Filter(tensor1.Channels); 

            for (var i = 0; i < endTensor.Channels.Count; i++) 
            for (var j = 0; j < endTensor.Channels[i].Body.GetLength(0); j++)
            for (var k = 0; k < endTensor.Channels[i].Body.GetLength(1); k++) {
                endTensor.Channels[i].Body[j, k] += tensor2.Channels[i].Body[j, k];
            }

            
            return endTensor;
        }
        
        public static Filter operator *(Filter tensor1, double value) {
            var endTensor = new Filter(tensor1.Channels);
            
            foreach (var t in endTensor.Channels)
                for (var j = 0; j < t.Body.GetLength(0); j++)
                for (var k = 0; k < t.Body.GetLength(1); k++)
                    t.Body[j, k] *= value;

            return endTensor;
        }
        
        public static Filter operator -(Filter tensor1, double value) {
            var endTensor = new Filter(tensor1.Channels);
            
            foreach (var t in endTensor.Channels)
                for (var j = 0; j < t.Body.GetLength(0); j++)
                for (var k = 0; k < t.Body.GetLength(1); k++)
                    t.Body[j, k] -= value;

            return endTensor;
        }
    }
}