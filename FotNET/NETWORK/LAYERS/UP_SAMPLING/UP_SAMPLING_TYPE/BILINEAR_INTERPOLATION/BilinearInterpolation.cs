using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.BILINEAR;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.BILINEAR_INTERPOLATION;

public class BilinearInterpolation : UpSampling {
    protected override Matrix UpSample(Matrix matrix, int scale) {
        var srcWidth  = matrix.Rows;
        var srcHeight = matrix.Columns;
        var dstWidth  = srcWidth * scale;
        var dstHeight = srcHeight * scale;
        
        var result = new Matrix(dstHeight, dstWidth);
        for (var y = 0; y < dstHeight; y++) {
            for (var x = 0; x < dstWidth; x++) {
                var srcX = (double)x / scale;
                var srcY = (double)y / scale;

                var x0 = (int)srcX;
                var x1 = x0 + 1;
                var y0 = (int)srcY;
                var y1 = y0 + 1;

                var weightX1 = srcX - x0;
                var weightX0 = 1.0 - weightX1;
                var weightY1 = srcY - y0;
                var weightY0 = 1.0 - weightY1;

                x0 = Math.Max(0, Math.Min(x0, srcWidth - 1));
                x1 = Math.Max(0, Math.Min(x1, srcWidth - 1));
                y0 = Math.Max(0, Math.Min(y0, srcHeight - 1));
                y1 = Math.Max(0, Math.Min(y1, srcHeight - 1));

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
        }

        return result;
    }

    protected override Matrix DownSample(Matrix matrix, int scale) =>
        new BilinearPooling().Pool(new Tensor(matrix), scale).Channels[0];
}