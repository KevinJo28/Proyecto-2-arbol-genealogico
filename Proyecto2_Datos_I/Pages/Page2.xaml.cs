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
        private readonly Grafo _family;
        private Canvas _treeCanvas = null!;
        private Dictionary<PersonNode, Point> _nodePositions;
        private const double CARD_WIDTH = 120;
        private const double CARD_HEIGHT = 140;
        private const double HORIZONTAL_SPACING = 40;
        private const double VERTICAL_SPACING = 100;
        private const double PAREJA_SPACING = 20;

        public Page2()
        {
            InitializeComponent();
            _family = ((App)Application.Current).Family;
            _nodePositions = new Dictionary<PersonNode, Point>();
            DibujarArbol();
        }

        public void BtnAdd(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/Page4.xaml", UriKind.Relative));
        }

        private void DibujarArbol()
        {
            TreePanel.Children.Clear();
            _nodePositions.Clear();

            _treeCanvas = new Canvas
            {
                Background = Brushes.White,
                MinWidth = 1400,
                MinHeight = 1000
            };

            var todas = _family.PeopleRedOnly.ToList();

            if (todas.Count == 0)
            {
                TreePanel.Children.Add(new TextBlock
                {
                    Text = "No hay personas en el árbol todavía.",
                    FontSize = 24
                });
                return;
            }

            var roots = todas.Where(p => p.Parents.Count == 0).ToList();

            var niveles = BuildLevelsConParejas(roots);
            CalcularPosicionesConParejas(niveles);

            DibujarTodasLasLineas();
            DibujarTarjetas();

            TreePanel.Children.Add(_treeCanvas);
        }

        private Dictionary<int, List<PersonNode>> BuildLevelsConParejas(List<PersonNode> rootsOriginal)
        {
            var niveles = new Dictionary<int, List<PersonNode>>();
            var nivelAsignado = new Dictionary<PersonNode, int>();

            var todasLasPersonas = _family.PeopleRedOnly.ToList();

            
            var roots = todasLasPersonas
                .Where(p => p.Parents.Count == 0 &&
                        !p.Partners.Any(partner => partner.Parents.Count > 0))
                .ToList();

            if (roots.Count == 0)
                roots = todasLasPersonas.Where(p => p.Parents.Count == 0).ToList();

            var cola = new Queue<(PersonNode node, int nivel)>();
            var visitados = new HashSet<PersonNode>();

            foreach (var root in roots.Where(r => r.Children.Count > 0))
                cola.Enqueue((root, 0));

            if (cola.Count == 0)
            {
                foreach (var r in roots)
                    cola.Enqueue((r, 0));
            }

            while (cola.Count > 0)
            {
                var (node, nivel) = cola.Dequeue();
                if (!visitados.Add(node)) continue;

                nivelAsignado[node] = nivel;

                if (!niveles.ContainsKey(nivel))
                    niveles[nivel] = new List<PersonNode>();
                if (!niveles[nivel].Contains(node))
                    niveles[nivel].Add(node);

                foreach (var hijo in node.Children)
                {
                    if (!visitados.Contains(hijo))
                        cola.Enqueue((hijo, nivel + 1));
                }
            }

            bool cambios = true;
            int intentos = 0;

            while (cambios && intentos < 10)
            {
                cambios = false;
                intentos++;

                foreach (var persona in todasLasPersonas)
                {
                    if (!nivelAsignado.ContainsKey(persona))
                        continue;

                    int nivelPersona = nivelAsignado[persona];

                    foreach (var pareja in persona.Partners)
                    {
                        if (!nivelAsignado.ContainsKey(pareja))
                        {
                            nivelAsignado[pareja] = nivelPersona;

                            if (!niveles.ContainsKey(nivelPersona))
                                niveles[nivelPersona] = new List<PersonNode>();

                            if (!niveles[nivelPersona].Contains(pareja))
                                niveles[nivelPersona].Add(pareja);

                            cambios = true;
                        }
                    }
                }
            }

            foreach (var persona in todasLasPersonas)
            {
                if (!nivelAsignado.ContainsKey(persona))
                {
                    nivelAsignado[persona] = 0;

                    if (!niveles.ContainsKey(0))
                        niveles[0] = new List<PersonNode>();

                    if (!niveles[0].Contains(persona))
                        niveles[0].Add(persona);
                }
            }

            return niveles;
        }

        //---------------------------------------------------------------------

        private void CalcularPosicionesConParejas(Dictionary<int, List<PersonNode>> niveles)
        {
            double yActual = 50;

            foreach (var kv in niveles.OrderBy(k => k.Key))
            {
                var personasDelNivel = kv.Value;
                var procesados = new HashSet<PersonNode>();
                var grupos = new List<List<PersonNode>>();

                foreach (var persona in personasDelNivel)
                {
                    if (procesados.Contains(persona)) continue;

                    var grupo = new List<PersonNode> { persona };
                    procesados.Add(persona);

                    var parejaEnNivel = persona.Partners
                        .FirstOrDefault(p => personasDelNivel.Contains(p) && !procesados.Contains(p));

                    if (parejaEnNivel != null)
                    {
                        grupo.Add(parejaEnNivel);
                        procesados.Add(parejaEnNivel);
                    }

                    grupos.Add(grupo);
                }

                double anchoTotal = 0;
                foreach (var grupo in grupos)
                {
                    anchoTotal += grupo.Count * CARD_WIDTH;
                    if (grupo.Count > 1)
                        anchoTotal += PAREJA_SPACING;
                }
                anchoTotal += (grupos.Count - 1) * HORIZONTAL_SPACING;

                double xInicial = Math.Max(50, (1400 - anchoTotal) / 2);
                double xActual = xInicial;

                foreach (var grupo in grupos)
                {
                    for (int i = 0; i < grupo.Count; i++)
                    {
                        _nodePositions[grupo[i]] = new Point(xActual, yActual);
                        xActual += CARD_WIDTH;

                        if (i < grupo.Count - 1)
                            xActual += PAREJA_SPACING;
                    }
                    xActual += HORIZONTAL_SPACING;
                }

                yActual += CARD_HEIGHT + VERTICAL_SPACING;
            }
        }
        //---------------------------------------------------------------------
        private void DibujarTodasLasLineas()
        {
            if (_treeCanvas == null) return;

            var parejasDibujadas = new HashSet<string>();
            var relacionesPadresDibujadas = new HashSet<string>();

            foreach (var persona in _nodePositions.Keys)
            {
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

                if (persona.Children.Count > 0)
                {
                    var parejaConHijos = EncontrarParejaConHijosCompartidos(persona);
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
        //---------------------------------------------------------------------
        private PersonNode? EncontrarParejaConHijosCompartidos(PersonNode persona)
        {
            foreach (var pareja in persona.Partners)
            {
                if (_nodePositions.ContainsKey(pareja) &&
                    persona.Children.Any(hijo => pareja.Children.Contains(hijo)))
                {
                    return pareja;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        private void DibujarLineaPareja(PersonNode p1, PersonNode p2)
        {
            var pos1 = _nodePositions[p1];
            var pos2 = _nodePositions[p2];

            var izq = pos1.X < pos2.X ? pos1 : pos2;
            var der = pos1.X < pos2.X ? pos2 : pos1;

            double y = izq.Y + CARD_HEIGHT / 2;
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
        //---------------------------------------------------------------------
        private void DibujarLineasPadreHijos(PersonNode padre, PersonNode? pareja)
        {
            if (!_nodePositions.ContainsKey(padre)) return;

            var posPadre = _nodePositions[padre];
            var todosLosHijos = padre.Children.Where(h => _nodePositions.ContainsKey(h)).ToList();

            if (todosLosHijos.Count == 0) return;

            List<PersonNode> hijos;
            if (pareja != null && _nodePositions.ContainsKey(pareja))
            {
                hijos = todosLosHijos.Where(h => pareja.Children.Contains(h)).ToList();
                if (hijos.Count == 0) return;
            }
            else
            {
                hijos = todosLosHijos;
            }

            double xInicio, yInicio;

            if (pareja != null && _nodePositions.ContainsKey(pareja))
            {
                var posPareja = _nodePositions[pareja];
                xInicio = (posPadre.X + posPareja.X + CARD_WIDTH) / 2;
                yInicio = posPadre.Y + CARD_HEIGHT / 2;
            }
            else
            {
                xInicio = posPadre.X + CARD_WIDTH / 2;
                yInicio = posPadre.Y + CARD_HEIGHT;
            }

            var primeraPos = _nodePositions[hijos[0]];
            double yInterseccion = primeraPos.Y - 30;

            _treeCanvas.Children.Add(new Line
            {
                X1 = xInicio,
                Y1 = yInicio,
                X2 = xInicio,
                Y2 = yInterseccion,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

            var posicionesHijos = hijos.Select(h => _nodePositions[h].X + CARD_WIDTH / 2).ToList();
            double minX = posicionesHijos.Min();
            double maxX = posicionesHijos.Max();

            double x1Horizontal = hijos.Count == 1 ? xInicio : minX;
            double x2Horizontal = hijos.Count == 1 ? minX : maxX;

            _treeCanvas.Children.Add(new Line
            {
                X1 = x1Horizontal,
                Y1 = yInterseccion,
                X2 = x2Horizontal,
                Y2 = yInterseccion,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

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
        //---------------------------------------------------------------------
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
        //---------------------------------------------------------------------
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

            stack.Children.Add(new TextBlock
            {
                Text = p.FullName,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"ID: {p.Id}",
                FontSize = 10,
                TextAlignment = TextAlignment.Center
            });

            borde.Child = stack;
            return borde;
        }
        //---------------------------------------------------------------------
        private string GenerarKey(PersonNode p1, PersonNode p2)
        {
            var ids = new[] { p1.Id, p2.Id }.OrderBy(x => x);
            return string.Join("-", ids);
        }
    }
}
