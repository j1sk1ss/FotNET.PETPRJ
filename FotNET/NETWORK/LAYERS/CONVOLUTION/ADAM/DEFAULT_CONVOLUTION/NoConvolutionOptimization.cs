using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.SAME;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.ADAM.DEFAULT_CONVOLUTION;

public class NoConvolutionOptimization : ConvolutionOptimization {
    public override Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input,
        Filter[] filters, bool update, int stride) {
        var inputTensor = input;
        var extendedInput = inputTensor.GetSameChannels(error);

        var originalFilters = new Filter[filters.Length];
        for (var i = 0; i < filters.Length; i++)
            originalFilters[i] = new Filter(new List<Matrix>(filters[i].Channels));
            
        for (var i = 0; i < originalFilters.Length; i++)
            originalFilters[i] = originalFilters[i].GetSameChannels(error).AsFilter();

        if (update && backPropagate)
            Parallel.For(0, filters.Length, filter => {
                for (var channel = 0; channel < filters[filter].Shape.Depth; channel++) 
                    filters[filter].Channels[channel] -= Convolution.GetConvolution(
                        extendedInput.Channels[filter],error.Channels[filter],
                        stride, filters[filter].Bias) * learningRate;
                    
                filters[filter].Bias -= error.Channels[filter].Sum() * learningRate;
            });

        return Convolution.GetConvolution(new SamePadding(originalFilters[0]).GetPadding(error), 
            FlipFilters(GetFiltersWithoutBiases(originalFilters)), stride);
    }
}