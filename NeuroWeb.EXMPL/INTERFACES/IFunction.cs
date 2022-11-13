namespace NeuroWeb.EXMPL.INTERFACES {
    public interface IFunction {
        public enum FunctionType {
            sigmoid = 1,
            ReLU,
            thx
        }
        public void Set();
        public void Use(double[] value, int n);
        public void UseDer(double[] value, int n);
        public double UseDer(double value);
    }
}