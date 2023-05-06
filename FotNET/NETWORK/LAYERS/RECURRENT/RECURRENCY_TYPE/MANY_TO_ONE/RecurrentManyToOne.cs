using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.MANY_TO_ONE;

/// <summary>
/// Type on RNN where model takes sequence and return one element
/// </summary>
public class RecurrentManyToOne : RecurrentLayer {
    /// <summary>
    /// MTO RNN is RNN model that perform calculations with sequence and returns one element
    /// </summary>
    /// <param name="function"> RNN activation function </param>
    /// <param name="size"> Size of hidden layers </param>
    /// <param name="weightsInitialization"> Initialization type for weights </param>
    public RecurrentManyToOne(Function function, int size, 
        IWeightsInitialization weightsInitialization) {
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
    
    public override Tensor GetNextLayer(Tensor tensor) {
        HiddenNeurons!.Clear();
        OutputNeurons!.Clear();

        InputData = tensor;

        var sequence = tensor.Flatten();
        
        for (var step = 0; step < sequence.Count; step++) {
            var currentElement = sequence[step];
            var inputNeurons = Matrix.Multiply(new Matrix(new[] { currentElement }), InputWeights!);
            
            if (step > 0)
                HiddenNeurons!.Add(inputNeurons 
                                        + Matrix.Multiply(HiddenNeurons[step - 1], HiddenWeights!) 
                                        + HiddenBias!.AsMatrix(1, HiddenBias.Size));
            else
                HiddenNeurons!.Add(inputNeurons);

            HiddenNeurons[^1] = Function!.Activate(HiddenNeurons[^1]);
            OutputNeurons!.Add(Matrix.Multiply(HiddenNeurons[^1], OutputWeights!) + OutputBias);
        }

        return new Tensor(OutputNeurons![^1]);
    }

    public override Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate) {
        var currentError = error.Flatten()[0];
        var nextHidden = new Matrix(0,0);

        var transposedOutputWeights = OutputWeights!.Transpose();
        var transposedHiddenWeights = HiddenWeights!.Transpose();
        
        for (var step = HiddenNeurons!.Count - 1; step >= 0; step--) {
            OutputWeights -= Matrix.Multiply(HiddenNeurons![step].Transpose(),
                new Matrix(new[] { currentError })) * learningRate;
            OutputBias -= currentError * learningRate;
            
            var outputGradient = Matrix.Multiply(new Matrix(new[] { currentError }), transposedOutputWeights);
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
                (new[]{InputData!.Flatten()[step]}), nextHidden) * learningRate;
        }

        return error;
    }
}