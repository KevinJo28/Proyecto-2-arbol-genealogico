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
    //Clase que representa el segundo form para definir parentesco

    public partial class Page5 : Page
    {
        public Page5()
        {
            InitializeComponent();
            FillCombo(); // Cargar las listas al abrir la página
        }



        // Botón "Siguiente": enlaza relaciones y navega si todo está bien
        private void NextPage2(object sender, RoutedEventArgs e)
        {
            var Grafo = ((App)Application.Current).Family;

            // Obtener las selecciones de los combos
            PersonNode? madre = this.cmbPersonas.SelectedItem as PersonNode;
            PersonNode? padre = this.cmbPersonas2.SelectedItem as PersonNode;
            PersonNode? pareja = this.cmbPersonas3.SelectedItem as PersonNode;
            PersonNode? me = this.cmbPersonas4.SelectedItem as PersonNode;

            // Nombres para mostrar en el mensaje
            string madreName = string.Empty;
            string padreName = string.Empty;
            string parejaName = string.Empty;

            // Enlazar madre/padre/pareja con "Yo" si están seleccionados
            if (madre != null && me != null) { Grafo.LinkParentChild(madre, me); madreName = madre.FullName; }
            if (padre != null && me != null) { Grafo.LinkParentChild(padre, me); padreName = padre.FullName; }
            if (pareja != null && me != null) { Grafo.LinkPartners(pareja, me); parejaName = pareja.FullName; }

            // Validación básica: nadie puede ser su propia madre/padre/pareja
            if (madreName == me?.FullName || padreName == me?.FullName || parejaName == me?.FullName)
            {
                MessageBox.Show("Una persona no puede ser el mismo su pareja, madre o padre");
                this.cmbPersonas.SelectedItem = null;
                this.cmbPersonas2.SelectedItem = null;
                this.cmbPersonas3.SelectedItem = null;
            }
            else if (me != null)
            {
                // Todo bien: confirmar y pasar a la siguiente página
                MessageBox.Show(
                    "Se ha agregado a " + me.FullName + " satisfactoriamente \n" +
                    "Padre: " + padreName + "\n" +
                    "Madre: " + madreName + "\n" +
                    "Pareja: " + parejaName
                );

                NavigationService?.Navigate(new Uri("Pages/Page6.xaml", UriKind.Relative));
            }
            else
            {
                // Falta seleccionar "Yo"
                MessageBox.Show("La casilla de 'Yo' tiene que estar llena");
            }
        }

        // Cargar los combos con la lista de personas 
        public void FillCombo()
        {
            // Obtener la lista de personas desde la app
            var listaDePersonas = ((App)Application.Current).Family.PeopleRedOnly;

            // Copiar a una lista para enlazar por seguridad
            List<PersonNode> personNodes = new List<PersonNode>();
            foreach (PersonNode node in listaDePersonas)
            {
                personNodes.Add(node);
            }

            // Agrupar los ComboBox en una colección para configurarlos con un solo loop
            var combos = new[] { cmbPersonas, cmbPersonas2, cmbPersonas3, cmbPersonas4 };

            // Configurar todos los combos de la misma manera
            foreach (var combo in combos)
            {
                combo.SelectedValuePath = "Id";       // Valor seleccionado será el Id
                combo.DisplayMemberPath = "FullName"; // Texto mostrado será el nombre completo
                combo.ItemsSource = null;             // Limpiar primero
                combo.ItemsSource = personNodes;      // Asignar la lista de personas
            }
        }
    }

}
