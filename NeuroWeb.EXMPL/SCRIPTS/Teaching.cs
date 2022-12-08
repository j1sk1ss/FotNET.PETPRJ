using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
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
                MessageBox.Show($"{e}");
                throw;
            }
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.Double[]")]
        public static void HardStudying(Network network) {
            try {
                double rightAnswersCount = 0d, maxRightAnswers = 0d;
                
                var epoch    = 0;
                var examples = 0;

                MessageBox.Show("Укажите файл обучения!");
                var file = new OpenFileDialog();
                if (file.ShowDialog() != true) return;
                
                var dataInformation = DataWorker.ReadData(file.FileName, network.Configuration, ref examples);
                
                MessageBox.Show($"Загруженно приверов: {examples}");
                while (rightAnswersCount / examples * 100 < 100) {
                    rightAnswersCount = 0;
                    for (var i = 0; i < examples; ++i) {
                        network.InsertInformation(dataInformation[i].Pixels);
                        double right = dataInformation[i].Digit;
                        
                        var prediction = network.ForwardFeed();
                        if (!prediction.Equals(right)) {
                            network.BackPropagation(right);
                            network.SetWeights(.15d * Math.Exp(-epoch / 20d));
                        }
                        else rightAnswersCount++;
                    }
                    if (rightAnswersCount > maxRightAnswers) maxRightAnswers = rightAnswersCount;
                    MessageBox.Show($"Right Answers: {Math.Round(rightAnswersCount / examples * 100, 3)}%\n" +
                                    $"Maximum Right Answers: {Math.Round(maxRightAnswers / examples * 100, 3)}%\n" +
                                    $"Generation: {epoch}");
                    
                    if (++epoch == 20) break;
                }
                network.SaveWeights();
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
    }
}