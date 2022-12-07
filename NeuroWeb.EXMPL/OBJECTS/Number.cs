using System.Collections.Generic;

namespace NeuroWeb.EXMPL.OBJECTS {
    public class Number {
        public Number() {
            Pixels = new List<double>();
        }
        public List<double> Pixels { get; set; }
        public int Digit { get; set; }
    }
}