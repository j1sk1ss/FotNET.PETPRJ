using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NeuroWeb.EXMPL.SCRIPTS;
using NeuroWeb.EXMPL.WINDOWS;
using Matrix = NeuroWeb.EXMPL.OBJECTS.Matrix;

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