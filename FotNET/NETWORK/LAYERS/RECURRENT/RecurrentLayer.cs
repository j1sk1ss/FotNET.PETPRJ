using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT;

public abstract class RecurrentLayer : ILayer {
    protected Matrix? InputWeights { get; set; }

    protected Matrix? HiddenWeights { get; set; }
    
    protected Matrix? OutputWeights { get; set; }
    
    protected Vector? HiddenBias { get; set; }
    
    protected double OutputBias { get; set; }
    
    protected Function? Function { get; set; }
    
    protected List<Matrix>? HiddenNeurons { get; set; }
    
    protected List<Matrix>? OutputNeurons { get; set; }
    
    protected Tensor? InputData { get; set; }

    public abstract Tensor GetNextLayer(Tensor tensor);

    public abstract Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate);

    public Tensor GetValues() => InputData!;

    public string GetData() => InputWeights!.GetValues() + HiddenWeights!.GetValues() + OutputWeights!.GetValues() 
                               + HiddenBias!.Print() + " " + OutputBias;
    
    public string LoadData(string data) {
        var position = 0;
        var dataNumbers = data.Split(" ",  StringSplitOptions.RemoveEmptyEntries);
        
        for (var x = 0; x < InputWeights!.Rows; x++)
            for (var y = 0; y < InputWeights.Columns; y++)
                InputWeights.Body[x, y] = double.Parse(dataNumbers[position++]);
        
        for (var x = 0; x < HiddenWeights!.Rows; x++)
            for (var y = 0; y < HiddenWeights.Columns; y++)
                HiddenWeights.Body[x, y] = double.Parse(dataNumbers[position++]);
        
        for (var x = 0; x < HiddenWeights.Rows; x++)
            for (var y = 0; y < HiddenWeights.Columns; y++)
                HiddenWeights.Body[x, y] = double.Parse(dataNumbers[position++]);

        for (var i = 0; i < HiddenBias!.Size; i++)
            HiddenBias[i] = double.Parse(dataNumbers[position++]);

        OutputBias = double.Parse(dataNumbers[position++]);
        
        return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
    }
}