using System; 
using System.Globalization; 
using System.Linq; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; 
using System.Windows.Media.Imaging; 
using System.Windows.Shapes; 
using Proyecto2_Datos_I; 
using Proyecto2_Datos_I_Grafo; 

namespace SideBar_Nav.Pages
{
    public partial class Page1 : Page
    {
        private PersonNode? _selectedPerson; 

        // tamaño de fuente de la lista de distancias
        private double _distancesFontSize = 18;

        public Page1()
        {
            InitializeComponent();
            Loaded += Page1_Loaded; 
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e) 
        {
            DibujarMarcadores(); 
            DibujarLineas(null); 
        }

        // Método público para que la UI (o pruebas) puedan modificar el tamaño de fuente
        public void SetDistancesFontSize(double size)
        {
            if (size <= 0) return;
            _distancesFontSize = size;
            // Redibujar para aplicar el nuevo tamaño
            DibujarLineas(_selectedPerson);
        }

        // Manejador de ejemplo para un Slider en XAML: ValueChanged="DistanceFontSizeSlider_ValueChanged"
        private void DistanceFontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetDistancesFontSize(e.NewValue);
        }

        // ---------- Marcadores ----------

        private void DibujarMarcadores() 
        {
            if (MarkersCanvas == null) return; 

            MarkersCanvas.Children.Clear(); 

            var app = (App)Application.Current; 
            var grafo = app.Family; 

            foreach (var person in grafo.PeopleRedOnly) 
            {
                if (!TryGetCoords(person, out double x, out double y)) 
                    continue; 

                bool isSelected = _selectedPerson != null && ReferenceEquals(_selectedPerson, person); 

                var marker = CrearMarcadorPersona(person, isSelected); 
                if (marker == null) 
                    continue; 

                Canvas.SetLeft(marker, x); 
                Canvas.SetTop(marker, y); 
                MarkersCanvas.Children.Add(marker); 
            }
        }

        private bool TryGetCoords(PersonNode person, out double x, out double y) 
        {
            x = 0; y = 0; 
            if (string.IsNullOrWhiteSpace(person.Coords)) return false; 

            var parts = person.Coords.Split(','); 
            if (parts.Length != 2) return false; 

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x)) 
                return false; 

            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y)) 
                return false; 

            return true; 
        }

        private UIElement? CrearMarcadorPersona(PersonNode person, bool isSelected) 
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Tag = person; 
            panel.MouseLeftButtonUp += Marker_MouseLeftButtonUp; 

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
                    // Imagen inválida, dejamos imgSource en null 
                }
            }

            var ellipse = new Ellipse
            {
                Width = isSelected ? 56 : 48, 
                Height = isSelected ? 56 : 48, 
                Stroke = isSelected ? Brushes.Gold : Brushes.White, 
                StrokeThickness = isSelected ? 3 : 2, 
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

        private void Marker_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) 
        {
            if (sender is FrameworkElement fe && fe.Tag is PersonNode person) 
            {
                _selectedPerson = person; 
                DibujarMarcadores(); 
                DibujarLineas(_selectedPerson); 
            }
        }

        // líneas y panel de distancias

        private void DibujarLineas(PersonNode? origen) 
        {
            LinesCanvas.Children.Clear(); 
            DistancesPanel.Children.Clear(); 

            if (SelectedPersonName != null) 
            {
                SelectedPersonName.Text = origen?.FullName ?? "Sin selección"; 
            }

            if (origen == null) return; 

            var app = (App)Application.Current; 
            var grafo = app.Family; 

            if (!TryGetCoords(origen, out double x0, out double y0)) 
                return; 

            double cx0 = x0 + 24; // centro aprox del círculo 
            double cy0 = y0 + 24; 
            if (_selectedPerson != null && origen == _selectedPerson) 
            { 
                cx0 = x0 + 28; 
                cy0 = y0 + 28; 
            }

            var distancias = grafo.PeopleRedOnly 
                .Where(p => !ReferenceEquals(p, origen)) 
                .Select(p =>
                {
                    if (!TryGetCoords(p, out double x, out double y)) 
                        return (p, distancia: double.NaN, x, y); 

                    double cx = x + 24; 
                    double cy = y + 24; 
                    double dx = cx - cx0; 
                    double dy = cy - cy0; 
                    double d = Math.Sqrt(dx * dx + dy * dy); 
                    return (p, distancia: d, x, y); 
                })
                .Where(t => !double.IsNaN(t.distancia)) 
                .OrderBy(t => t.distancia) 
                .ToList(); 

            foreach (var (personaDestino, distancia, x, y) in distancias) 
            {
                double cx = x + 24; 
                double cy = y + 24; 

                var line = new Line 
                {
                    X1 = cx0,
                    Y1 = cy0,
                    X2 = cx,
                    Y2 = cy,
                    Stroke = new SolidColorBrush(Color.FromArgb(160, 255, 0, 0)), 
                    StrokeThickness = 2,
                    SnapsToDevicePixels = true
                };

                LinesCanvas.Children.Add(line); 

                var tb = new TextBlock 
                {
                    Text = $"{personaDestino.FullName}: {distancia:0} km",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 4),
                    FontSize = _distancesFontSize
                };

                DistancesPanel.Children.Add(tb); 
            }
        }
    }
}

