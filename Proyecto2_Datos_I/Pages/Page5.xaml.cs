using Proyecto2_Datos_I;
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

namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page5 : Page
    {


        public Page5()
        {
            InitializeComponent();
            FillCombo();


        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void DropArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void DropArea_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true; // Necesario para que Drop funcione correctamente
        }

        private void DropArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file).ToLower() is ".jpg" or ".png" or ".jpeg" or ".bmp")
                    {
                        // Cargar imagen en un Image control
                        var bitmap = new BitmapImage(new Uri(file));
                        MyImageControl.Source = bitmap;
                    }
                }
            }
        }

        private void NextPage2(object sender, RoutedEventArgs e)
        {
            var Grafo = ((App)Application.Current).Family;
            PersonNode? madre = this.cmbPersonas.SelectedItem as PersonNode;
            PersonNode? padre = this.cmbPersonas2.SelectedItem as PersonNode;
            PersonNode? pareja = this.cmbPersonas3.SelectedItem as PersonNode;
            PersonNode? me = this.cmbPersonas4.SelectedItem as PersonNode;
            string madreName = string.Empty;
            string padreName = string.Empty;
            string parejaName = string.Empty;


            if (madre != null && me != null) { Grafo.LinkParentChild(madre, me); madreName = madre.FullName; }
            if (padre != null && me != null) { Grafo.LinkParentChild(padre, me); padreName = padre.FullName; }
            if (pareja != null && me != null) {Grafo.LinkPartners(pareja, me); parejaName = pareja.FullName; }
            if (me != null)
            {
                MessageBox.Show("Se ha agregado a " + me.FullName + " satisfactoriamente \n" + "Padre: " + padreName + "\n Madre: " + madreName + "\n Pareja: " + parejaName);
                NavigationService?.Navigate(new Uri("Pages/Page4.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("La casilla de 'Yo' tiene que estar llena");

            }
            




        }

        public void FillCombo()
        {
            var ListaDePesonas = ((App)Application.Current).Family.PeopleRedOnly;
            List<PersonNode> personNodes = new List<PersonNode>();
            foreach(PersonNode node in ListaDePesonas)
            {
                personNodes.Add(node);
            }
            this.cmbPersonas.SelectedValuePath = "Id";
            this.cmbPersonas.DisplayMemberPath = "FullName";
            this.cmbPersonas.ItemsSource = null;
            this.cmbPersonas.ItemsSource = personNodes;

            this.cmbPersonas2.SelectedValuePath = "Id";
            this.cmbPersonas2.DisplayMemberPath = "FullName";
            this.cmbPersonas2.ItemsSource = null;
            this.cmbPersonas2.ItemsSource = personNodes;

            this.cmbPersonas3.SelectedValuePath = "Id";
            this.cmbPersonas3.DisplayMemberPath = "FullName";
            this.cmbPersonas3.ItemsSource = null;
            this.cmbPersonas3.ItemsSource = personNodes;

            this.cmbPersonas4.SelectedValuePath = "Id";
            this.cmbPersonas4.DisplayMemberPath = "FullName";
            this.cmbPersonas4.ItemsSource = null;
            this.cmbPersonas4.ItemsSource = personNodes;


        }
    }
}
