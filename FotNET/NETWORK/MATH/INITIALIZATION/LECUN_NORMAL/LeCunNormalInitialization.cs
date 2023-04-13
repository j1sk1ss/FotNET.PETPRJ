using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.LECUN_NORMAL;

public class LeCunNormalInitialization : IWeightsInitialization {
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++) {
                var stdDev = Math.Sqrt(1.0 / matrix.Rows);
                matrix.Body[i,j] = NextGaussian(0, stdDev);
            }

        return matrix;
    }
    
    private double NextGaussian(double mean, double stdDev) {
        var u1 = 1.0 - new Random().NextDouble();
        var u2 = 1.0 - new Random().NextDouble();
        var normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * normal;
    }
}