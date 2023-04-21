using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.EXTENDED_MANY_TO_MANY;

/// <summary>
/// (Currently don`t work) Type on RNN where model takes sequence and return another sequence with same size with duration during calculation
/// </summary>
public class ExtendedManyToMany : IRecurrentType {
    public Tensor GetNextLayer(RecurrentLayer layer, Tensor tensor) {
        var sequence = tensor.Flatten();
        
        for (var step = 0; step < sequence.Count * 2; step++) {
            var currentElement = sequence[step >= sequence.Count ? 0 : step];
            var inputNeurons = Matrix.Multiply(new Matrix(new[] { currentElement }), layer.InputWeights);

            if (step >= sequence.Count) inputNeurons *= 0;
            
            if (step > 0)
                layer.HiddenNeurons.Add(inputNeurons + Matrix.Multiply(layer.HiddenNeurons[step - 1], layer.HiddenWeights) + new Matrix(layer.HiddenBias).Transpose());
            else
                layer.HiddenNeurons.Add(inputNeurons);

            layer.HiddenNeurons[^1] = layer.Function.Activate(layer.HiddenNeurons[^1]);
            if (step >= sequence.Count) layer.OutputNeurons.Add(Matrix.Multiply(layer.HiddenNeurons[^1], layer.OutputWeights) + layer.OutputBias);
        }

        return new Vector(new Tensor(layer.OutputNeurons).Flatten().ToArray())
            .AsTensor(1, new Tensor(layer.OutputNeurons).Flatten().Count, 1);
    }

    public Tensor BackPropagate(RecurrentLayer layer, Tensor error, double learningRate) {
        var sequence = error.Flatten();
        var nextHidden = new Matrix(0,0);

        learningRate /= sequence.Count;

        var transposedOutputWeights = layer.OutputWeights.Transpose();
        var transposedHiddenWeights = layer.HiddenWeights.Transpose();
        
        for (var step = layer.HiddenNeurons.Count - 1; step >= 0; step--) {
            var currentError = sequence[step - sequence.Count < 0 ? 0 : step - sequence.Count];

            layer.OutputWeights -= Matrix.Multiply(layer.HiddenNeurons[step].Transpose(),
                new Matrix(new[] { currentError })) * learningRate;
            layer.OutputBias -= currentError * learningRate;
            
            var outputGradient = Matrix.Multiply(new Matrix(new[] { currentError }), transposedOutputWeights);
            if (step != layer.HiddenNeurons.Count - 1) 
                nextHidden = outputGradient + Matrix.Multiply(nextHidden, transposedHiddenWeights);
            else nextHidden = outputGradient;
            
            nextHidden = layer.Function.Derivation(layer.HiddenNeurons[step], layer.HiddenNeurons[step]) * nextHidden;
            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(layer.HiddenNeurons[step - 1].Transpose(), nextHidden);
                layer.HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < layer.HiddenBias.Length; bias++)
                    layer.HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            }
            
            if (step < layer.InputData.Flatten().Count)
                layer.InputWeights -= Matrix.Multiply(new Matrix(new[]{layer.InputData.Flatten()[step]}), nextHidden) * learningRate;
        }
        
        return error;
    }
}