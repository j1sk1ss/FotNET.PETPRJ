using System.Collections.Generic;
using System.Windows;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Number {
        public Number(Configuration configuration) {
            Pixels        = new List<double>();
            Configuration = configuration;
        }
        
        private Configuration Configuration { get; set; }

        public List<double> Pixels { get; set; }
        public int Digit { get; set; }

        public string PrintNumber() {
            var temp = "";
            var position = 0;
            for (var i = 0; i < Configuration.Weight; i++) {
                for (var j = 0; j < Configuration.Height; j++) {
                    temp += Pixels[position++] + " ";
                }

                temp += "\n";
            }

            return temp;
        }

        public double[,] GetValues() {
            var temp = new double[Configuration.Weight, Configuration.Height];
            var position = 0;

            for (var i = 0; i < Configuration.Weight; i++)
                for (var j = 0; j < Configuration.Height; j++)
                    temp[i, j] = Pixels[position++];
            
            return temp;
        }

        public Matrix GetAsMatrix() {
            var temp = new Matrix(new double[Configuration.Weight, Configuration.Height]);
            var position = 0;

            for (var i = 0; i < Configuration.Weight; i++)
                for (var j = 0; j < Configuration.Height; j++)
                    temp.Body[i, j] = Pixels[position++];
            
            return temp;
        }
    }
}