using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT;

public class RecurrentLayer : ILayer {
    /// <summary> Recurrent neural network. </summary>
    /// <param name="function"> Type of activation function. </param>
    /// <param name="recurrentType"> Type of recurrent neural network. </param>
    /// <param name="size"> Hidden neurons size. </param>
    /// <param name="weightsInitialization"> Type of weights initialization of filters on layer. </param>
    public RecurrentLayer(Function function, IRecurrentType recurrentType, int size, 
        IWeightsInitialization weightsInitialization) {
        InputWeights  = weightsInitialization.Initialize(new Matrix(1, size));
        HiddenWeights = weightsInitialization.Initialize(new Matrix(size, size));
        OutputWeights = weightsInitialization.Initialize(new Matrix(size, 1));

        HiddenNeurons = new List<Matrix>();
        OutputNeurons = new List<Matrix>();
        
        HiddenBias    = new double[size];
        HiddenBias[0] = .01d;
        OutputBias    = .01d;
        
        Function      = function;
        RecurrentType = recurrentType;
        
        InputData = new Tensor(new List<Matrix>());
    }

    public Matrix InputWeights { get; set; }

    public Matrix HiddenWeights { get; set; }
    
    public Matrix OutputWeights { get; set; }
    
    public double[] HiddenBias { get; }
    
    public double OutputBias { get; set; }
    

    public Function Function { get; }
    private IRecurrentType RecurrentType { get; }


    public List<Matrix> HiddenNeurons { get; }
    public List<Matrix> OutputNeurons { get; }


    public Tensor InputData { get; private set; }
    
    public Tensor GetNextLayer(Tensor tensor) {
        HiddenNeurons.Clear();
        OutputNeurons.Clear();

        InputData = tensor;
        return RecurrentType.GetNextLayer(this, tensor);
    }

    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) =>
        RecurrentType.BackPropagate(this, error, learningRate);

    public Tensor GetValues() => InputData;

    public string GetData() => InputWeights.GetValues() + HiddenWeights.GetValues() + OutputWeights.GetValues() 
                               + new Vector(HiddenBias).Print() + " " + OutputBias;
    
    public string LoadData(string data) {
        var position = 0;
        var dataNumbers = data.Split(" ",  StringSplitOptions.RemoveEmptyEntries);
        
        for (var x = 0; x < InputWeights.Rows; x++)
            for (var y = 0; y < InputWeights.Columns; y++)
                InputWeights.Body[x, y] = double.Parse(dataNumbers[position++]);
        
        for (var x = 0; x < HiddenWeights.Rows; x++)
            for (var y = 0; y < HiddenWeights.Columns; y++)
                HiddenWeights.Body[x, y] = double.Parse(dataNumbers[position++]);
        
        for (var x = 0; x < HiddenWeights.Rows; x++)
            for (var y = 0; y < HiddenWeights.Columns; y++)
                HiddenWeights.Body[x, y] = double.Parse(dataNumbers[position++]);

        for (var i = 0; i < HiddenBias.Length; i++)
            HiddenBias[i] = double.Parse(dataNumbers[position++]);

        OutputBias = double.Parse(dataNumbers[position++]);
        
        return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
    }
}