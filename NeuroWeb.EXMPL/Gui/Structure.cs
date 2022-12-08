using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NeuroWeb.EXMPL.WINDOWS;

namespace NeuroWeb.EXMPL.Gui {
    public static class Structure {
        public static Grid GetStructure(Teacher teacher, int size) {
            try {
                var tempGrid = new Grid();

                tempGrid.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Входные_Структура.png",UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Height = 70,
                    Width = 120,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 30, 0, 0)
                });

                for (var i = 0; i < size; i++)
                {
                    tempGrid.Children.Add(new Image {
                        Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Скрытый_Структура.png",UriKind.Relative)),
                        Stretch = Stretch.Fill,
                        Height = 100,
                        Width = 120,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0, 100 + 100 * i, 0, 0)
                    });
                    tempGrid.Children.Add(new TextBox
                    {
                        Height = 25,
                        Width = 100,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0, 100 + 100 * i + 3, 0, 0),
                        Background = Brushes.Transparent
                    });
                }

                var img = new Image {
                    Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Увеличить_Структура.png",UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Height = 20,
                    Width = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(150, 100 * size + 40, 0, 0),
                    Cursor = Cursors.Hand,
                    ToolTip = "Добавить скрытый слой"
                };
                img.MouseDown += teacher.IncreaseStructure;
                tempGrid.Children.Add(img);
                
                var img1 = new Image {
                    Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Уменьшить_Структура.png",UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Height = 20,
                    Width = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(150,  100 * size + 10, 0, 0),
                    Cursor = Cursors.Hand,
                    ToolTip = "Убрать скрытый слой"
                };
                img1.MouseDown += teacher.DecreaseStructure;
                tempGrid.Children.Add(img1);
                
                tempGrid.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Выходные_Структура.png",UriKind.Relative)),
                    Stretch = Stretch.Fill,
                    Height = 30,
                    Width = 120,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 100 + 100 * size + 1, 0, 0)
                });

                return tempGrid;

            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception}");
            }
            return null;
        }
    }
}