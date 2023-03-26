using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT;

public class RecurrentLayer : ILayer {

    public RecurrentLayer(Function function, RecurrencyType recurrentType, int size) {
        InputWeights  = new Matrix(1, size);
        HiddenWeights = new Matrix(size, size);
        OutputWeights = new Matrix(size, 1);

        InputWeights.HeInitialization();
        HiddenWeights.HeInitialization();
        OutputWeights.HeInitialization();
        
        HiddenNeurons = new List<double[]>();
        OutputNeurons = new List<double>();
        
        HiddenBias    = new double[size];
        HiddenBias[0] = .01d;
        OutputBias    = .01d;
        
        Function      = function;
        RecurrentType = recurrentType;
    }

    private Matrix InputWeights { get; set; }

    private Matrix HiddenWeights { get; set; }
    
    private Matrix OutputWeights { get; set; }
    
    private double[] HiddenBias { get; }
    
    private double OutputBias { get; set; }
    

    private Function Function { get; }
    private RecurrencyType RecurrentType { get; }


    private List<double[]> HiddenNeurons { get; }
    private List<double> OutputNeurons { get; }

    
    public Tensor GetNextLayer(Tensor tensor) {
        var sequence = tensor.Flatten();
        for (var step = 0; step < sequence.Count; step++) {
            var currentElement = sequence[step];
            var inputNeurons = (InputWeights * currentElement).GetAsList().ToArray();
            
            if (step > 0)
                HiddenNeurons.Add(new Vector(new Vector(inputNeurons) 
                                                  + new Vector(HiddenNeurons[step - 1] 
                                                               * HiddenWeights)) + new Vector(HiddenBias));
            else
                HiddenNeurons.Add(inputNeurons);

            HiddenNeurons[^1] = Function.Activate(HiddenNeurons[^1]);
            OutputNeurons.Add(
                (new Vector(HiddenNeurons[^1] * OutputWeights) + OutputBias).Body[0]);
        }
        
        return RecurrentType == RecurrencyType.ManyToMany 
            ? new Tensor(new Matrix(OutputNeurons.ToArray()))
                : new Tensor(new Matrix(new[] { OutputNeurons[^1] }));
    }
    
    public Tensor BackPropagate(Tensor error, double learningRate) {
        var sequence = error.Flatten();
        var nextHidden = (OutputWeights.Transpose() * sequence[^1]).GetAsList().ToArray();

        learningRate /= sequence.Count;
        
        for (var step = sequence.Count - 1; step >= 0; step--) {
            var currentError = RecurrentType == RecurrencyType.ManyToMany ? sequence[step] : sequence[0];
            var inputGradient = (new Vector(nextHidden) * currentError).Body;
            InputWeights  -= new Matrix(inputGradient).Transpose() * learningRate;
            
            if (nextHidden.Length == 0)
                nextHidden = (OutputWeights.Transpose() * currentError).GetAsList().ToArray();
            else {
                var position = 0;
                nextHidden = (OutputWeights.Transpose() * currentError
                              + new Vector(nextHidden * HiddenWeights.Transpose())
                                  .AsMatrix(1, OutputWeights.Rows, ref position)).GetAsList().ToArray();
            }

            nextHidden = Function.Derivation(nextHidden);

            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(new Matrix(HiddenNeurons[step - 1]), new Matrix(nextHidden).Transpose());
                HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < HiddenBias.Length; bias++)
                    HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            }
            
            var outputWeightsGradient = (new Vector(HiddenNeurons[step]) * currentError).Body;
            OutputWeights -= new Matrix(outputWeightsGradient) * learningRate;
            OutputBias -= currentError * learningRate;
        }

        return error;
    }

    public Tensor GetValues() => null!;

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