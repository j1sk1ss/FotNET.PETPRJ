using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using NeuroWeb.EXMPL.Gui;
using NeuroWeb.EXMPL.OBJECTS;
using NeuroWeb.EXMPL.SCRIPTS;
using Configuration = NeuroWeb.EXMPL.Gui.Configuration;

namespace NeuroWeb.EXMPL.WINDOWS {
    public partial class Teacher {
        public Teacher() {
           InitializeComponent();
        }

        private int _size = 1;
        public void DecreaseStructure(object sender, MouseButtonEventArgs e) {
            if (_size <= 1) return;
            NetworkStructure.Content = Structure.GetStructure(this, --_size);
        }

        public void IncreaseStructure(object sender, MouseButtonEventArgs e) {
            NetworkStructure.Content = Structure.GetStructure(this, ++_size);
        }

        private void CloseTeacher(object sender, MouseButtonEventArgs e) {
            Configuration.WriteConfig(NetworkStructure.Content as Grid, _size + 2);
            Close();
        }

        private void FastTeaching(object sender, MouseButtonEventArgs e) {
            new User().Show();
            Close();
        }
        
        private void HardTeaching(object sender, MouseButtonEventArgs e) {
            MessageBox.Show("Укажите файл конфигурации сети!");
            var file = new OpenFileDialog();
            if (file.ShowDialog() == true) {
                Teaching.HardStudying(new Network(DataWorker.ReadNetworkConfig(file.FileName)));
            }
        }
    }
}