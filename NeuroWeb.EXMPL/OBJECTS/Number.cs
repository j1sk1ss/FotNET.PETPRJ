using System.Collections.Generic;
using NeuroWeb.EXMPL.OBJECTS.MATH;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Number {
        public Number() {
            Pixels = new List<double>();
        }

        public List<double> Pixels { get; }
        public int Digit { get; set; }
        
        public Matrix GetAsMatrix() {
            var temp = new Matrix(new double[28, 28]);
            var position = 0;

            for (var i = 0; i < 28; i++)
                for (var j = 0; j < 28; j++)
                    temp.Body[i, j] = Pixels[position++];
            
            return temp;
        }
    }
}