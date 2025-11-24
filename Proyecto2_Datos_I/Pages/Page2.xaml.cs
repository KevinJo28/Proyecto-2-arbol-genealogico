using Proyecto2_Datos_I;
using Proyecto2_Datos_I_Grafo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SideBar_Nav.Pages
{
    public partial class Page2 : Page
    {
        // Grafo que contiene todas las personas y sus relaciones
        private readonly Grafo _family;

        private Canvas _treeCanvas = null!;

        // Posición asignada a cada persona en el Canvas
        private readonly Dictionary<PersonNode, Point> _nodePositions;

        // Constantes de tamaño y espaciado de las tarjetas
        private const double CARD_WIDTH = 120;
        private const double CARD_HEIGHT = 140;
        private const double HORIZONTAL_SPACING = 40;
        private const double VERTICAL_SPACING = 100;
        private const double PAREJA_SPACING = 20;

        public Page2()
        {
            InitializeComponent();

            // Se obtiene el árbol de familia que está en clase App
            _family = ((App)Application.Current).Family;

            // Diccionario donde se gurdan las posiciones calculadas
            _nodePositions = new Dictionary<PersonNode, Point>();

            // Se dibuja el árbol al cargar la página
            DibujarArbol();
        }

        // Navega a la página de agregar persona (Page4)
        public void BtnAdd(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/Page4.xaml", UriKind.Relative));
        }

        // Método principal que arma y dibuja todo el árbol
        private void DibujarArbol()
        {
            // Limpia contenido previo en el panel y las posiciones
            TreePanel.Children.Clear();
            _nodePositions.Clear();

            // Crea el Canvas donde se dibujará el árbol
            _treeCanvas = new Canvas
            {
                Background = Brushes.White,
                MinWidth = 1400,
                MinHeight = 1000
            };

            // Obtiene la lista de todas las personas
            var todas = _family.PeopleRedOnly.ToList();

            // Si no hay personas, muestra un mensaje y termina
            if (todas.Count == 0)
            {
                TreePanel.Children.Add(new TextBlock
                {
                    Text = "No hay personas en el árbol todavía.",
                    FontSize = 24
                });
                return;
            }

            // Se agrupa personas por niveles (generaciones), considerando parejas
            var niveles = BuildLevelsConParejas(todas);

            // Se calcula la posición de cada persona
            CalcularPosicionesConParejas(niveles);

            // Dibuja las líneas de parejas y padres-hijos
            DibujarTodasLasLineas();

            // Dibuja las tarjetas de cada persona
            DibujarTarjetas();

            // Agrega el Canvas final al panel de la página
            TreePanel.Children.Add(_treeCanvas);
        }

        // Construye los niveles del árbol (0 = generación superior, 1 = hijos, etc.)
        // y trata de ubicar a las parejas en el mismo nivel.
        internal static Dictionary<int, List<PersonNode>> BuildLevelsConParejas(
            IEnumerable<PersonNode> todasLasPersonasEnumerable)
        {
            var niveles = new Dictionary<int, List<PersonNode>>();
            var nivelAsignado = new Dictionary<PersonNode, int>();

            var todasLasPersonas = todasLasPersonasEnumerable.ToList();

            // Busca raíces: personas sin padres y con parejas que tampoco tengan padres
            var roots = todasLasPersonas
                .Where(p => p.Parents.Count == 0 &&
                            !p.Partners.Any(partner => partner.Parents.Count > 0))
                .ToList();

            // Si no encontra, se usan personas sin padres
            if (roots.Count == 0)
                roots = todasLasPersonas.Where(p => p.Parents.Count == 0).ToList();

            // Cola para recorrer el grafo en anchura (BFS)
            var cola = new Queue<(PersonNode node, int nivel)>();
            var visitados = new HashSet<PersonNode>();

            // Encola raíces que tengan hijos (para priorizar ramas activas)
            foreach (var root in roots)
            {
                if (root.Children.Count > 0)
                    cola.Enqueue((root, 0));
            }

            // Si ninguna raíz tenía hijos, encola todas las raíces
            if (cola.Count == 0)
            {
                foreach (var r in roots)
                    cola.Enqueue((r, 0));
            }

            // BFS: asigna niveles a partir de padres a hijos
            while (cola.Count > 0)
            {
                var (node, nivel) = cola.Dequeue();
                if (!visitados.Add(node)) continue;

                nivelAsignado[node] = nivel;

                if (!niveles.TryGetValue(nivel, out var listaNivel))
                {
                    listaNivel = new List<PersonNode>();
                    niveles[nivel] = listaNivel;
                }
                if (!listaNivel.Contains(node))
                    listaNivel.Add(node);

                // Encola los hijos al siguiente nivel
                foreach (var hijo in node.Children)
                {
                    if (!visitados.Contains(hijo))
                        cola.Enqueue((hijo, nivel + 1));
                }
            }

            // Intenta igualar el nivel de las parejas a lo largo de varios intentos
            bool cambios = true;
            int intentos = 0;

            while (cambios && intentos < 10)
            {
                cambios = false;
                intentos++;

                foreach (var persona in todasLasPersonas)
                {
                    if (!nivelAsignado.TryGetValue(persona, out int nivelPersona))
                        continue;

                    foreach (var pareja in persona.Partners)
                    {
                        // Si la pareja no tiene nivel aún, se asigna igual que a la persona
                        if (nivelAsignado.ContainsKey(pareja))
                            continue;

                        nivelAsignado[pareja] = nivelPersona;

                        if (!niveles.TryGetValue(nivelPersona, out var lista))
                        {
                            lista = new List<PersonNode>();
                            niveles[nivelPersona] = lista;
                        }

                        if (!lista.Contains(pareja))
                            lista.Add(pareja);

                        cambios = true;
                    }
                }
            }

            // Cualquier persona sin nivel asignado quedará en nivel 0 por defecto
            foreach (var persona in todasLasPersonas)
            {
                if (nivelAsignado.ContainsKey(persona))
                    continue;

                if (!niveles.TryGetValue(0, out var listaNivel0))
                {
                    listaNivel0 = new List<PersonNode>();
                    niveles[0] = listaNivel0;
                }

                if (!listaNivel0.Contains(persona))
                    listaNivel0.Add(persona);
            }

            return niveles;
        }

        // Calcula la posición de cada persona en el Canvas,
        // agrupando parejas y distribuyendo los niveles verticalmente.
        private void CalcularPosicionesConParejas(Dictionary<int, List<PersonNode>> niveles)
        {
            double yActual = 50; // Y inicial para el primer nivel

            foreach (var kv in niveles.OrderBy(k => k.Key))
            {
                var personasDelNivel = kv.Value;
                var procesados = new HashSet<PersonNode>();
                var grupos = new List<List<PersonNode>>();

                // Agrupa personas del nivel en grupos (individual o pareja)
                foreach (var persona in personasDelNivel)
                {
                    if (procesados.Contains(persona)) continue;

                    var grupo = new List<PersonNode> { persona };
                    procesados.Add(persona);

                    // Busca una pareja que también esté en este nivel y no se haya procesado
                    var parejaEnNivel = persona.Partners
                        .FirstOrDefault(p => personasDelNivel.Contains(p) && !procesados.Contains(p));

                    if (parejaEnNivel != null)
                    {
                        grupo.Add(parejaEnNivel);
                        procesados.Add(parejaEnNivel);
                    }

                    grupos.Add(grupo);
                }

                // Calcula el ancho total de todos los grupos de este nivel
                double anchoTotal = 0;
                foreach (var grupo in grupos)
                {
                    anchoTotal += grupo.Count * CARD_WIDTH;
                    if (grupo.Count > 1)
                        anchoTotal += PAREJA_SPACING;
                }
                anchoTotal += (grupos.Count - 1) * HORIZONTAL_SPACING;

                // Calcula posición inicial X para centrar el nivel
                double xInicial = Math.Max(50, (1400 - anchoTotal) / 2);
                double xActual = xInicial;

                // Asigna posición X,Y a cada persona de cada grupo
                foreach (var grupo in grupos)
                {
                    for (int i = 0; i < grupo.Count; i++)
                    {
                        _nodePositions[grupo[i]] = new Point(xActual, yActual);
                        xActual += CARD_WIDTH;

                        // Espaciado extra entre tarjetas de una misma pareja
                        if (i < grupo.Count - 1)
                            xActual += PAREJA_SPACING;
                    }
                    // Espacio entre grupos
                    xActual += HORIZONTAL_SPACING;
                }

                // Avanza al siguiente nivel vertical
                yActual += CARD_HEIGHT + VERTICAL_SPACING;
            }
        }

        // Dibuja todas las líneas de parejas y relaciones de padres-hijos
        private void DibujarTodasLasLineas()
        {
            if (_treeCanvas == null) return;

            // HashSets para evitar dibujar la misma relación dos veces
            var parejasDibujadas = new HashSet<string>();
            var relacionesPadresDibujadas = new HashSet<string>();

            foreach (var persona in _nodePositions.Keys)
            {
                // Dibujar líneas entre parejas
                foreach (var pareja in persona.Partners)
                {
                    if (_nodePositions.ContainsKey(pareja))
                    {
                        string key = GenerarKey(persona, pareja);
                        if (!parejasDibujadas.Contains(key))
                        {
                            DibujarLineaPareja(persona, pareja);
                            parejasDibujadas.Add(key);
                        }
                    }
                }

                // Dibujar líneas entre padres e hijos
                if (persona.Children.Count > 0)
                {
                    // Busca una pareja que tenga hijos en común
                    var parejaConHijos = EncontrarParejaConHijosCompartidos(persona);

                    // Key única para esta relación de padres-hijos
                    string keyRelacion = parejaConHijos != null
                        ? GenerarKey(persona, parejaConHijos)
                        : persona.Id.ToString();

                    if (!relacionesPadresDibujadas.Contains(keyRelacion))
                    {
                        DibujarLineasPadreHijos(persona, parejaConHijos);
                        relacionesPadresDibujadas.Add(keyRelacion);
                    }
                }
            }
        }

        // Devuelve la primera pareja que comparte al menos un hijo con la persona
        internal static PersonNode? EncontrarParejaConHijosCompartidos(PersonNode persona)
        {
            foreach (var pareja in persona.Partners)
            {
                if (persona.Children.Any(hijo => pareja.Children.Contains(hijo)))
                    return pareja;
            }
            return null;
        }

        // Dibuja la línea horizontal que conecta a dos parejas
        private void DibujarLineaPareja(PersonNode p1, PersonNode p2)
        {
            var pos1 = _nodePositions[p1];
            var pos2 = _nodePositions[p2];

            // Determina quién está a la izquierda y quién a la derecha
            var izq = pos1.X < pos2.X ? pos1 : pos2;
            var der = pos1.X < pos2.X ? pos2 : pos1;

            // Altura media de la tarjeta
            double y = izq.Y + CARD_HEIGHT / 2;
            // Línea desde el borde derecho de la tarjeta izquierda al borde izquierdo de la derecha
            double x1 = izq.X + CARD_WIDTH;
            double x2 = der.X;

            _treeCanvas.Children.Add(new Line
            {
                X1 = x1,
                Y1 = y,
                X2 = x2,
                Y2 = y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });
        }

        // Dibuja las líneas desde los padres (o padre solo) hasta sus hijos
        private void DibujarLineasPadreHijos(PersonNode padre, PersonNode? pareja)
        {
            if (!_nodePositions.ContainsKey(padre)) return;

            var posPadre = _nodePositions[padre];
            // Toma solo hijos que tienen posición en el árbol
            var todosLosHijos = padre.Children.Where(h => _nodePositions.ContainsKey(h)).ToList();

            if (todosLosHijos.Count == 0) return;

            List<PersonNode> hijos;
            if (pareja != null && _nodePositions.ContainsKey(pareja))
            {
                // Si hay pareja, usa solo los hijos que comparten
                hijos = todosLosHijos.Where(h => pareja.Children.Contains(h)).ToList();
                if (hijos.Count == 0) return;
            }
            else
            {
                // Si no hay pareja, usa todos los hijos que tenga el padre
                hijos = todosLosHijos;
            }

            double xInicio, yInicio;

            if (pareja != null && _nodePositions.ContainsKey(pareja))
            {
                // Punto medio entre las tarjetas de los padres
                var posPareja = _nodePositions[pareja];
                xInicio = (posPadre.X + posPareja.X + CARD_WIDTH) / 2;
                yInicio = posPadre.Y + CARD_HEIGHT / 2;
            }
            else
            {
                // Padre solo: se usa el centro inferior de su tarjeta
                xInicio = posPadre.X + CARD_WIDTH / 2;
                yInicio = posPadre.Y + CARD_HEIGHT;
            }

            // Toma la posición del primer hijo para calcular la altura de la barra horizontal
            var primeraPos = _nodePositions[hijos[0]];
            double yInterseccion = primeraPos.Y - 30;

            // Línea vertical desde los padres hasta la barra horizontal
            _treeCanvas.Children.Add(new Line
            {
                X1 = xInicio,
                Y1 = yInicio,
                X2 = xInicio,
                Y2 = yInterseccion,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

            // Calcula el rango horizontal donde están los hijos
            var posicionesHijos = hijos.Select(h => _nodePositions[h].X + CARD_WIDTH / 2).ToList();
            double minX = posicionesHijos.Min();
            double maxX = posicionesHijos.Max();

            // Si solo hay un hijo, la barra se reduce a un punto
            double x1Horizontal = hijos.Count == 1 ? xInicio : minX;
            double x2Horizontal = hijos.Count == 1 ? minX : maxX;

            // Barra horizontal sobre los hijos
            _treeCanvas.Children.Add(new Line
            {
                X1 = x1Horizontal,
                Y1 = yInterseccion,
                X2 = x2Horizontal,
                Y2 = yInterseccion,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

            // Baja una línea vertical desde la barra hasta cada hijo
            foreach (var hijo in hijos)
            {
                var posHijo = _nodePositions[hijo];
                double xHijo = posHijo.X + CARD_WIDTH / 2;
                double yHijo = posHijo.Y;

                _treeCanvas.Children.Add(new Line
                {
                    X1 = xHijo,
                    Y1 = yInterseccion,
                    X2 = xHijo,
                    Y2 = yHijo,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                });
            }
        }

        // Crea y dibuja las tarjetas de todas las personas según sus posiciones
        private void DibujarTarjetas()
        {
            if (_treeCanvas == null) return;

            foreach (var kv in _nodePositions)
            {
                var persona = kv.Key;
                var pos = kv.Value;

                var tarjeta = CrearTarjetaPersona(persona);
                Canvas.SetLeft(tarjeta, pos.X);
                Canvas.SetTop(tarjeta, pos.Y);

                _treeCanvas.Children.Add(tarjeta);
            }
        }

        // Crea la tarjeta visual de una persona (borde, imagen, nombre, ID)
        private Border CrearTarjetaPersona(PersonNode p)
        {
            var borde = new Border
            {
                BorderBrush = Brushes.DarkBlue,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Width = CARD_WIDTH,
                Height = CARD_HEIGHT,
                Background = Brushes.White
            };

            var stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Agrega imagen si la persona tiene BitMap
            if (p.BitMap != null)
            {
                stack.Children.Add(new Image
                {
                    Source = p.BitMap,
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(0, 0, 0, 5)
                });
            }

            // Nombre completo en negrita
            stack.Children.Add(new TextBlock
            {
                Text = p.FullName,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            });

            // ID de la persona (ayuda a identificar en la lógica interna)
            stack.Children.Add(new TextBlock
            {
                Text = $"ID: {p.Id}",
                FontSize = 10,
                TextAlignment = TextAlignment.Center
            });

            borde.Child = stack;
            return borde;
        }

        // Genera una clave única para una pareja, independiente del orden
        internal static string GenerarKey(PersonNode p1, PersonNode p2)
        {
            var ids = new[] { p1.Id, p2.Id }.OrderBy(x => x);
            return string.Join("-", ids);
        }
    }
}
