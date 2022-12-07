using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NeuroWeb.EXMPL.SCRIPTS;

namespace NeuroWeb.EXMPL {
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show($"{new Prediction().Predict("")}");
            /*
            TextUpdate = new DispatcherTimer() {
                Interval = new TimeSpan(900000)
            };
            TextUpdate.Tick += UpdateText;
            TextUpdate.IsEnabled = true;
            
            UserInputAnimation = new DispatcherTimer() {
                Interval = new TimeSpan(0,0,0,1)
            };
            UserInputAnimation.Tick += UpdateInput;
            UserInputAnimation.IsEnabled = true;
            */
        }
        
        private DispatcherTimer TextUpdate { get; set; }
        
        private DispatcherTimer UserInputAnimation { get; set; }

        private const string Text = ">> >> >> Меню\n\n>> Приветствую!\n\n>> Выберите действие:\n\n" +
                                    ">> Обучение(1/teaching)\n\n>> Работа(2/start)";

        private int _position;
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
            
            if (UserInput.Content.ToString()!.Length < 20) UserInput.Content += temp;
            else UserInput.Content = "";
        }
    }
}