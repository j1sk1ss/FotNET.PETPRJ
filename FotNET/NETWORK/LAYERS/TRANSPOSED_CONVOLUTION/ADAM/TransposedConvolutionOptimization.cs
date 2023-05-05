using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.ADAM;

public abstract class TransposedConvolutionOptimization {
    protected static Filter[] FlipFilters(Filter[] filters) {
        for (var i = 0; i < filters.Length; i++)
            filters[i] = filters[i].Flip().AsFilter();

        return filters;
    }

    protected static Filter[] GetFiltersWithoutBiases(Filter[] filters) {
        for (var i = 0; i < filters.Length; i++)
            filters[i] = new Filter(filters[i].Channels);

        return filters;
    }
    
    public abstract Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input, Filter[] filters,
        int stride, double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8);
}