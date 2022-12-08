namespace NeuroWeb.EXMPL.SCRIPTS {
    public class NeuronActivate {
        public double[] Neurons { get; private set; }
        public void Use(double[] value, int n) {
            for (var i = 0; i < n; i++) 
                switch (value[i]) {
                    case < 0:
                        value[i] *= 0.01d;
                        break;
                    case > 1:
                        value[i] = 1d + .01d * (value[i] - 1d);
                        break;
                }
            
            Neurons = value;
        }
        public void UseDer(double[] value) {
            for (var i = 0; i < value.Length; i++) 
                if (value[i] < 0 || value[i] > 1) value[i] = 0.01d;
                else value[i] = 1;
            
            Neurons = value;
        }
        public static double UseDer(double value) => value is < 0 or > 1 ? 0.01 : value;
    }
}