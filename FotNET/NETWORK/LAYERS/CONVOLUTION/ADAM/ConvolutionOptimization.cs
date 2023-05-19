using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.ADAM;

public abstract class ConvolutionOptimization {
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
        bool update, int stride);
}