namespace NeuroWeb.EXMPL.INTERFACES
{
    public abstract class Matrix
    {
        double[,] matrix { get; set; }
        int Row { get; set; }
        int Col { get; set; }

        public void Init(int row, int col) {}
        void Rand() {}
        static void Multi(ref Matrix m, double[] b, int n, double[] c) {}
        static void SumVector(double[] a, double[] b, int n) {}
    }
}