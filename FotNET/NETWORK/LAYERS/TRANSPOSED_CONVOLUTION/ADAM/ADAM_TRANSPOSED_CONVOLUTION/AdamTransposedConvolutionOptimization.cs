using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.ADAM.ADAM_TRANSPOSED_CONVOLUTION;

/// <summary>
/// Transposed convolution with Adam optimization
/// </summary>
public class AdamTransposedConvolutionOptimization : ITransposedConvolutionOptimization {
    private static Filter[] FlipFilters(Filter[] filters) {
        for (var i = 0; i < filters.Length; i++)
            filters[i] = filters[i].Flip().AsFilter();

        return filters;
    }

    private static Filter[] GetFiltersWithoutBiases(Filter[] filters) {
        for (var i = 0; i < filters.Length; i++)
            filters[i] = new Filter(filters[i].Channels);

        return filters;
    }
    
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input, Filter[] filters, int stride,
        double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8) {

        var inputTensor = input;
        var extendedError = error.GetSameChannels(inputTensor);

        var originalFilters = new Filter[filters.Length];
        for (var i = 0; i < filters.Length; i++)
            originalFilters[i] = new Filter(new List<Matrix>(filters[i].Channels));

        for (var i = 0; i < originalFilters.Length; i++)
            originalFilters[i] = originalFilters[i].GetSameChannels(error).AsFilter();

        if (backPropagate) {
            var m = new Filter[filters.Length];
            var v = new Filter[filters.Length];

            for (var i = 0; i < filters.Length; i++) {
                m[i] = new Filter(new List<Matrix>(filters[i].Channels.Select(channel => new Matrix(channel.Rows, channel.Columns))));
                v[i] = new Filter(new List<Matrix>(filters[i].Channels.Select(channel => new Matrix(channel.Rows, channel.Columns))));
            }
            
            var t = 0;
            Parallel.For(0, filters.Length, filter => {
                for (var channel = 0; channel < filters[filter].Channels.Count; channel++) {
                    var grad = Convolution.GetConvolution(extendedError.Channels[filter], input.Channels[filter], stride, filters[filter].Bias);
                    m[filter].Channels[channel] = m[filter].Channels[channel] * beta1 + grad * (1 - beta1);
                    v[filter].Channels[channel] = v[filter].Channels[channel] * beta2 + grad * grad * (1 - beta2);
                    var mHat = m[filter].Channels[channel] / (1 - Math.Pow(beta1, t + 1));
                    var vHat = v[filter].Channels[channel] / (1 - Math.Pow(beta2, t + 1));
                    filters[filter].Channels[channel] -= mHat * learningRate / (vHat.Sqrt() + epsilon);
                }

                filters[filter].Bias -= error.Channels[filter].Sum() * learningRate;
            });

            t++;
        }

        return Convolution.GetConvolution(extendedError, 
            FlipFilters(GetFiltersWithoutBiases(originalFilters)), stride);
    }
}