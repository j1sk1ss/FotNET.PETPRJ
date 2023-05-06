using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.MATH.Initialization;
using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.EXTENDED_MANY_TO_MANY;

/// <summary>
/// (Currently don`t work) Type on RNN where model takes sequence and return another sequence with same size with duration during calculation
/// </summary>
public class RecurrentExtendedManyToMany : RecurrentLayer {
    /// <summary>
    /// Extended MTM RNN is RNN model that perform calculations with big sequence and returns same size sequence 
    /// </summary>
    /// <param name="function"> RNN activation function </param>
    /// <param name="size"> Size of hidden layers </param>
    /// <param name="weightsInitialization"> Initialization type for weights </param>
    public RecurrentExtendedManyToMany(Function function, int size, 
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
        
        for (var step = 0; step < sequence.Count * 2; step++) {
            var currentElement = sequence[step >= sequence.Count ? 0 : step];
            var inputNeurons = Matrix.Multiply(new Matrix(new[] { currentElement }), InputWeights!);

            if (step >= sequence.Count) inputNeurons *= 0;
            
            if (step > 0)
                HiddenNeurons!.Add(inputNeurons + Matrix.Multiply(HiddenNeurons[step - 1], HiddenWeights!) + 
                                        HiddenBias!.AsMatrix(1, HiddenBias.Size));
            else
                HiddenNeurons!.Add(inputNeurons);

            HiddenNeurons[^1] = Function!.Activate(HiddenNeurons[^1]);
            if (step >= sequence.Count) OutputNeurons!.Add(Matrix.Multiply(HiddenNeurons[^1], OutputWeights!) + OutputBias);
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
        
        for (var step = HiddenNeurons!.Count - 1; step >= 0; step--) {
            var currentError = sequence[step - sequence.Count < 0 ? 0 : step - sequence.Count];

            OutputWeights -= Matrix.Multiply(HiddenNeurons[step].Transpose(),
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
            
            if (step < InputData!.Flatten().Count)
                InputWeights -= Matrix.Multiply(new Matrix(new[]{InputData.Flatten()[step]}), nextHidden) * learningRate;
        }
        
        return error;
    }
}