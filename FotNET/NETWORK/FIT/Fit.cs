using FotNET.NETWORK.OBJECTS.DATA_OBJECTS;

namespace FotNET.NETWORK.FIT;

public static class Fit {
    public static Network FitModel(Network network, List<IData> dataSet, int epochs) {
        for (var epoch = 0; epoch < epochs; epoch++)
            foreach (var data in from data in dataSet let prediction = 
                         network.ForwardFeed(data.AsTensor()) where prediction != 
                                                                    data.GetRight().GetMaxIndex() select data) {
                network.BackPropagation(data.GetRight().GetMaxIndex());
            }

        return network;
    }

    public static double TestModel(Network network, List<IData> dataSet) =>
        dataSet.Count / (double)(from data in dataSet let prediction = 
            network.ForwardFeed(data.AsTensor()) where prediction == 
                                                       data.GetRight().GetMaxIndex() select data).Count();
    
}