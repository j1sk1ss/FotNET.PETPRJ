namespace NeuroWeb.EXMPL.SCRIPTS {
    public class NeuronActivate {
        public static double[] Activation(double[] value) {
            for (var i = 0; i < value.Length; i++) 
                switch (value[i]) {
                    case < 0:
                        value[i] *= 0.01d;
                        break;
                    case > 1:
                        value[i] = 1d + .01d * (value[i] - 1d);
                        break;
                }
            
            return value;
        }
        public static double GetDerivative(double value) => value is < 0 or > 1 ? 0.01d : value;
    }
}