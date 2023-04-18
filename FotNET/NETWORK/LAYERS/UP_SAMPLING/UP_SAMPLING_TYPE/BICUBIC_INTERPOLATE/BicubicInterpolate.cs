using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.BICUBIC_INTERPOLATE;

/// <summary>
/// Bicubic Interpolate up-sample method
/// </summary>
public class BicubicInterpolate : UpSampling {
    protected override Matrix UpSample(Matrix matrix, int scale) {
        var newWidth = matrix.Rows * scale;
        var newHeight = matrix.Columns * scale;
        var result = new Matrix(newWidth, newHeight);

        for (var y = 0; y < matrix.Columns; y++) {
            for (var x = 0; x < newWidth; x++) {
                var xScaled = (double)x / scale;
                var x0 = (int)Math.Floor(xScaled) - 1;
                var x1 = x0 + 1;
                var x2 = x0 + 2;
                var x3 = x0 + 3;
                
                x0 = Math.Max(0, Math.Min(x0, matrix.Rows - 1));
                x1 = Math.Max(0, Math.Min(x1, matrix.Rows - 1));
                x2 = Math.Max(0, Math.Min(x2, matrix.Rows - 1));
                x3 = Math.Max(0, Math.Min(x3, matrix.Rows - 1));

                result.Body[x, y] = BicubicInterpolateValue(matrix.Body[x0, y], matrix.Body[x1, y], 
                    matrix.Body[x2, y], matrix.Body[x3, y], xScaled - x0);
            }
        }

        for (var x = 0; x < newWidth; x++) {
            for (var y = 0; y < newHeight; y++) {
                var yScaled = (double)y / scale;
                var y0 = (int)Math.Floor(yScaled) - 1;
                var y1 = y0 + 1;
                var y2 = y0 + 2;
                var y3 = y0 + 3;
                
                y0 = Math.Max(0, Math.Min(y0, matrix.Columns - 1));
                y1 = Math.Max(0, Math.Min(y1, matrix.Columns - 1));
                y2 = Math.Max(0, Math.Min(y2, matrix.Columns - 1));
                y3 = Math.Max(0, Math.Min(y3, matrix.Columns - 1));
                
                result.Body[x, y] = BicubicInterpolateValue(result.Body[x, y0], result.Body[x, y1], 
                    result.Body[x, y2], result.Body[x, y3], yScaled - y0);
            }
        }

        return result;
    }
    
    private static double BicubicInterpolateValue(double p0, double p1, double p2, double p3, double x) {
        var a0 = -0.5*p0 + 1.5*p1 - 1.5*p2 + 0.5*p3;
        var a1 = p0 - 2.5*p1 + 2*p2 - 0.5*p3;
        var a2 = -0.5*p0 + 0.5*p2;
        
        return a0*Math.Pow(x,3) + a1*Math.Pow(x,2) + a2*x + p1;
    }
    
    protected override Matrix DownSample(Matrix matrix, int scale)
    {
        throw new NotImplementedException();
    }
}