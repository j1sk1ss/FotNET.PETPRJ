using System.Collections.Generic;
using System.Linq;

namespace NeuroWeb.EXMPL.OBJECTS.CONVOLUTION {
    public class Tensor {
        public Tensor(Matrix matrix) => Channels = new List<Matrix> { matrix };
        
        public Tensor(List<Matrix> matrix) => Channels = matrix;
        
        public List<Matrix> Channels { get; set; }
        
        public List<double> GetValues() {
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

        public Tensor IncreaseChannels(int channels) {
            var tensor = new Tensor(Channels);
            for (var i = 0; i < channels; i++) tensor.Channels.Add(Channels[^1]);
            return tensor;
        }
        
        public Tensor CropChannels(int channels) {
            var matrix = new List<Matrix>();
            for (var i = 0; i < channels; i++) {
                matrix.Add(Channels[i]);
            }
            return new Tensor(matrix);
        }
        
        public double TensorSum() => Channels.Sum(matrix => matrix.GetSum());
        
        public static Tensor operator -(Tensor tensor1, Tensor tensor2) {
            var endTensor = new Tensor(tensor1.Channels);
            
            for (var i = 0; i < endTensor.Channels.Count; i++) 
                 for (var j = 0; j < endTensor.Channels[i].Body.GetLength(0); j++)
                     for (var k = 0; k < endTensor.Channels[i].Body.GetLength(1); k++)
                         endTensor.Channels[i].Body[j, k] -= tensor2.Channels[i].Body[j, k];
            
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
        public Filter(Matrix matrix) : base(matrix) {
            Channels = new List<Matrix> { matrix };
            Bias     = 0;
        }

        public Filter(List<Matrix> matrix) : base(matrix) {
            Channels = matrix;
            Bias     = 0;
        }
        
        public double Bias { get; set; }
        
        public static Filter operator -(Filter tensor1, Tensor tensor2) {
            var endTensor    = new Filter(tensor1.Channels);
            var secondTenser = new Tensor(tensor2.Channels)
                .Resize(endTensor.Channels[0].Body.GetLength(0), endTensor.Channels[0].Body.GetLength(1));

            for (var i = 0; i < endTensor.Channels.Count; i++) 
            for (var j = 0; j < endTensor.Channels[i].Body.GetLength(0); j++)
            for (var k = 0; k < endTensor.Channels[i].Body.GetLength(1); k++) {
                endTensor.Channels[i].Body[j, k] -= secondTenser.Channels[i].Body[j, k];
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