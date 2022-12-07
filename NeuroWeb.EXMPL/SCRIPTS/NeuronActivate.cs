using System;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public class NeuronActivate {
        public double[] Neurons { get; private set; }
        public void Use(double[] value, int n) { 
            for (var i = 0; i < n; i++)
                value[i] = 1 / (1 + Math.Exp(-value[i]));
            Neurons = value;
        }
        public void UseDer(double[] value) {
            for (var i = 0; i < value.Length; i++)
                value[i] *= 1 - value[i];
            Neurons = value;
        }
        public static double UseDer(double value) => 1 / (1 + Math.Exp(-value));
    }
}