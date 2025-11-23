using Proyecto2_Datos_I;
using Proyecto2_Datos_I_Grafo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page6 : Page
    {
        
        public Page6()
        {
            InitializeComponent();
            RefreshGrafo();
        }


        public void RefreshGrafo()
        {
            var ListaDePesonas = ((App)Application.Current).Family.PeopleRedOnly;
            lstPeople.Items.Clear();
            foreach (PersonNode p in ListaDePesonas) {
                lstPeople.Items.Add(p);
             
            }
        }

        public void Eliminar(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var id = btn.Tag.ToString() as string; // recuperar el id
            if (id == null) return;
            MessageBox.Show(id);
           ((App)Application.Current).Family.Remove(int.Parse(id));
            RefreshGrafo();



        }





        public void BtnAdd(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/Page4.xaml", UriKind.Relative));
        }

        public void BtnEditar(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/Page5.xaml", UriKind.Relative));
        }


    }

    
}
