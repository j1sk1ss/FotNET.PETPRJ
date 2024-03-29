using FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.ADAM;
using FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION.SCRIPTS;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.TRANSPOSED_CONVOLUTION;

public class TransposedConvolutionLayer : ILayer {
    /// <summary> Layer that perform tensor deconvolution by filters and biases. </summary>
    /// <param name="filters"> Count of filters on layer. </param>
    /// <param name="filterShape"> Shape of filters </param>
    /// <param name="weightsInitialization"> Type of weights initialization of filters on layer. </param>
    /// <param name="stride"> Stride of deconvolution. </param>
    /// <param name="transposedConvolutionOptimization"> Optimization type </param>
    public TransposedConvolutionLayer(int filters, (int Weight, int Height, int Depth) filterShape,
        IWeightsInitialization weightsInitialization, int stride, TransposedConvolutionOptimization transposedConvolutionOptimization) {
        TransposedConvolutionOptimization = transposedConvolutionOptimization;
        Filters = new Filter[filters];
            
        for (var j = 0; j < filters; j++) {
            Filters[j] = new Filter(new List<Matrix>()) {
                Bias = .001d
            };

            for (var i = 0; i < filterShape.Depth; i++)
                Filters[j].Channels.Add(new Matrix(
                    new double[filterShape.Weight, filterShape.Height]));
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
    private TransposedConvolutionOptimization TransposedConvolutionOptimization { get; }

    public Tensor GetNextLayer(Tensor tensor) {
        Input = tensor.Copy();
        return TransposedConvolution.GetTransposedConvolution(tensor, Filters, _stride);
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
        TransposedConvolutionOptimization.BackPropagate(error, learningRate, backPropagate, Input, Filters,
            _stride);

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