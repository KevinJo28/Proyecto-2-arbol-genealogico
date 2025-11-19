using Proyecto2_Datos_I;
using Proyecto2_Datos_I_Grafo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SideBar_Nav.Pages   // 👈 MISMO NAMESPACE QUE EN EL XAML
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private readonly Grafo _family;

        public Page2()
        {
            InitializeComponent();

            // Tomamos el grafo (la “mochila” con la familia)
            _family = ((App)Application.Current).Family;

            DibujarArbol();
        }

        // Botón Agregar: te lleva de vuelta al formulario (Page4)
        public void BtnAdd(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/Page4.xaml", UriKind.Relative));
        }

        // Dibuja el árbol genealógico
        private void DibujarArbol()
        {
            TreePanel.Children.Clear();

            // IMPORTANTE: lo convertimos a lista para usar .Count sin problemas
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

            // 1. Buscar raíces (personas sin padres)
            var roots = todas.Where(p => p.Parents.Count == 0).ToList();
            if (roots.Count == 0)
            {
                // Por si acaso todas tienen padres, usamos todas como nivel 0
                roots = todas;
            }

            // 2. Construir niveles (generaciones)
            var niveles = BuildLevels(roots);

            // 3. Crear una fila por nivel
            foreach (var kv in niveles.OrderBy(k => k.Key))
            {
                int nivel = kv.Key;
                List<PersonNode> personasDelNivel = kv.Value;

                var fila = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 20)
                };

                foreach (var p in personasDelNivel)
                {
                    fila.Children.Add(CrearTarjetaPersona(p));
                }

                TreePanel.Children.Add(fila);
            }
        }

        // Agrupa personas por nivel (0 = raíces, 1 = hijos, 2 = nietos…)
        private Dictionary<int, List<PersonNode>> BuildLevels(List<PersonNode> roots)
        {
            var niveles = new Dictionary<int, List<PersonNode>>();
            var visitados = new HashSet<PersonNode>();
            var cola = new Queue<(PersonNode node, int nivel)>();

            foreach (var r in roots)
                cola.Enqueue((r, 0));

            while (cola.Count > 0)
            {
                var (node, nivel) = cola.Dequeue();
                if (!visitados.Add(node)) continue; // ya lo vimos

                if (!niveles.ContainsKey(nivel))
                    niveles[nivel] = new List<PersonNode>();

                niveles[nivel].Add(node);

                // Los hijos van al siguiente nivel
                foreach (var hijo in node.Children)
                    cola.Enqueue((hijo, nivel + 1));
            }

            return niveles;
        }

        // Tarjetita visual de una persona
        private Border CrearTarjetaPersona(PersonNode p)
        {
            var borde = new Border
            {
                BorderBrush = Brushes.DarkBlue,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10),
                Margin = new Thickness(10)
            };

            var stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Foto
            if (p.BitMap != null)
            {
                stack.Children.Add(new Image
                {
                    Source = p.BitMap,
                    Width = 80,
                    Height = 80,
                    Margin = new Thickness(0, 0, 0, 5)
                });
            }

            // Nombre
            stack.Children.Add(new TextBlock
            {
                Text = p.FullName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });

            // ID
            stack.Children.Add(new TextBlock
            {
                Text = $"ID: {p.Id}",
                FontSize = 12,
                TextAlignment = TextAlignment.Center
            });

            borde.Child = stack;
            return borde;
        }
    }
}
