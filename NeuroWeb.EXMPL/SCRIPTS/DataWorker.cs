using System;
using System.Windows;
using System.Collections.Generic;

using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;
using Vector = NeuroWeb.EXMPL.OBJECTS.Vector;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class DataWorker {
        public static Configuration ReadNetworkConfig(string config) {
            var data = new Configuration();
            var tempData = config.Split("\n",
                StringSplitOptions.RemoveEmptyEntries);

            data.ConvolutionConfigurations = new ConvolutionConfiguration[(tempData.Length - 2) / 3];
            var layer = 0;

            for (var i = 0; i < tempData.Length - 1; i++) {
                var lineSymbols = tempData[i].Split(" ");
                var option = lineSymbols[0];
                
                switch (option) {
                    case "Нейронка":
                        data.ConvolutionLayouts = int.Parse(lineSymbols[1]);
                        data.ForwardLayout      = int.Parse(lineSymbols[2]);
                        break;
                    case "Фильтр:":
                        data.ConvolutionConfigurations[layer].FilterColumn = int.Parse(lineSymbols[1]);
                        data.ConvolutionConfigurations[layer].FilterRow    = int.Parse(lineSymbols[2]);
                        data.ConvolutionConfigurations[layer].FilterDepth  = int.Parse(lineSymbols[3]);
                        data.ConvolutionConfigurations[layer].FilterCount  = int.Parse(lineSymbols[4]);
                        break;
                    case "Шаг:":
                        data.ConvolutionConfigurations[layer].Stride = int.Parse(lineSymbols[1]);
                        break;
                    case "Пул:":
                        data.ConvolutionConfigurations[layer++].PoolSize = int.Parse(lineSymbols[1]);
                        break;
                }
            }
            
            var perceptronLine = tempData[^1].Split(" ");
            data.NeuronsLayer = new int[perceptronLine.Length];

            for (var i = 0; i < perceptronLine.Length; i++) 
                data.NeuronsLayer[i] = int.Parse(perceptronLine[i]);

            return data;
        }
        
        public static Tensor ReadImage(double[][] pixelsValue) {
            var tensor = new Tensor(new List<Matrix>());
            
            for (var i = 0; i < pixelsValue.Length; i++) {
                var matrix = new Vector(pixelsValue[i]).AsMatrix((int)Math.Sqrt(pixelsValue.Length),
                    (int)Math.Sqrt(pixelsValue.Length));
                tensor.Channels.Add(matrix);
            }

            return tensor;
        }
        
        public static Tensor ReadImage(double[] pixelsValue) {
            var tensor = new Tensor(new List<Matrix>());
            
            var matrix = new Vector(pixelsValue).AsMatrix((int)Math.Sqrt(pixelsValue.Length),
                (int)Math.Sqrt(pixelsValue.Length));
            tensor.Channels.Add(matrix);
            
            return tensor;
        }
        
        public static List<Number> ReadNumber(string config, Configuration configuration, ref int examples) {
            try {
                var numbers = new List<Number>();

                var lines = config.Split("\n",
                    StringSplitOptions.RemoveEmptyEntries);

                
                if (lines[0].Split(" ")[0] != "Examples") return numbers;
                //examples = int.Parse(lines[0].Split(" ")[1]);
                examples = 1000;
                
                for (var i = 0; i < examples; i++) {
                    numbers.Add(new Number());
                    for (var j = 0; j < 784; j++) 
                        numbers[i].Pixels.Add(0);                    
                }
                
                for (var i = 0; i < numbers.Count; i++) {
                    for (var j = 0; j < 29; j++) {
                        var symbols = lines[j + 29 * i + 1].Split(" ");
                        if (symbols.Length <= 1) numbers[i].Digit = int.Parse(symbols[0]);
                        else {
                            for (var k = 0; k < 28; k++) {
                                if (double.TryParse(symbols[k], out var value)) numbers[i].Pixels[k + 28 * (j - 1)] = value;
                            }
                        }
                    }
                }

                return numbers;
            }
            catch (Exception e) {
                MessageBox.Show($"{e}","Ошибка создания обьекта числа", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}