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

                tempGrid.Children.Add(new Image {
                    Height = 100,
                    Width  = 120,                    
                    
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Top,
                    Stretch             = Stretch.Fill,                    
                    Margin              = new Thickness(0, 5, 0, 0),
                    
                    Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Входные_Структура.png",UriKind.Relative))                   
                });

                tempGrid.Children.Add(new TextBox {
                    Height = 23,
                    Width  = 100,
                    
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Top,
                    TextAlignment       = TextAlignment.Center,
                    Margin              = new Thickness(0,10,0,0),
                    
                    Background = Brushes.Transparent,
                    Text       = "784",
                    IsEnabled  = false
                });

                for (var i = 0; i < size; i++) {
                    tempGrid.Children.Add(new Image {
                        Height = 100,
                        Width  = 120,

                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment   = VerticalAlignment.Top,
                        Stretch             = Stretch.Fill,                                
                        Margin              = new Thickness(0, 105 + 105 * i, 0, 0),   
                
                        Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Скрытый_Структура.png",UriKind.Relative))
                    });
                    tempGrid.Children.Add(new TextBox {
                        Height = 25,
                        Width  = 100,
                        
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment   = VerticalAlignment.Top,
                        Margin              = new Thickness(0, 105 + 105 * i + 3, 0, 0),
                        
                        Background = Brushes.Transparent
                    });
                }

                var img = new Image {
                    Height = 20,
                    Width  = 20,                    
                    
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Top,
                    Stretch             = Stretch.Fill,                       
                    Margin              = new Thickness(150, 105 * size + 45, 0, 0),

                    Source  = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Увеличить_Структура.png",UriKind.Relative)),                    
                    Cursor  = Cursors.Hand,
                    ToolTip = "Добавить скрытый слой"
                };
                img.MouseDown += teacher.IncreaseStructure;
                tempGrid.Children.Add(img);
                
                var img1 = new Image {
                    Height = 20,
                    Width  = 20,                    
                    
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Top,
                    Stretch             = Stretch.Fill,                    
                    Margin              = new Thickness(150,  105 * size + 15, 0, 0),
                    
                    Source  = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Уменьшить_Структура.png",UriKind.Relative)),                    
                    Cursor  = Cursors.Hand,
                    ToolTip = "Убрать скрытый слой"
                };
                img1.MouseDown += teacher.DecreaseStructure;
                tempGrid.Children.Add(img1);
                
                tempGrid.Children.Add(new Image {
                    Height = 65,
                    Width  = 120,             
                    
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Top,                    
                    Stretch             = Stretch.Fill,      
                    Margin              = new Thickness(0, 102.6 + 102.6 * size + 3, 0, 0),        
                    
                    Source = new BitmapImage(new Uri("../IMAGES/TeacherWindow/Выходные_Структура.png",UriKind.Relative))
                });

                tempGrid.Children.Add(new TextBox {
                    Height = 23,
                    Width  = 100,
                    
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Top,
                    TextAlignment       = TextAlignment.Center,
                    Margin              = new Thickness(0,102.6 + 102.6 * size + 8,0,0),
                    
                    Background = Brushes.Transparent,
                    Text       = "10",
                    IsEnabled  = false
                });
                
                return tempGrid;

            }
            catch (Exception exception) {
                MessageBox.Show($"{exception}", "Ошибка создания интерфейса!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            return null;
        }
    }
}