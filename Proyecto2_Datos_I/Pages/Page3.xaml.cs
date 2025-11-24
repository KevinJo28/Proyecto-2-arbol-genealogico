using System; 
using System.Collections.Generic; 
using System.Globalization; 
using System.Linq; 
using System.Windows;
using System.Windows.Controls;
using Proyecto2_Datos_I; 
using Proyecto2_Datos_I_Grafo; 

namespace SideBar_Nav.Pages
{
    // PÁGINA DE ESTADÍSTICAS 
    public partial class Page3 : Page
    {
        public Page3()
        {
            InitializeComponent(); 
            Loaded += Page3_Loaded; 
        }

        private void Page3_Loaded(object sender, RoutedEventArgs e) 
        { 
            CalcularEstadisticas(); // calcula las estadísticas al cargar la página
        }

        // Método para calcular estadísticas de distancias entre familiares
        private void CalcularEstadisticas() 
        { 
            var app = (App)Application.Current; 
            var grafo = app.Family; 

            //obtiene lista de personas con coordenadas válidas dentro del mapa 
            var personasConCoords = new List<(PersonNode person, double x, double y)>(); 

            foreach (var p in grafo.PeopleRedOnly) 
            { 
                if (TryGetCoords(p, out double x, out double y)) 
                { 
                    personasConCoords.Add((p, x, y)); 
                } 
            } 

            int n = personasConCoords.Count; 

            SummaryPeopleCountText.Text = $"Familiares considerados: {n}"; 

            if (n < 2) 
            { 
                // validación para menos de 2 personas 
                FarthestPairNamesText.Text = "No disponible"; 
                FarthestPairDistanceText.Text = "—"; 
                ClosestPairNamesText.Text = "No disponible"; 
                ClosestPairDistanceText.Text = "—"; 
                AverageDistanceText.Text = "—"; 
                SummaryPairsCountText.Text = "Pares de familiares considerados: 0"; 
                SummaryWarningText.Text = "No hay suficientes familiares con coordenadas válidas para calcular estadísticas."; 
                return; 
            } 

            // recorre todas las parejas (i, j) con i < j 
            double minDist = double.MaxValue; 
            double maxDist = double.MinValue; 
            PersonNode? minA = null, minB = null; 
            PersonNode? maxA = null, maxB = null; 
            double sumaDistancias = 0.0; 
            int conteoPares = 0; 

            for (int i = 0; i < n; i++) 
            { 
                for (int j = i + 1; j < n; j++)
                { 
                    var (p1, x1, y1) = personasConCoords[i]; 
                    var (p2, x2, y2) = personasConCoords[j]; 

                    double dx = x2 - x1; 
                    double dy = y2 - y1; 
                    double d = Math.Sqrt(dx * dx + dy * dy); // calcula la distancia euclidiana

                    // validación para personas en el mismo punto
                    if (d <= 0) 
                        continue;

                    // datos para el promedio
                    sumaDistancias += d; 
                    conteoPares++;

                    // cálculo de mínimo 
                    if (d < minDist) 
                    { 
                        minDist = d; 
                        minA = p1; 
                        minB = p2; 
                    }

                    // cálculo de máximo
                    if (d > maxDist) 
                    { 
                        maxDist = d; 
                        maxA = p1; 
                        maxB = p2; 
                    } 
                } 
            }

            // validación para el caso de todas las distancias cero
            if (conteoPares == 0) 
            { 
                FarthestPairNamesText.Text = "No disponible"; 
                FarthestPairDistanceText.Text = "—"; 
                ClosestPairNamesText.Text = "No disponible"; 
                ClosestPairDistanceText.Text = "—"; 
                AverageDistanceText.Text = "—"; 
                SummaryPairsCountText.Text = "Pares de familiares considerados: 0"; 
                SummaryWarningText.Text = "Todas las coordenadas válidas corresponden a puntos coincidentes (distancia cero)."; 
                return; 
            } 

            // cálculo del promedio
            double promedio = sumaDistancias / conteoPares; 

            SummaryPairsCountText.Text = $"Pares de familiares considerados: {conteoPares}"; 
            SummaryWarningText.Text = "(Las distancias se calculan con base en las coordenadas de pixeles del mapa, no en kilómetros reales)."; 

            // muestra los resultado en los cuadritos
            if (maxA != null && maxB != null) 
            { 
                FarthestPairNamesText.Text = $"{maxA.FullName} — {maxB.FullName}"; 
                FarthestPairDistanceText.Text = $"{maxDist:0} km"; 
            } 
            else 
            { 
                FarthestPairNamesText.Text = "No disponible"; 
                FarthestPairDistanceText.Text = "—"; 
            } 

            if (minA != null && minB != null) 
            { 
                ClosestPairNamesText.Text = $"{minA.FullName} — {minB.FullName}"; 
                ClosestPairDistanceText.Text = $"{minDist:0} km"; 
            } 
            else 
            { 
                ClosestPairNamesText.Text = "No disponible"; 
                ClosestPairDistanceText.Text = "—"; 
            } 

            AverageDistanceText.Text = $"{promedio:0} km"; 
        }

        // Método para obtener coordenadas de una persona
        private bool TryGetCoords(PersonNode person, out double x, out double y) 
        { 
            x = 0; y = 0; 

            if (string.IsNullOrWhiteSpace(person.Coords)) 
                return false; 

            var parts = person.Coords.Split(','); 
            if (parts.Length != 2) 
                return false; // valida formato

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x)) 
                return false;  // valida que sea float

            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y)) 
                return false; // valida que sea float

            // mismos límites del mapa
            if (x < MapBounds.MinX || x > MapBounds.MaxX || y < MapBounds.MinY || y > MapBounds.MaxY) 
                return false; 

            return true; 
        } 
    }
}
