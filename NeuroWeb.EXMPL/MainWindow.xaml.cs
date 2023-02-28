using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.WINDOWS;


namespace NeuroWeb.EXMPL {
    public partial class MainWindow
    {
        public MainWindow() {
            InitializeComponent();
            StartAnimation();
        }
        
        private DispatcherTimer TextUpdate { get; set; }
        private DispatcherTimer UserInputAnimation { get; set; }

        private const string Text = ">> >> >> Меню\n\n>> Приветствую!\n\n>> Выберите действие:\n\n" +
                                    ">> Обучение (teaching)\n\n>> Работа (start)";
        private int _position;

        private void StartAnimation() {
            try {
                var config = new Configuration {
                    ConvolutionConfigurations = new ConvolutionConfiguration[3],
                    ConvolutionLayouts = 3,
                    ForwardLayout = 3,
                    NeuronsLayer = new[]{288, 144, 10}
                };
                config.ConvolutionConfigurations[0] = new ConvolutionConfiguration {
                    FilterColumn = 3,
                    FilterRow = 3,
                    FilterCount = 2,
                    PoolSize = 2,
                    Stride = 1,
                    FilterDepth = 2
                };
                config.ConvolutionConfigurations[1] = new ConvolutionConfiguration {
                    FilterColumn = 3,
                    FilterRow = 3,
                    FilterCount = 2,
                    PoolSize = 2,
                    Stride = 1,
                    FilterDepth = 4
                };
                config.ConvolutionConfigurations[2] = new ConvolutionConfiguration {
                    FilterColumn = 3,
                    FilterRow = 3,
                    FilterCount = 2,
                    PoolSize = 2,
                    Stride = 1,
                    FilterDepth = 8
                };

                var matrix = new Matrix(64, 64);
                for (var i = 0; i < matrix.Body.GetLength(0); i++) {
                    for (var j = 0; j < matrix.Body.GetLength(1); j++) {
                        matrix.Body[i, j] = (float)new Random().Next() % 10 - 5; 
                    }
                }
                
                var tensor = new Tensor(matrix);

                var network = new Network(config);
                network.InsertInformation(tensor);
            
                network.ForwardFeed();
                network.BackPropagation(1, .08);
            }
            catch (Exception e) {
                MessageBox.Show($"{e}");
                throw;
            }
            
            




































            TextUpdate = new DispatcherTimer {
                Interval = new TimeSpan(900000)
            };
            TextUpdate.Tick += UpdateText;
            TextUpdate.IsEnabled = true;
            
            UserInputAnimation = new DispatcherTimer {
                Interval = new TimeSpan(0,0,0,1)
            };
            UserInputAnimation.Tick += UpdateInput;
            UserInputAnimation.IsEnabled = true;
        }
        
        private void UpdateInput(object sender, EventArgs eventArgs) {
            if (UserInput.Content.ToString()!.Contains("|")) UserInput.Content = 
                UserInput.Content.ToString()!.Replace("|", "");
            else UserInput.Content += "|";
        } 
        
        private void UpdateText(object sender, EventArgs eventArgs) {
            if (_position >= Text.Length) {
                TextUpdate.IsEnabled = false;
                return;
            }
            
            MainText.Content += Text[_position++].ToString();
        }

        private void UserPressKey(object sender, KeyEventArgs e) {
            if (UserInput.Content.ToString()!.Contains("|")) UserInput.Content = 
                UserInput.Content.ToString()!.Replace("|", "");
            
            var temp = e.Key.ToString().ToLower();
            
            switch (temp) {
                case "return":
                    SelectOption(UserInput.Content.ToString());
                    return;
                case "back":
                    var str = UserInput.Content.ToString();
                    if (str!.Length <= 0) return;
                    UserInput.Content = UserInput.Content.ToString()!.Remove(str!.Length - 1);
                    return;
            }
            
            if (UserInput.Content.ToString()!.Length < 20) UserInput.Content += temp;
            else UserInput.Content = "";
        }

        private void SelectOption(string optionName) {
            switch (optionName) {
                case "teaching":
                    new Teacher().Show();
                    break;
                case "start":
                    new User().Show();
                    break;
                default:
                    return;
            }
            Close();
        }
    }
}