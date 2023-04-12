using FotNET.NETWORK.LAYERS.CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.LAYERS.DECONVOLUTION.SCRIPTS;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.DECONVOLUTION;

public class DeconvolutionLayer : ILayer {
    public DeconvolutionLayer(int filters, int filterWeight, int filterHeight, int filterDepth,
        IWeightsInitialization weightsInitialization, int stride) {
        Filters = new Filter[filters];
            
        for (var j = 0; j < filters; j++) {
            Filters[j] = new Filter(new List<Matrix>()) {
                Bias = .001d
            };

            for (var i = 0; i < filterDepth; i++)
                Filters[j].Channels.Add(new Matrix(
                    new double[filterWeight, filterHeight]));
        }

        foreach (var filter in Filters)
            for (var i = 0; i < filter.Channels.Count; i++)
                filter.Channels[i] = weightsInitialization.Initialize(filter.Channels[i]);

        _stride = stride;
        Input   = new Tensor(new Matrix(0, 0));
    }
    
    private readonly int _stride;
    
    private Filter[] Filters { get; }
    private Tensor Input { get; set; }
    
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
    
    public Tensor GetNextLayer(Tensor tensor) {
        Input = new Tensor(new List<Matrix>(tensor.Channels));
        return Deconvolution.GetDeconvolution(tensor, Filters, _stride);
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        var inputTensor = Input;
        var extendedError = error.GetSameChannels(inputTensor);
        
        var originalFilters = new Filter[Filters.Length];
        for (var i = 0; i < Filters.Length; i++)
            originalFilters[i] = new Filter(new List<Matrix>(Filters[i].Channels));
            
        for (var i = 0; i < originalFilters.Length; i++)
            originalFilters[i] = originalFilters[i].GetSameChannels(inputTensor).AsFilter();
        
        if (backPropagate)
            Parallel.For(0, Filters.Length, filter => {
                for (var channel = 0; channel < Filters[filter].Channels.Count; channel++) 
                    Filters[filter].Channels[channel] -= Deconvolution.GetDeconvolution(inputTensor.Channels[filter],
                        extendedError.Channels[filter], _stride, Filters[filter].Bias) * learningRate;
                
                Filters[filter].Bias -= extendedError.Channels[filter].Sum() * learningRate;
            });
        
        return Convolution.GetConvolution(extendedError, FlipFilters(GetFiltersWithoutBiases(originalFilters)), _stride);
    }

    public Tensor GetValues() => Input;

    public string GetData() {
        var temp = "";
            
        foreach (var filter in Filters) {
            temp = filter.Channels.Aggregate(temp, (current, channel) => current + channel.GetValues());
            temp += filter.Bias + " ";
        }
            
        return temp;
    }

    public string LoadData(string data) {
        var position = 0;
        var dataNumbers = data.Split(" ",  StringSplitOptions.RemoveEmptyEntries);

        foreach (var filter in Filters) {
            foreach (var channel in filter.Channels)
                for (var x = 0; x < channel.Rows; x++)
                for (var y = 0; y < channel.Columns; y++)
                    channel.Body[x, y] = double.Parse(dataNumbers[position++]);

            filter.Bias = double.Parse(dataNumbers[position++]);
        }

        return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
    }
}