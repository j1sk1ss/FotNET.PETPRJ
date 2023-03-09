using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using Microsoft.Win32;

using NeuroWeb.EXMPL.LAYERS.INTERFACES;
using NeuroWeb.EXMPL.SCRIPTS.ACTIVATION.ReLU;
using NeuroWeb.EXMPL.SCRIPTS.MATH;
using Vector = NeuroWeb.EXMPL.OBJECTS.MATH.Vector;

namespace NeuroWeb.EXMPL.OBJECTS.NETWORK {
    public class Network {
        public Network(List<ILayer> layers) {
            Layers = layers;
        }
        
        public List<ILayer> Layers { get; }
        
        public int ForwardFeed(Tensor data) {
            try {
                data = Layers.Aggregate(data, (current, layer) => layer.GetNextLayer(current));
                return Vector.GetMaxIndex(data.Flatten());
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        public void BackPropagation(double expectedAnswer) {
            try {
                var errorTensor = LossFunction.GetLoss(Layers[^1].GetValues(), (int)expectedAnswer, new LeakyReLu());
                for (var i = Layers.Count - 2; i >= 0; i--) {
                    errorTensor = Layers[i].BackPropagate(errorTensor);
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        private static string _weights;
        private static string GetWeights() {

            var file = new OpenFileDialog {
                Filter = "TXT files | *.txt"
            };
            var message = MessageBox.Show("Использовать стандартные веса вместо " +
                                          "других", "Укажите файл весов!", MessageBoxButton.YesNo);
            _weights = file.FileName;
            return file.ShowDialog() != true ? "" : File.ReadAllText(file.FileName);
        }
        
        public void SaveWeights() {
            try {
                MessageBox.Show("Начата запись весов!");
                
                var temp = Layers.Aggregate("", (current, layer) => current + layer.GetData());

                if (File.Exists(_weights)) File.WriteAllText(_weights, temp);
                else {
                    var file = new SaveFileDialog {
                        Filter = "TXT files | *.txt"
                    };
                    MessageBox.Show("Укажите место для сохранения весов!");
                    if (file.ShowDialog() == true) File.WriteAllText(file.FileName, temp);
                    return;
                }
                MessageBox.Show("Веса обновлены!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при записи весов!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }

        public void ReadWeights() {
            try {
                var data = GetWeights();
                var length = data.Length;

                if (data.Length < 1) {
                    MessageBox.Show("Веса не загружены!", "Внимание!", MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return;
                }

                foreach (var layer in Layers)
                    layer.LoadData(data);

                if (length > data.Length) MessageBox.Show("Веса считанны некорректно или не считанны",
                    "Предупреждение!");
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Сбой при чтении весов!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}