using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.BICUBIC;
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
                var floorX = (int)Math.Floor(xScaled) - 1;
                var secondX = floorX + 1;
                var thirdX = floorX + 2;
                var fourthX = floorX + 3;
                
                floorX = Math.Max(0, Math.Min(floorX, matrix.Rows - 1));
                secondX = Math.Max(0, Math.Min(secondX, matrix.Rows - 1));
                thirdX = Math.Max(0, Math.Min(thirdX, matrix.Rows - 1));
                fourthX = Math.Max(0, Math.Min(fourthX, matrix.Rows - 1));

                result.Body[x, y] = BicubicInterpolateValue(matrix.Body[floorX, y], matrix.Body[secondX, y], 
                    matrix.Body[thirdX, y], matrix.Body[fourthX, y], xScaled - floorX);
            }
        }

        for (var x = 0; x < newWidth; x++) {
            for (var y = 0; y < newHeight; y++) {
                var yScaled = (double)y / scale;
                var floorY = (int)Math.Floor(yScaled) - 1;
                var secondY = floorY + 1;
                var thirdY = floorY + 2;
                var fourthY = floorY + 3;
                
                floorY = Math.Max(0, Math.Min(floorY, matrix.Columns - 1));
                secondY = Math.Max(0, Math.Min(secondY, matrix.Columns - 1));
                thirdY = Math.Max(0, Math.Min(thirdY, matrix.Columns - 1));
                fourthY = Math.Max(0, Math.Min(fourthY, matrix.Columns - 1));
                
                result.Body[x, y] = BicubicInterpolateValue(result.Body[x, floorY], result.Body[x, secondY], 
                    result.Body[x, thirdY], result.Body[x, fourthY], yScaled - floorY);
            }
        }

        return result;
    }
    
    private static double BicubicInterpolateValue(double firstValue, double secondValue, double thirdValue, double fourthValue, double x) {
        var a0 = -.5d * firstValue + 1.5d * secondValue - 1.5d * thirdValue + .5d *fourthValue;
        var a1 = firstValue - 2.5d * secondValue + 2d * thirdValue - .5d * fourthValue;
        var a2 = -.5d * firstValue + .5d * thirdValue;
        
        return a0*Math.Pow(x,3) + a1*Math.Pow(x,2) + a2*x + secondValue;
    }

    protected override Matrix DownSample(Matrix matrix, int scale) =>
        new BicubicPooling().Pool(new Tensor(matrix), scale).Channels[0];
}