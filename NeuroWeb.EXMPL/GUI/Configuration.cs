using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using Microsoft.Win32;

namespace NeuroWeb.EXMPL.Gui {
    public static class Configuration {
        public static void WriteConfig(Grid configGrid, int size) {
            var tempConfig = $"Нейронка {size}\n";
            var tempArray = new List<int>();
            
            foreach (var element in configGrid.Children) {
                if (element.GetType() != typeof(TextBox)) continue;
                var text = (element as TextBox)!.Text;

                if (int.TryParse(text, out var layerSize)) {
                    tempConfig += $"{layerSize} ";
                    tempArray.Add(layerSize);
                }
                else {
                    MessageBox.Show("Некорректный размер скрытого слоя!");
                    tempConfig += "0 ";
                    tempArray.Add(0);
                }
            }

            for (var i = 0; i < tempArray.Count - 1; i++) {
                if (tempArray[i] >= tempArray[i + 1]) continue;
                MessageBox.Show("Некоректные размерности скрытыхte слоёв. Идут не по убыванию!");
                break;
            }

            var openFile = new SaveFileDialog();
            if (openFile.ShowDialog() == true) {
                File.WriteAllText(openFile.FileName, tempConfig);
            }
        }
    }
}