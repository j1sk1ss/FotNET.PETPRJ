using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.PERCEPTRON.ADAM.ADAM_PERCEPTRON;

public class AdamPerceptronOptimization : IPerceptronOptimization {
    public Tensor BackPropagate(Tensor error, double learningRate, bool backPropagate, 
        bool isEndLayer, Matrix weights, Vector neurons, Vector bias, double beta1 = 0.9, double beta2 = 0.999, double epsilon = 1e-8) {
        var previousError = new Vector(error.Flatten().ToArray());
        if (isEndLayer) return previousError.AsTensor(1, previousError.Size, 1);

        var neuronsError = previousError * weights.Transpose();
        if (backPropagate) {
            var m = new Matrix(weights.Rows, weights.Columns);
            var v = new Matrix(weights.Rows, weights.Columns);
            var t = 0;

            for (var j = 0; j < weights.Rows; ++j) 
                for (var k = 0; k < weights.Columns; ++k) {
                    var grad = neurons[k] * previousError[j];
                    m.Body[j, k] = beta1 * m.Body[j, k] + (1 - beta1) * grad;
                    v.Body[j, k] = beta2 * v.Body[j, k] + (1 - beta2) * Math.Pow(grad, 2);
                    var mHat = m.Body[j, k] / (1 - Math.Pow(beta1, t + 1));
                    var vHat = v.Body[j, k] / (1 - Math.Pow(beta2, t + 1));
                    weights.Body[j, k] -= learningRate * mHat / (Math.Sqrt(vHat) + epsilon);
                }
            
            var bm = new Vector(bias.Size);
            var bv = new Vector(bias.Size);

            for (var j = 0; j < weights.Rows; j++) {
                bm[j] = beta1 * bm[j] + (1 - beta1) * previousError[j];
                bv[j] = beta2 * bv[j] + (1 - beta2) * Math.Pow(previousError[j], 2);
                var mHat = bm[j] / (1 - Math.Pow(beta1, t + 1));
                var vHat = bv[j] / (1 - Math.Pow(beta2, t + 1));
                bias[j] -= learningRate * mHat / (Math.Sqrt(vHat) + epsilon);
            }
        }

        return neuronsError.AsTensor(1, neuronsError.Size, 1);
    }
}