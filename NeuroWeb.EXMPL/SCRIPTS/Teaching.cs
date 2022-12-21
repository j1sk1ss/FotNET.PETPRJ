using System;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32;

using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public static class Teaching {
        public static void LightStudying(Network network, string number, int expected) {
            try {
                var dataInformation = DataWorker.ReadData(number, network.Configuration);
                network.InsertInformation(dataInformation.Pixels);
                
                var prediction = network.ForwardFeed();
                if (expected.Equals((int)prediction)) return;
                
                network.BackPropagation(expected);
                network.SetWeights(.08);
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
                
                var dataInformation = DataWorker.ReadData(file.FileName, network.Configuration, ref examples);
                
                MessageBox.Show($"Загруженно приверов: {examples}\n");
                while (rightAnswersCount / examples * 100 < 100) {
                    rightAnswersCount = 0;
                    for (var i = 0; i < examples; ++i) {
                        network.InsertInformation(dataInformation[i].Pixels);
                        double right = dataInformation[i].Digit;
                        
                        var prediction = network.ForwardFeed();
                        if (!prediction.Equals(right)) {
                            network.BackPropagation(right);
                            network.SetWeights(.15d * Math.Exp(-era / 20d));
                        }
                        else rightAnswersCount++;
                    }
                    if (rightAnswersCount > maxRightAnswers) maxRightAnswers = rightAnswersCount;
                    MessageBox.Show($"Правильно: {Math.Round(rightAnswersCount / examples * 100, 3)}%\n" +
                                    $"Максимум правильных: {Math.Round(maxRightAnswers / examples * 100, 3)}%\n" +
                                    $"Цикл обучения №{era}");
                    
                    if (++era == teachingCounts) break;
                }
                network.SaveWeights();
            }
            catch (Exception e) {
                MessageBox.Show($"{e}", "Ошибка при глубоком обучении!", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}