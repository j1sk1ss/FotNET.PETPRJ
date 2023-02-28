using System;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32;

using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.OBJECTS.CONVOLUTION;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Teaching {
        public static void LightStudying(Network network, Tensor data, int expected) {
            try {
                network.InsertInformation(data);
                
                var prediction = network.ForwardFeed();
                if (expected.Equals(prediction)) return;
                
                network.BackPropagation(expected, .08d);
            }
            catch (Exception e) {
                MessageBox.Show($"{e}", "Ошибка при обучении!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }


        [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
        public static void HardStudying(Network network, int teachingCounts) {
            try {
                double rightAnswersCount = 0d, maxRightAnswers = 0d;
                
                var era      = 0;
                var examples = 0;

                MessageBox.Show("Укажите файл обучения!");
                var file = new OpenFileDialog();
                if (file.ShowDialog() != true) return;
                
                var dataInformation = DataWorker.ReadNumber(file.FileName, network.Configuration, ref examples);
                
                MessageBox.Show($"Загруженно приверов: {examples}\n");
                while (rightAnswersCount / examples * 100 < 100) {
                    rightAnswersCount = 0;
                    for (var i = 0; i < examples; ++i) {
                        network.InsertInformation(dataInformation[i]);
                        
                        var right = dataInformation[i].Digit;
                        var prediction = network.ForwardFeed();
                        
                        if (prediction != right) 
                            network.BackPropagation(right, .15d * Math.Exp(-era / 20d));
                        else rightAnswersCount++;
                    }
                    if (rightAnswersCount > maxRightAnswers) maxRightAnswers = rightAnswersCount;
                    MessageBox.Show($"Правильно: {Math.Round(rightAnswersCount / examples * 100, 3)}%\n" +
                                    $"Максимум правильных: {Math.Round(maxRightAnswers / examples * 100, 3)}%\n" +
                                    $"Цикл обучения №{era}");
                    
                    if (++era == teachingCounts) break;
                }
                //network.SaveForwardWeights();
            }
            catch (Exception e) {
                MessageBox.Show($"{e}", "Ошибка при глубоком обучении!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}