using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM.DEFAULT_PERCEPTRON;

public class NoPerceptronOptimization : IPerceptronOptimization {
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, 
        bool isEndLayer, Matrix weights, Vector neurons, Vector bias, double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8) {
        var previousError = new Vector(error.Flatten().ToArray());
        if (isEndLayer) return previousError.AsTensor(1, previousError.Size, 1);

        var neuronsError = previousError * weights.Transpose();
        if (backPropagate) {
            for (var j = 0; j < weights.Rows; ++j)
                for (var k = 0; k < weights.Columns; ++k)
                    weights.Body[j, k] -= neurons[k] * previousError[j] * learningRate;
    
            for (var j = 0; j < weights.Rows; j++)
                bias[j] -= previousError[j] * learningRate;
        }
            
        return neuronsError.AsTensor(1, neuronsError.Size, 1);
    }
}