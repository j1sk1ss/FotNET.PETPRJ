using System;
using System.IO;
using System.Windows;
using NeuroWeb.EXMPL.OBJECTS;

namespace NeuroWeb.EXMPL.SCRIPTS {
    public class Teaching {
        public void Studying() {
            try {
                double rightAnswersCount = 0d, maxRightAnswers = 0d;
                var epoch = 0;
                var repeat = true;

                var data = DataWorker.ReadNetworkConfig(@"C:\\Users\\j1sk1ss\\RiderProjects\\NeuroWeb.EXMPL\\" +
                                                       @"NeuroWeb.EXMPL\\DATA\\Config.txt");
                
                var network = new Network(data);
                MessageBox.Show($"{network.GetConfiguration()}");

                while (repeat) {
                    var examples = 0;
                    var dataInformation =
                        DataWorker.ReadData(@"C:\\Users\\j1sk1ss\\RiderProjects\\NeuroWeb.EXMPL\\NeuroWeb.EXMPL\\DATA\\References.txt",
                            ref data, ref examples);
                    MessageBox.Show($"Examples: {examples}");
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
                        MessageBox.Show($"Right Answers:{rightAnswersCount / examples * 100}%\n" +
                                        $"Maximum Right Answers:{maxRightAnswers / examples * 100}%\n" +
                                        $"epoch:{epoch}");
                        
                        if (++epoch == 20) break;
                    }
                    network.SaveWeights();
                    repeat = false;
                }
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
        }
    }

    public struct DataInformation {
        public double[] Pixels;
        public int Digit;
    }
}