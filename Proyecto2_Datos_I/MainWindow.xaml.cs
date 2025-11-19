using SideBar_Nav;
using System;
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
using Proyecto2_Datos_I_Grafo;


namespace Proyecto2_Datos_I
{
    public partial class MainWindow : Window
    {
    
        public Grafo Family { get; } = new Grafo();

        public MainWindow()
        {

            InitializeComponent();
            navframe.Navigate(new Uri("Pages/Page6.xaml", UriKind.RelativeOrAbsolute));

        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = sidebar.SelectedItem as NavButton;

            navframe.Navigate(selected?.NavLink);
        }
        private void Imagen_Click(object sender, MouseButtonEventArgs e)
        {
            navframe.Navigate(new Uri("Pages/Page6.xaml", UriKind.RelativeOrAbsolute));
          


        }
    }
}
