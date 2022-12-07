using System.Windows;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS
{
    public class Prediction
    {
        public int Predict(string number) {
            const string numberPath = 
                @"C:\\Users\\j1sk1ss\\RiderProjects\\NeuroWeb.EXMPL\\NeuroWeb.EXMPL\\DATA\\References.txt";
            var data = DataWorker.ReadNetworkConfig(@"C:\\Users\\j1sk1ss\\RiderProjects\\NeuroWeb.EXMPL\\" +
                                                    @"NeuroWeb.EXMPL\\DATA\\Config.txt");
            var network = new Network(data);

            MessageBox.Show($"{network.GetConfiguration()}");

            var count = 1;
            var dataInformation = DataWorker.ReadData(numberPath, ref data, ref count);
            
            network.InsertInformation(dataInformation[0].Pixels);
            var prediction = network.ForwardFeed();
            
            return (int)prediction;
        }
    }
}