using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.ABS;
/// <summary>
/// Normalization type that normalize vector with Abs
/// </summary>
public class Abs : INormalization {
    public Tensor Normalize(Tensor tensor) {
        var newTensor = new Tensor(new List<Matrix>(tensor.Channels));
        
        Parallel.For(0, tensor.Channels.Count, channel => {
            var normalizedChannel = new Matrix(tensor.Channels[channel].Rows, tensor.Channels[channel].Columns);

            for (var i = 0; i < tensor.Channels[channel].Rows; i++) 
                for (var j = 0; j < tensor.Channels[channel].Columns; j++) {
                    var value = tensor.Channels[channel].Body[i, j];
                    var normalizedValue = Math.Abs(tensor.Channels[channel].Body[i,j]);
                    normalizedChannel.Body[i, j] = normalizedValue;
                }
            
            newTensor.Channels[channel] = normalizedChannel;
        });
        
        return newTensor;
    }
}