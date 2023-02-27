using System.Collections.Generic;
using System.Linq;

namespace NeuroWeb.EXMPL.OBJECTS {
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
    }

    
    
    
    public class Filter : Tensor {
        public Filter(Matrix matrix) : base(matrix) {
            Channels = new List<Matrix> { matrix };
            Bias     = new Bias(new List<Matrix>());
        }

        public Filter(List<Matrix> matrix) : base(matrix) {
            Channels = matrix;
            Bias     = new Bias(new List<Matrix>());
        }
        
        public Bias Bias { get; set; }
    }

    
    
    
    public class Bias : Tensor {
        public Bias(Matrix matrix) : base(matrix) {
            Channels = new List<Matrix> { matrix };
        }

        public Bias(List<Matrix> matrix) : base(matrix) {
            Channels = matrix;
        }
    }
}