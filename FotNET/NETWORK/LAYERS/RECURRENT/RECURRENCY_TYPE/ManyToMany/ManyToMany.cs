using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.ManyToMany;

public class ManyToMany : IRecurrentType {
    public Tensor GetNextLayer(RecurrentLayer layer, Tensor tensor) {
        var sequence = tensor.Flatten();
        for (var step = 0; step < sequence.Count; step++) {
            var currentElement = sequence[step];
            var inputNeurons = (layer.InputWeights * currentElement).GetAsList().ToArray();
            
            if (step > 0)
                layer.HiddenNeurons.Add(new Vector(new Vector(inputNeurons) 
                                                  + new Vector(layer.HiddenNeurons[step - 1] 
                                                               * layer.HiddenWeights)) + new Vector(layer.HiddenBias));
            else
                layer.HiddenNeurons.Add(inputNeurons);

            layer.HiddenNeurons[^1] = layer.Function.Activate(layer.HiddenNeurons[^1]);
            layer.OutputNeurons.Add(
                (new Vector(layer.HiddenNeurons[^1] * layer.OutputWeights) + layer.OutputBias).Body[0]);
        }

        return new Tensor(new Matrix(layer.OutputNeurons.ToArray()));
    }

    public Tensor BackPropagate(RecurrentLayer layer, Tensor error, double learningRate) {
        var sequence = error.Flatten();
        var nextHidden = new Matrix(0,0);

        learningRate /= sequence.Count;
        
        for (var step = layer.HiddenNeurons.Count - 1; step >= 0; step--) {
            layer.OutputWeights -= Matrix.Multiply(new Matrix(layer.HiddenNeurons[step]),
                new Matrix(new[] { sequence[step] })) * learningRate;
            layer.OutputBias -= sequence[step] * learningRate;
            
            var outputGradient = Matrix.Multiply(new Matrix(new[] { sequence[step] }), layer.OutputWeights.Transpose());
            if (step != layer.HiddenNeurons.Count - 1) 
                nextHidden = outputGradient + Matrix.Multiply(nextHidden, layer.HiddenWeights.Transpose());
            else nextHidden = outputGradient;
            
            nextHidden = new Matrix(layer.Function.Derivation(layer.HiddenNeurons[step])).Transpose() * nextHidden;
            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(new Matrix(layer.HiddenNeurons[step - 1]), nextHidden);
                layer.HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < layer.HiddenBias.Length; bias++)
                    layer.HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            }
            
            layer.InputWeights -= Matrix.Multiply(new Matrix(new[]{layer.InputData.Flatten()[step]}), nextHidden) * learningRate;
        }

        return error;
    }
}