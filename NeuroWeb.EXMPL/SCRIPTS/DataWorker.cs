using System;
using System.IO;
using System.Windows;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS
{
    public static class DataWorker
    {
        public static Data ReadNetworkConfig(string path) {
            try {
                var data           = new Data();
                var tempData = File.ReadAllText(path).Split(new char[] {' ', '\n'},
                    StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < tempData.Length; i++) {

                    if (tempData[i] != "NetWork") continue;
                    var layouts = int.Parse(tempData[i + 1]);
                    
                    data.Layout = layouts;
                    data.Size   = new int[layouts];

                    for (var j = 1; j < layouts + 1; j++)
                        data.Size[j - 1] = int.Parse(tempData[i + 1 + j]);
                    break;
                }

                return data;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }

        public static DataInformation[] ReadData(string path, ref Data data, ref int examples) {
            try {
                var dataInformation = Array.Empty<DataInformation>();

                var tempValues = File.ReadAllText(path).Split(new char[] {' ', '\n'},
                    StringSplitOptions.RemoveEmptyEntries);
                var position = 0;

                if (tempValues[position++] != "Examples") return dataInformation;
                
                examples = int.Parse(tempValues[position++]);

                dataInformation = new DataInformation[examples];
                for (var i = 0; i < examples; i++)
                    dataInformation[i].Pixels = new double[data.Size[0]];

                for (var i = 0; i < examples; i++) {
                    dataInformation[i].Digit = int.TryParse(tempValues[position++], out var it) ? it : 0;
                    for (var j = 0; j < data.Size[0]; j++)
                        if (double.TryParse(tempValues[position++], out var db)) {
                            dataInformation[i].Pixels[j] = db;
                        }
                        else dataInformation[i].Pixels[j] = 0d;
                }

                return dataInformation;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
    }
}