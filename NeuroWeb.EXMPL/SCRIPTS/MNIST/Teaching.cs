using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using NeuroWeb.EXMPL.NETWORK;
using NeuroWeb.EXMPL.NETWORK.OBJECTS;
using NeuroWeb.EXMPL.SCRIPTS.DATA;

namespace NeuroWeb.EXMPL.SCRIPTS.MNIST
{
    public static class Teaching
    {
        public static void LightStudying(Network network, Tensor data, int expected)
        {
            try
            {
                var prediction = network.ForwardFeed(data);
                if (expected.Equals(prediction)) return;
                network.BackPropagation(expected);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "Ошибка при обучении!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public static void HardStudying(Network network, int teachingCounts)
        {
            try
            {
                double rightAnswersCount = 0d, maxRightAnswers = 0d;

                var era = 0;
                var examples = 0;

                MessageBox.Show("Укажите файл обучения!");
                var file = new OpenFileDialog();
                if (file.ShowDialog() != true) return;

                var dataInformation = DataWorker.ReadNumber(File.ReadAllText(file.FileName), ref examples);

                MessageBox.Show($"Загруженно приверов: {examples}\n");
                while (rightAnswersCount / examples * 100 < 97)
                {
                    rightAnswersCount = 0;
                    for (var i = 0; i < examples; ++i)
                    {
                        var right = dataInformation[i].Digit;
                        var prediction = network.ForwardFeed(new Tensor(dataInformation[i].GetAsMatrix()));
                        //MessageBox.Show(right + " pr: " + prediction);
                        if (prediction != right)
                            network.BackPropagation(right);
                        else rightAnswersCount++;
                    }
                    if (rightAnswersCount > maxRightAnswers) maxRightAnswers = rightAnswersCount;
                    MessageBox.Show($"Правильно: {Math.Round(rightAnswersCount / examples * 100, 3)}%\n" +
                                    $"Максимум правильных: {Math.Round(maxRightAnswers / examples * 100, 3)}%\n" +
                                    $"Цикл обучения №{era}");

                    if (++era == teachingCounts) break;
                }
                WeightsWorker.ExportData(network);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "Ошибка при глубоком обучении!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}