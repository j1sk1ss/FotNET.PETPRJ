using FotNET.NETWORK.OBJECTS.DATA_OBJECTS;

namespace FotNET.NETWORK.FIT;

public static class Fit {
    public static Network FitModel(Network network, List<IData> dataSet, int epochs, double baseLearningRate) {
        for (var epoch = 0; epoch < epochs; epoch++)
            foreach (var data in from data in dataSet let prediction = 
                         network.ForwardFeed(data.AsTensor()) where prediction != 
                                                                    data.GetRight().GetMaxIndex() select data) {
                var learningRate = baseLearningRate * Math.Pow(.1, epoch);
                network.BackPropagation(data.GetRight().GetMaxIndex(), learningRate);
            }

        return network;
    }

    public static double TestModel(Network network, List<IData> dataSet) =>
        dataSet.Count / (double)(from data in dataSet let prediction = 
            network.ForwardFeed(data.AsTensor()) where prediction == 
                                                       data.GetRight().GetMaxIndex() select data).Count();
    
}