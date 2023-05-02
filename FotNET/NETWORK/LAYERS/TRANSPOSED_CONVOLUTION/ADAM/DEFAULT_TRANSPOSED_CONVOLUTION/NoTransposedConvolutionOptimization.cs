using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.ADAM.DEFAULT_TRANSPOSED_CONVOLUTION;

/// <summary>
/// Transposed convolution without any optimization
/// </summary>
public class NoTransposedConvolutionOptimization : ITransposedConvolutionOptimization {
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

        if (backPropagate)
            Parallel.For(0, filters.Length, filter => {
                for (var channel = 0; channel < filters[filter].Channels.Count; channel++) {
                    filters[filter].Channels[channel] -= Convolution.GetConvolution(
                        extendedError.Channels[filter], input.Channels[filter],
                        stride, filters[filter].Bias) * learningRate;
                }

                filters[filter].Bias -= error.Channels[filter].Sum() * learningRate;
            });
        
        return Convolution.GetConvolution(extendedError, 
            FlipFilters(GetFiltersWithoutBiases(originalFilters)), stride);
    }
}