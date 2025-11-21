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
    // PÁGINA DE MAPA 
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

        //Método para cargar la página
        private void Page1_Loaded(object sender, RoutedEventArgs e) 
        {
            DibujarMarcadores(); 
            DibujarLineas(null); 
        }

        // Método público para modificar el tamaño de fuente
        public void SetDistancesFontSize(double size)
        {
            if (size <= 0) return;
            _distancesFontSize = size;
            DibujarLineas(_selectedPerson);
        }

        private void DistanceFontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetDistancesFontSize(e.NewValue);
        }

        // Método para dibujar marcadores
        private void DibujarMarcadores() 
        {
            if (MarkersCanvas == null) return; 

            MarkersCanvas.Children.Clear(); // limpiar marcadores previos

            var app = (App)Application.Current; 
            var grafo = app.Family; 

            foreach (var person in grafo.PeopleRedOnly) // para cada persona en el grafo
            {
                if (!TryGetCoords(person, out double x, out double y)) 
                    continue; // si no tiene coordenadas válidas, saltar

                bool isSelected = _selectedPerson != null && ReferenceEquals(_selectedPerson, person); // verificar si es la persona seleccionada

                var marker = CrearMarcadorPersona(person, isSelected); 
                if (marker == null) 
                    continue; // si no se pudo crear el marcador, saltar

                Canvas.SetLeft(marker, x); 
                Canvas.SetTop(marker, y); 
                MarkersCanvas.Children.Add(marker); // si todo sale bien, agregar el marcador al canvas
            }
        }

        // Método para obtener coordenadas
        private bool TryGetCoords(PersonNode person, out double x, out double y) 
        {
            x = 0; y = 0; 
            if (string.IsNullOrWhiteSpace(person.Coords)) return false; // validar coordenadas existentes

            var parts = person.Coords.Split(','); 
            if (parts.Length != 2) return false; // validar formato correcto

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x)) 
                return false;  // validar que se pueda convertir a double

            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y)) 
                return false; // validar que se pueda convertir a double

            if (x < MapBounds.MinX || x > MapBounds.MaxX || y < MapBounds.MinY || y > MapBounds.MaxY) 
                return false; // validar que estén dentro de los límites del mapa

            return true; 
        }

        // Método para crear marcador de persona
        private UIElement? CrearMarcadorPersona(PersonNode person, bool isSelected) 
        {
            var panel = new StackPanel // contenedor del marcador
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Tag = person; 
            panel.MouseLeftButtonUp += Marker_MouseLeftButtonUp; 

            ImageSource? imgSource = null; 

            if (person.BitMap != null)  // usar imagen en memoria si está disponible
            {
                imgSource = person.BitMap; 
            }
            else if (!string.IsNullOrWhiteSpace(person.ImagePath)) // si no, intentar cargar desde ruta
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

            var ellipse = new Ellipse // crear el círculo del marcador
            {
                Width = isSelected ? 64 : 58, 
                Height = isSelected ? 64 : 58, 
                Stroke = isSelected ? Brushes.Red : Brushes.White, 
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
                ellipse.Fill = Brushes.Gray; // si no hay imagen, usar gris
            }

            var nameText = new TextBlock // texto con el nombre
            {
                Text = person.FullName,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)),
                Padding = new Thickness(4, 2, 4, 2),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                FontSize = 12
            };

            panel.Children.Add(ellipse); 
            panel.Children.Add(nameText);

            return panel;
        }

        // evento al hacer clic en un marcador
        private void Marker_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) 
        {
            if (sender is FrameworkElement fe && fe.Tag is PersonNode person) // si se obtuvo la persona del marcador
            {
                _selectedPerson = person; 
                DibujarMarcadores(); 
                DibujarLineas(_selectedPerson);
            } // entonces redibujar marcadores y líneas
        }

        // Método para dibujar líneas de distancia
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

            double cx0 = x0 + 29; // centro aprox del círculo 
            double cy0 = y0 + 29; // el centro en y es igual a x + radio
            if (_selectedPerson != null && origen == _selectedPerson) 
            { 
                cx0 = x0 + 29; 
                cy0 = y0 + 29; 
            }

            var distancias = grafo.PeopleRedOnly 
                .Where(p => !ReferenceEquals(p, origen)) // excluir la persona de origen
                .Select(p =>
                {
                    if (!TryGetCoords(p, out double x, out double y)) // obtener coordenadas
                        return (p, distancia: double.NaN, x, y); // si no son válidas, devolver NaN
                    // cálculo de la distancia euclidiana
                    double cx = x + 29; // centro en x del círculo
                    double cy = y + 29; // centro en y del círculo
                    double dx = cx - cx0; // diferencia en x de un miembro a otro
                    double dy = cy - cy0; // diferencia en y
                    double d = Math.Sqrt(dx * dx + dy * dy); // raiz de las suma de las diferencias al cuadrado
                    return (p, distancia: d, x, y); // devulve la distancia calculada
                })
                .Where(t => !double.IsNaN(t.distancia)) 
                .OrderBy(t => t.distancia) 
                .ToList();  // ordenar por distancia

            foreach (var (personaDestino, distancia, x, y) in distancias) // dibuja las líneas y textos
            {
                double cx = x + 29; 
                double cy = y + 29; 

                var line = new Line // líneas de distancia
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

                var tb = new TextBlock // textos con las distancias
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

