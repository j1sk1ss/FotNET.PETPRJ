using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.ADAM.ADAM_TRANSPOSED_CONVOLUTION;

/// <summary>
/// Transposed convolution with Adam optimization
/// </summary>
public class AdamTransposedConvolutionOptimization : TransposedConvolutionOptimization {
    private int _iteration;
    
    public override Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input, Filter[] filters, int stride,
        double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8) {

        var inputTensor = input;
        var extendedError = error.GetSameChannels(inputTensor);

        var originalFilters = new Filter[filters.Length];
        for (var i = 0; i < filters.Length; i++)
            originalFilters[i] = new Filter(new List<Matrix>(filters[i].Channels));

        for (var i = 0; i < originalFilters.Length; i++)
            originalFilters[i] = originalFilters[i].GetSameChannels(error).AsFilter();

        if (backPropagate) {
            var momentum = new Filter[filters.Length];
            var velocity = new Filter[filters.Length];

            for (var i = 0; i < filters.Length; i++) {
                momentum[i] = new Filter(filters[0].Channels[0].Rows, filters[0].Channels[0].Columns, filters[0].Channels.Count);
                velocity[i] = new Filter(filters[0].Channels[0].Rows, filters[0].Channels[0].Columns, filters[0].Channels.Count);
            }

            Parallel.For(0, filters.Length, filter => {
                for (var channel = 0; channel < filters[filter].Channels.Count; channel++) {
                    var current = _iteration++ + 1;
                    var grad = Convolution.GetConvolution(extendedError.Channels[filter], 
                        input.Channels[filter], stride, filters[filter].Bias);
                    
                    momentum[filter].Channels[channel] = momentum[filter].Channels[channel] * beta1 + grad * (1 - beta1);
                    velocity[filter].Channels[channel] = velocity[filter].Channels[channel] * beta2 + grad * grad * (1 - beta2);
                    
                    var momentumHat = momentum[filter].Channels[channel] / (1 - Math.Pow(beta1, current));
                    var velocityHat = velocity[filter].Channels[channel] / (1 - Math.Pow(beta2, current));
                    
                    filters[filter].Channels[channel] -= momentumHat * learningRate / (velocityHat.Sqrt() + epsilon);
                }

                filters[filter].Bias -= error.Channels[filter].Sum() * learningRate;
            });
        }

        return Convolution.GetConvolution(extendedError, 
            FlipFilters(GetFiltersWithoutBiases(originalFilters)), stride);
    }
}