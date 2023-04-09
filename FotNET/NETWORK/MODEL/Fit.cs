using FotNET.DATA.DATA_OBJECTS;
using FotNET.NETWORK.MATH.LOSS_FUNCTION;

namespace FotNET.NETWORK.MODEL;

public static class Fit {
    public static Network FitModel(Network network, List<IData> dataSet, int epochs, LossFunction lossFunction, double baseLearningRate) {
        for (var epoch = 0; epoch < epochs; epoch++)
            foreach (var datum in dataSet) {
                var predictedClass = network.ForwardFeed(datum.AsTensor(), AnswerType.Class);
                var predictedValue = network.GetLayers()[^1].GetValues().Flatten()[(int)predictedClass];

                var expectedClass = datum.GetRight().GetMaxIndex();
                var expectedValue = datum.GetRight().Flatten()[datum.GetRight().GetMaxIndex()];

                if (Math.Abs(predictedClass - expectedClass) > .01) {
                    network.BackPropagation(expectedClass, expectedValue,
                        lossFunction, baseLearningRate * Math.Pow(.1, epoch));
                    continue;
                }
                
                if (Math.Abs(predictedValue - expectedValue) > .1)
                    network.BackPropagation(expectedClass, expectedValue,
                        lossFunction, baseLearningRate * Math.Pow(.1, epoch));
            }

        return network;
    }
}