using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.OneToMany;

public class OneToMany : IRecurrentType {
    public Tensor GetNextLayer(RecurrentLayer layer, Tensor tensor) {
        var currentElement = tensor.Flatten()[0];

        for (var step = 0; step < layer.HiddenNeurons.Count; step++) {
            var inputNeurons = Matrix.Multiply(new Matrix(new[] { currentElement }), layer.InputWeights);
            
            if (step > 0)
                layer.HiddenNeurons.Add(inputNeurons + Matrix.Multiply(layer.HiddenNeurons[step - 1], layer.HiddenWeights) + new Matrix(layer.HiddenBias).Transpose());
            else
                layer.HiddenNeurons.Add(inputNeurons);

            layer.HiddenNeurons[^1] = layer.Function.Activate(layer.HiddenNeurons[^1]);
            layer.OutputNeurons.Add(Matrix.Multiply(layer.HiddenNeurons[^1], layer.OutputWeights) + layer.OutputBias);
        }

        return new Tensor(layer.OutputNeurons);
    }

    public Tensor BackPropagate(RecurrentLayer layer, Tensor error, double learningRate) {
        var sequence = error.Flatten();
        var nextHidden = new Matrix(0,0);

        learningRate /= sequence.Count;
        
        var transposedOutputWeights = layer.OutputWeights.Transpose();
        var transposedHiddenWeights = layer.HiddenWeights.Transpose();
        
        for (var step = layer.HiddenNeurons.Count - 1; step >= 0; step--) {
            layer.OutputWeights -= Matrix.Multiply(layer.HiddenNeurons[step].Transpose(),
                new Matrix(new[] { sequence[step] })) * learningRate;
            layer.OutputBias -= sequence[step] * learningRate;
            
            var outputGradient = Matrix.Multiply(new Matrix(new[] { sequence[step] }), transposedOutputWeights);
            if (step != layer.HiddenNeurons.Count - 1) 
                nextHidden = outputGradient + Matrix.Multiply(nextHidden, transposedHiddenWeights);
            else nextHidden = outputGradient;
            
            nextHidden = layer.Function.Derivation(layer.HiddenNeurons[step]) * nextHidden;
            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(layer.HiddenNeurons[step - 1].Transpose(), nextHidden);
                layer.HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < layer.HiddenBias.Length; bias++)
                    layer.HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            }
            
            layer.InputWeights -= Matrix.Multiply(new Matrix(new[]{layer.InputData.Flatten()[0]}), nextHidden) * learningRate;
        }

        return error;
    }
}