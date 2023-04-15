using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.NORMALIZATION.NORMALIZATION_TYPE.MIN_MAX;

public class MinMax : INormalization {
    /// <summary>
    /// Type of normalization that make all values between chosen params
    /// </summary>
    /// <param name="value"> Max value of normalized tensor </param>
    public MinMax(double value) => Coefficient = value;
    
    private double Coefficient { get; }
    
    public Tensor Normalize(Tensor tensor) {
        var min = tensor.Min();
        var max = tensor.Max();
        
        var normalized = new List<Matrix>(tensor.Channels);

        Parallel.For(0, tensor.Channels.Count, channel => {
            var normalizedChannel = new Matrix(tensor.Channels[channel].Rows, tensor.Channels[channel].Columns);

            for (var i = 0; i < tensor.Channels[channel].Rows; i++) 
                for (var j = 0; j < tensor.Channels[channel].Columns; j++) {
                    var value = tensor.Channels[channel].Body[i, j];
                    var normalizedValue = (value - min) / (max - min) * Coefficient;
                    normalizedChannel.Body[i, j] = normalizedValue;
                }
            
            normalized[channel] = normalizedChannel;
        });

        return new Tensor(normalized);
    }
}