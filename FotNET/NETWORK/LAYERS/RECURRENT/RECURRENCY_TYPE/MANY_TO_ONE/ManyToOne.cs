using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.MANY_TO_ONE;

public class ManyToOne : IRecurrent {
    
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

        return new Tensor(new Matrix(new[] { layer.OutputNeurons[^1] }));
    }
    
    public Tensor BackPropagate(RecurrentLayer layer, Tensor error, double learningRate) {
        var layerError = error.Flatten()[0];
        var nextHidden = (layer.OutputWeights.Transpose() * layerError).GetAsList().ToArray();

        for (var step = layer.HiddenNeurons.Count - 1; step >= 0; step--) {
            var inputGradient = (new Vector(nextHidden) * layerError).Body;
            layer.InputWeights -= new Matrix(inputGradient).Transpose() * learningRate;

            if (nextHidden.Length == 0)
                nextHidden = (layer.OutputWeights.Transpose() * layerError).GetAsList().ToArray();
            else
                nextHidden = (layer.OutputWeights.Transpose() * layerError
                              + new Vector(nextHidden * layer.HiddenWeights.Transpose())
                                  .AsMatrix(1, layer.OutputWeights.Rows, 0)).GetAsList().ToArray();

            nextHidden = layer.Function.Derivation(nextHidden);

            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(new Matrix(layer.HiddenNeurons[step - 1]), new Matrix(nextHidden).Transpose());
                layer.HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < layer.HiddenBias.Length; bias++)
                    layer.HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            }
            
            var outputWeightsGradient = (new Vector(layer.HiddenNeurons[step]) * layerError).Body;
            layer.OutputWeights -= new Matrix(outputWeightsGradient) * learningRate;
            layer.OutputBias -= layerError * learningRate;
        }

        return error;
    }
}