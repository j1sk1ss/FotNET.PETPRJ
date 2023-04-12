using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.MIN_MAX;

public class MinMax : INormalization {
    public Tensor Normalize(Tensor tensor) {
        var min = tensor.Min();
        var max = tensor.Max();
        var normalized = new List<Matrix>();

        foreach (var channel in tensor.Channels) {
            var normalizedChannel = new Matrix(channel.Rows, channel.Columns);

            for (var i = 0; i < channel.Rows; i++) 
                for (var j = 0; j < channel.Columns; j++) {
                    var value = channel.Body[i, j];
                    var normalizedValue = (value - min) / (max - min) * 255;
                    normalizedChannel.Body[i, j] = normalizedValue;
                }
            
            normalized.Add(normalizedChannel);
        }

        return new Tensor(normalized);
    }
}