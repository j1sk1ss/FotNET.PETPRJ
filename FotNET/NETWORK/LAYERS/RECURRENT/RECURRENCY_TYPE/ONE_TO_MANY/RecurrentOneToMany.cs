using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ONE_TO_MANY;

/// <summary>
/// Type on RNN where model takes element and return sequence with size of hidden layer
/// </summary>
public class RecurrentOneToMany : RecurrentLayer {
    /// <summary>
    /// OTM RNN is RNN model that perform calculations with one element and returns sequence with size that like size of hidden
    /// </summary>
    /// <param name="function"> RNN activation function </param>
    /// <param name="size"> Size of hidden layers </param>
    /// <param name="outPutSize"> Size of output sequense </param>
    /// <param name="weightsInitialization"> Initialization type for weights </param>
    public RecurrentOneToMany(Function function, int size, int outPutSize, 
        IWeightsInitialization weightsInitialization) {
        OutPutSize = outPutSize;
        
        InputWeights  = weightsInitialization.Initialize(new Matrix(1, size));
        HiddenWeights = weightsInitialization.Initialize(new Matrix(size, size));
        OutputWeights = weightsInitialization.Initialize(new Matrix(size, 1));

        HiddenNeurons = new List<Matrix>();
        OutputNeurons = new List<Matrix>();
        
        HiddenBias    = new Vector(size) {
            [0] = .01d
        };
        
        OutputBias    = .01d;
        Function      = function;
        
        InputData = new Tensor(new List<Matrix>());
    }
    
    private int OutPutSize { get; }
    
    public override Tensor GetNextLayer(Tensor tensor) {
        HiddenNeurons!.Clear();
        OutputNeurons!.Clear();

        InputData = tensor;
        
        var currentElement = tensor.Flatten()[0];

        for (var step = 0; step < OutPutSize; step++) {
            if (step > 0)
                HiddenNeurons!.Add(Matrix.Multiply(HiddenNeurons[step - 1],
                    HiddenWeights!) + HiddenBias!.AsMatrix(1, HiddenBias.Size));
            else
                HiddenNeurons!.Add(Matrix.Multiply(new Matrix(new[] { currentElement }), InputWeights!));

            HiddenNeurons[^1] = Function!.Activate(HiddenNeurons[^1]);
            OutputNeurons!.Add(Matrix.Multiply(HiddenNeurons[^1], OutputWeights!) + OutputBias);
        }

        return new Vector(new Tensor(OutputNeurons!).Flatten().ToArray())
            .AsTensor(1, new Tensor(OutputNeurons!).Flatten().Count, 1);
    }

    public override Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        var sequence = error.Flatten();
        var nextHidden = new Matrix(0,0);

        learningRate /= sequence.Count;
        
        var transposedOutputWeights = OutputWeights!.Transpose();
        var transposedHiddenWeights = HiddenWeights!.Transpose();
        
        for (var step = sequence.Count - 1; step >= 0; step--) {
            OutputWeights -= Matrix.Multiply(HiddenNeurons![step].Transpose(),
                new Matrix(new[] { sequence[step] })) * learningRate;
            OutputBias -= sequence[step] * learningRate;
            
            var outputGradient = Matrix.Multiply(new Matrix(new[] { sequence[step] }), transposedOutputWeights);
            if (step != HiddenNeurons.Count - 1) 
                nextHidden = outputGradient + Matrix.Multiply(nextHidden, transposedHiddenWeights);
            else nextHidden = outputGradient;
            
            nextHidden = Function!.Derivation(HiddenNeurons[step], HiddenNeurons[step]) * nextHidden;
            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(HiddenNeurons[step - 1].Transpose(), nextHidden);
                HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < HiddenBias!.Size; bias++)
                    HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            } 
            else InputWeights -= Matrix.Multiply(new Matrix
                (new[]{InputData!.Flatten()[0]}), nextHidden) * learningRate;
        }

        return error;
    }
}