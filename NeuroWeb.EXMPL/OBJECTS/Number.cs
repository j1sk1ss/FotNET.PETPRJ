using System.Collections.Generic;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Number {
        public Number() {
            Pixels = new List<double>();
        }
        
        public List<double> Pixels { get; set; }
        public int Digit { get; set; }

        public string PrintNumber() {
            var temp = "";
            var position = 0;
            for (var i = 0; i < 28; i++) {
                for (var j = 0; j < 28; j++) {
                    temp += Pixels[position++] + " ";
                }

                temp += "\n";
            }

            return temp;
        }

        public double[,] GetValues() {
            var temp = new double[28, 28];
            var position = 0;

            for (var i = 0; i < 28; i++)
                for (var j = 0; j < 28; j++)
                    temp[i, j] = Pixels[position++];
            
            return temp;
        }
    }
}