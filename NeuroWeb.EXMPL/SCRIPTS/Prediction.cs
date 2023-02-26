using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Prediction {
        public static int Predict(Network network, string number) {
            var dataInformation = DataWorker.ReadData(number, network.Configuration);
            network.InsertInformation(dataInformation);
            return (int)network.ForwardFeed();
        }
    }
}