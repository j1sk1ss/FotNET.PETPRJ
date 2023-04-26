using FotNET.NETWORK.LAYERS.POOLING.SCRIPTS.AVERAGE;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.UP_SAMPLING.UP_SAMPLING_TYPE.NEAREST_NEIGHBOR;

/// <summary>
/// Nearest Neighbor up-sample method
/// </summary>
public class NearestNeighbor : UpSampling {
    protected override Matrix UpSample(Matrix matrix, int scale) {
        var output = new Matrix(matrix.Rows * scale, matrix.Columns * scale);

        Parallel.For(0, output.Rows, i => {
            for (var j = 0; j < output.Columns; j++) 
                            output.Body[i, j] = matrix.Body[i / scale, j / scale];
        });

        return output;
    }
    
    protected override Matrix DownSample(Matrix matrix, int scale) =>
         new AveragePooling().Pool(new Tensor(matrix), scale).Channels[0];
}