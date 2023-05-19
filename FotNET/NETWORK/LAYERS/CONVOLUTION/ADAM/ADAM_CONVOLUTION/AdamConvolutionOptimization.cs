using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS.PADDING.SAME;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.CONVOLUTION.ADAM.ADAM_CONVOLUTION;

public class AdamConvolutionOptimization : ConvolutionOptimization {

    /// <summary>
    /// Adam optimization for CNN layer
    /// </summary>
    /// <param name="optimizationParams"> Parameters for Adam optimization </param>
    public AdamConvolutionOptimization((double firstBeta, double secondBeta, double epsilon) optimizationParams) =>
        OptimizationParams = optimizationParams;
    
    private (double firstBeta, double secondBeta, double epsilon) OptimizationParams { get; }
    
    private int _iteration;
    
    public override Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, Tensor input, 
        Filter[] filters, bool update, int stride) {
        var extendedInput = input.GetSameChannels(error);

        var originalFilters = new Filter[filters.Length];
        for (var i = 0; i < filters.Length; i++)
            originalFilters[i] = new Filter(new List<Matrix>(filters[i].Channels));

        for (var i = 0; i < originalFilters.Length; i++)
            originalFilters[i] = originalFilters[i].GetSameChannels(error).AsFilter();

        if (update && backPropagate) {
            var momentum = new Filter[filters.Length];
            var velocity = new Filter[filters.Length];

            for (var i = 0; i < filters.Length; i++) {
                momentum[i] = new Filter(filters[0].Channels[0].Rows, filters[0].Channels[0].Columns, filters[0].Channels.Count);
                velocity[i] = new Filter(filters[0].Channels[0].Rows, filters[0].Channels[0].Columns, filters[0].Channels.Count);
            }
            
            Parallel.For(0, filters.Length, filter => {
                var current = _iteration++ + 1;
                for (var channel = 0; channel < filters[filter].Shape.Depth; channel++) {
                    var grad = Convolution.GetConvolution(extendedInput.Channels[filter],
                        error.Channels[filter], stride, filters[filter].Bias);
                    
                    momentum[filter].Channels[channel] = momentum[filter].Channels[channel] 
                        * OptimizationParams.firstBeta + grad * (1 - OptimizationParams.firstBeta);
                    velocity[filter].Channels[channel] = velocity[filter].Channels[channel] 
                        * OptimizationParams.secondBeta + grad * grad * (1 - OptimizationParams.secondBeta);
                    
                    var momentumHat = momentum[filter].Channels[channel] / (1 - Math.Pow(OptimizationParams.firstBeta, current));
                    var velocityHat = velocity[filter].Channels[channel] / (1 - Math.Pow(OptimizationParams.secondBeta, current));
                    
                    filters[filter].Channels[channel] -= momentumHat * learningRate / (velocityHat.Sqrt() + OptimizationParams.epsilon);
                }

                filters[filter].Bias -= error.Channels[filter].Sum() * learningRate;
            });
        }

        return Convolution.GetConvolution(new SamePadding(originalFilters[0]).GetPadding(error), 
            FlipFilters(GetFiltersWithoutBiases(originalFilters)), stride);
    }
}