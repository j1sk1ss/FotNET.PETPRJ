using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.BILINEAR;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.BILINEAR_INTERPOLATION;

/// <summary>
/// Bilinear Interpolation up-sample method
/// </summary>
public class BilinearInterpolation : UpSampling {
    protected override Matrix UpSample(Matrix matrix, int scale) {
        var srcWidth  = matrix.Rows;
        var srcHeight = matrix.Columns;
        var dstWidth  = srcWidth * scale;
        var dstHeight = srcHeight * scale;
        
        var result = new Matrix(dstHeight, dstWidth);

        Parallel.For(0, dstHeight, y => {
            for (var x = 0; x < dstWidth; x++) {
                var weightX1 = (double)x / scale - (double)x / scale;
                var weightX0 = 1.0 - weightX1;
                var weightY1 = (double)y / scale - (double)y / scale;
                var weightY0 = 1.0 - weightY1;

                var x0 = Math.Max(0, Math.Min((int)(double)x / scale, srcWidth - 1));
                var x1 = Math.Max(0, Math.Min((int)(double)x + 1, srcWidth - 1));
                var y0 = Math.Max(0, Math.Min((int)(double)y / scale, srcHeight - 1));
                var y1 = Math.Max(0, Math.Min((int)(double)y / scale + 1, srcHeight - 1));

                var value00 = matrix.Body[y0, x0];
                var value01 = matrix.Body[y1, x0];
                var value10 = matrix.Body[y0, x1];
                var value11 = matrix.Body[y1, x1];

                var interpolatedValue = weightX0 * weightY0 * value00
                                        + weightX1 * weightY0 * value10
                                        + weightX0 * weightY1 * value01
                                        + weightX1 * weightY1 * value11;

                result.Body[y, x] = interpolatedValue;
            }
        });

        return result;
    }

    protected override Matrix DownSample(Matrix matrix, int scale) =>
        new BilinearPooling().Pool(new Tensor(matrix), scale).Channels[0];
}