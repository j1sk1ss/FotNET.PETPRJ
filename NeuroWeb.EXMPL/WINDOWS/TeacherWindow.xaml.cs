using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

using NeuroWeb.EXMPL.Gui;
using NeuroWeb.EXMPL.LAYERS.CONVOLUTION;
using NeuroWeb.EXMPL.LAYERS.FLATTEN;
using NeuroWeb.EXMPL.LAYERS.INTERFACES;
using NeuroWeb.EXMPL.LAYERS.PERCEPTRON;
using NeuroWeb.EXMPL.LAYERS.POOLING;
using NeuroWeb.EXMPL.OBJECTS.NETWORK;
using NeuroWeb.EXMPL.SCRIPTS;

using Configuration = NeuroWeb.EXMPL.Gui.Configuration;

namespace NeuroWeb.EXMPL.WINDOWS {
    public partial class Teacher {
        public Teacher() => InitializeComponent();
        
        private int _size = 1;
        public void DecreaseStructure(object sender, MouseButtonEventArgs e) {
            if (_size <= 1) return;
            NetworkStructure.Content = Structure.GetStructure(this, --_size);
        }

        public void IncreaseStructure(object sender, MouseButtonEventArgs e) =>
            NetworkStructure.Content = Structure.GetStructure(this, ++_size);
        
        private void SaveStructure(object sender, MouseButtonEventArgs e) =>
            Configuration.WriteConfig(NetworkStructure.Content as Grid, _size + 2);
        
        private void FastTeaching(object sender, MouseButtonEventArgs e) {
            new User().Show();
            Close();
        }
        
        private void HardTeaching(object sender, MouseButtonEventArgs e) {
            var layers = new List<ILayer> {
                new ConvolutionLayer(6, 5, 5, 1, 1, .0001),
                new PoolingLayer(2),
                new ConvolutionLayer(16, 5, 5, 6, 1, .0001),
                new PoolingLayer(2),
                new FlattenLayer(),
                new PerceptronLayer(256, 128, .0001),
                new PerceptronLayer(128, 10, .0001),
                new PerceptronLayer(10, .0001)
            };
            
            Teaching.HardStudying(new Network(layers), 2);
        }
    }
}