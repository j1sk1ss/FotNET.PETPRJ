using System.Windows;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL.WINDOWS {
    public partial class Teacher {
        public Teacher() {
            MessageBox.Show("Hard studying started...");
            
            const string config = @"C:\Users\j1sk1ss\RiderProjects\NeuroWeb.EXMPL\NeuroWeb.EXMPL\DATA\Config.txt";
            var networkConfiguration = DataWorker.ReadNetworkConfig(config);
            var network = new Network(networkConfiguration);

            Teaching.HardStudying(network);
            MessageBox.Show("Hard studying ended\nCheck Weights.txt file:");
        }
    }
}