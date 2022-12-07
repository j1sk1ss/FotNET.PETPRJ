using System;
using System.Windows.Input;
using System.Windows.Threading;
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
                                    ">> Обучение(1/teaching)\n\n>> Работа(2/start)";
        private int _position;

        private void StartAnimation() {
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
            if (temp == "return") SelectOption(UserInput.Content.ToString());
            
            if (UserInput.Content.ToString()!.Length < 20) UserInput.Content += temp;
            else UserInput.Content = "";
        }

        private void SelectOption(string optionName) {
            switch (optionName) {
                case "1":
                    new Teacher();
                    break;
                case "teaching":
                    new Teacher();
                    break;
                case "2":
                    new User().Show();
                    break;
                case "start":
                    new User().Show();
                    break;
            }
            Close();
        }
    }
}