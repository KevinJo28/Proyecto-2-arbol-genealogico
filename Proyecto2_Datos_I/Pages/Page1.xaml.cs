using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Proyecto2_Datos_I;
using Proyecto2_Datos_I_Grafo;

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
            Loaded += Page1_Loaded;
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e) 
        { 
            DibujarMarcadores(); 
        }

        private void DibujarMarcadores() 
        { 
            if (MarkersCanvas == null) return; 

            MarkersCanvas.Children.Clear(); 

            var app = (App)Application.Current; 
            var grafo = app.Family; 

            foreach (var person in grafo.PeopleRedOnly) 
            { 
                if (string.IsNullOrWhiteSpace(person.Coords)) 
                    continue; 

                var parts = person.Coords.Split(','); 
                if (parts.Length != 2) 
                    continue; 

                if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x)) 
                    continue; 

                if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y)) 
                    continue; 

                var marker = CrearMarcadorPersona(person);
                if (marker == null) 
                    continue; 

                Canvas.SetLeft(marker, x);
                Canvas.SetTop(marker, y);
                MarkersCanvas.Children.Add(marker);
            }
        }

        private UIElement? CrearMarcadorPersona(PersonNode person)
        {
            var panel = new StackPanel
            { 
                Orientation = Orientation.Vertical, 
                HorizontalAlignment = HorizontalAlignment.Center, 
                VerticalAlignment = VerticalAlignment.Center,
            }; 

            ImageSource? imgSource = null; 

            if (person.BitMap != null) 
            { 
                imgSource = person.BitMap; 
            } 
            else if (!string.IsNullOrWhiteSpace(person.ImagePath)) 
            { 
                try 
                { 
                    imgSource = new BitmapImage(new Uri(person.ImagePath, UriKind.RelativeOrAbsolute)); 
                } 
                catch 
                { 
                  // Si falla la imagen, dejamos imgSource en null 
                } 
            } 

            var ellipse = new Ellipse 
            { 
                Width = 48, 
                Height = 48, 
                Stroke = Brushes.White, 
                StrokeThickness = 2, 
                Margin = new Thickness(0, 0, 0, 4) 
            }; 

            if (imgSource != null) 
            { 
                ellipse.Fill = new ImageBrush(imgSource) 
                { 
                    Stretch = Stretch.UniformToFill 
                }; 
            } 
            else 
            { 
                ellipse.Fill = Brushes.Gray; 
            } 

            var nameText = new TextBlock 
            { 
                Text = person.FullName, 
                Foreground = Brushes.White, 
                Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)), 
                Padding = new Thickness(4, 2, 4, 2), 
                TextWrapping = TextWrapping.Wrap, 
                TextAlignment = TextAlignment.Center, 
                FontSize = 10 
            }; 

            panel.Children.Add(ellipse); 
            panel.Children.Add(nameText); 

            return panel; 
        } 
    }
}
