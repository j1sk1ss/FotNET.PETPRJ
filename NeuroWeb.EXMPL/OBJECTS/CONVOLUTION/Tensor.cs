using System.Collections.Generic;
using System.Linq;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Tensor {
        public Tensor(Matrix matrix) => Channels = new List<Matrix> { matrix };
        
        public Tensor(List<Matrix> matrix) => Channels = matrix;
        
        public List<Matrix> Channels { get; set; }

        public List<double> GetValues() {
            var a = new List<double>();
            return Channels.Aggregate(a, (current, matrix) => (List<double>)current.Concat(matrix.GetAsList()));
        }
    }

    public class Filter : Tensor {
        public Filter(Matrix matrix) : base(matrix) {
            Channels = new List<Matrix> { matrix };
        }

        public Filter(List<Matrix> matrix) : base(matrix) {
            Channels = matrix;
        }
        
        public double Bias { get; set; }
    }
}