using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class DataWorker {
        public static Configuration ReadNetworkConfig(string config) {
            try {
                var data     = new Configuration();
                var tempData = config.Split(new[] {' ', '\n'},
                    StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < tempData.Length; i++) {

                    if (tempData[i] != "Нейронка") continue;
                    var layouts = int.Parse(tempData[i + 1]);
                    
                    data.Layout = layouts;
                    data.NeuronsLayer   = new int[layouts];

                    for (var j = 1; j < layouts + 1; j++)
                        data.NeuronsLayer[j - 1] = int.Parse(tempData[i + 1 + j]);
                    break;
                }

                return data;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }

        [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
        public static Number ReadData(string pixelsValue, Configuration configuration) {
            try {
                var number = new Number();

                for (var i = 0; i < configuration.NeuronsLayer[0]; i++) number.Pixels.Add(0);
               
                var position = 0;
                var pixels = pixelsValue.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                for (var j = 0; j < configuration.NeuronsLayer[0]; j++) {
                    if (double.TryParse(pixels[position], out var db)) {
                        number.Pixels[j] = db;
                    }
                    else number.Pixels[j] = 0d;
                    position++;
                }
                return number;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
        
        public static List<Number> ReadData(string config, Configuration configuration, ref int examples) {
            try {
                var numbers = new List<Number>();

                var tempValues = config.Split(new[] {' ', '\n'},
                    StringSplitOptions.RemoveEmptyEntries);
                var position = 0;

                if (tempValues[position++] != "Examples") return numbers;
                
                examples = int.Parse(tempValues[position++]);
                for (var i = 0; i < examples; i++) 
                    numbers.Add(new Number());
                    
                for (var i = 0; i < examples; i++) 
                    for (var j = 0; j < configuration.NeuronsLayer[0]; j++) 
                        numbers[i].Pixels.Add(0);
                
                for (var i = 0; i < examples; i++) {
                    numbers[i].Digit = int.TryParse(tempValues[position++], out var it) ? it : 0;
                    for (var j = 0; j < configuration.NeuronsLayer[0]; j++)
                        if (double.TryParse(tempValues[position++], out var db)) {
                            numbers[i].Pixels[j] = db;
                        }
                        else numbers[i].Pixels[j] = 0d;
                }

                return numbers;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
    }
}