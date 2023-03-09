using Microsoft.Win32;
using NeuroWeb.EXMPL.NETWORK;
using System.IO;
using System.Linq;
using System.Windows;

namespace NeuroWeb.EXMPL.SCRIPTS.DATA
{
    public static class WeightsWorker {
        private static string GetFile() {
            var file = new OpenFileDialog
            {
                Filter = "TXT files | *.txt"
            };
            var message = MessageBox.Show("Использовать стандартные веса вместо " +
                                          "других", "Укажите файл весов!", MessageBoxButton.YesNo);
            _weights = file.FileName;
            return file.ShowDialog() != true ? "" : File.ReadAllText(file.FileName);
        }

        private static string _weights;

        public static void ExportData(Network network) {
            MessageBox.Show("Начата запись весов!");

            var temp = network.Layers.Aggregate("", (current, layer) => current + layer.GetData());

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

        public static Network LoadData(Network network) {
            var data = GetFile();
            var length = data.Length;

            if (data.Length < 1) {
                MessageBox.Show("Веса не загружены!", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                return null;
            }

            foreach (var layer in network.Layers)
                layer.LoadData(data);

            if (length > data.Length) MessageBox.Show("Веса считанны некорректно или не считанны",
                "Предупреждение!");

            return network;
        }
    }
}
