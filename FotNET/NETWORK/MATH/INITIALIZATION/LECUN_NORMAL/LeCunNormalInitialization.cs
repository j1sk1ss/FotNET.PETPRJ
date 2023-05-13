using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.MATH.Initialization.LECUN_NORMAL;

/// <summary>
/// Le Cun Normal Initialization usually uses with SeLU function
/// </summary>
public class LeCunNormalInitialization : IWeightsInitialization {
    public Matrix Initialize(Matrix matrix) {
        for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++) {
                var stdDev = Math.Sqrt(1.0 / matrix.Rows);
                matrix.Body[i,j] = NextGaussian(0, stdDev);
            }

        return matrix;
    }
    
    private static double NextGaussian(double mean, double stdDev) => 
        mean + stdDev * Math.Sqrt(-2.0 * Math.Log(1.0 - new Random().NextDouble())) 
                      * Math.Sin(2.0 * Math.PI * (1.0 - new Random().NextDouble()));
    
}