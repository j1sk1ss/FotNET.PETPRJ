using System;
using System.Windows;
using System.Collections.Generic;

using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class DataWorker {
        public static List<Number> ReadNumber(string config, ref int examples) {
            try {
                var numbers = new List<Number>();

                var lines = config.Split("\n",
                    StringSplitOptions.RemoveEmptyEntries);

                
                if (lines[0].Split(" ")[0] != "Examples") return numbers;
                examples = int.Parse(lines[0].Split(" ")[1]);
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