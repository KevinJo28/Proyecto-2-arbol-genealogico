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
using System.Globalization;



namespace SideBar_Nav.Pages
{
    // PÁGINA DE FORMULARIO DE NUEVO MIEMBRO
    public partial class Page4 : Page
    {
        BitmapImage? bitmapGlobal; 
        string? imagePathGlobal;

        public Page4()
        {
            InitializeComponent();
        }

        // Se ejecuta cuando el usuario arrastra algo sobre el área de Drop
        private void DropArea_DragEnter(object sender, DragEventArgs e)
        {
            // Si lo que se arrastra son archivos, mostramos el efecto de "copiar"
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                // Si no son archivos, no permitimos soltar
                e.Effects = DragDropEffects.None;
        }

        // Se ejecuta mientras el usuario sigue arrastrando sobre el área
        private void DropArea_DragOver(object sender, DragEventArgs e)
        {
            // Marcamos el evento como manejado para que el Drop funcione bien
            e.Handled = true;
        }

        // Se ejecuta cuando el usuario suelta los archivos en el área
        private void DropArea_Drop(object sender, DragEventArgs e)
        {
            // Verificamos que lo que se soltó son archivos
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Obtenemos la lista de rutas de los archivos
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Recorremos cada archivo
                foreach (string file in files)
                {
                    // Validamos que la extensión sea una imagen (jpg, png, jpeg, bmp)
                    if (System.IO.Path.GetExtension(file).ToLower() is ".jpg" or ".png" or ".jpeg" or ".bmp")
                    {
                        // Creamos un BitmapImage con la ruta del archivo
                        var bitmap = new BitmapImage(new Uri(file));

                        // Mostramos la imagen en el control Image de la interfaz
                        MyImageControl.Source = bitmap;

                        // Guardamos la imagen y la ruta en variables globales para usarlas después
                        bitmapGlobal = new BitmapImage(new Uri(file));
                        imagePathGlobal = file;
                    }
                }
            }
        }


        private void NextPage(object sender, RoutedEventArgs e)
        {
            // Validación básica de campos vacíos 
            if (string.IsNullOrWhiteSpace(this.name.Text) || 
                string.IsNullOrWhiteSpace(this.location.Text) || 
                string.IsNullOrWhiteSpace(this.birthDay.Text) || 
                string.IsNullOrWhiteSpace(this.age.Text) || 
                string.IsNullOrWhiteSpace(this.id.Text) || 
                bitmapGlobal == null || 
                string.IsNullOrWhiteSpace(imagePathGlobal)) 
            { 
                MessageBox.Show("Todos los valores tienen que estar llenos.");
                return; 
            } 

            // Validar formato de coordenadas: "x,y" en píxeles 
            var coordsText = this.location.Text.Trim(); 
            var parts = coordsText.Split(','); 
            if (parts.Length != 2 || 
                !double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) || 
                !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y)) 
            { 
                MessageBox.Show("Las coordenadas deben tener el formato \"x,y\" en píxeles. Ejemplo: 350,420"); 
                return; 
            }

            // Validar que las coordenadas estén dentro del área del mapa
            if (x < MapBounds.MinX || x > MapBounds.MaxX || y < MapBounds.MinY || y > MapBounds.MaxY) 
            {
                MessageBox.Show( // NUEVA
                    $"Las coordenadas deben estar dentro del mapa.\n" + 
                    $"X entre {MapBounds.MinX} y {MapBounds.MaxX}, Y entre {MapBounds.MinY} y {MapBounds.MaxY}."); 
                return; 
            } 

            // Validar edad e id 
            if (!int.TryParse(this.age.Text, out int age)) 
            { 
                MessageBox.Show("La edad debe ser un número entero."); 
                return; 
            } 

            if (!int.TryParse(this.id.Text, out int id)) 
            { 
                MessageBox.Show("La cédula debe ser un número entero.");
                return;
            } 

            // Validar fecha 
            if (!DateTime.TryParse(this.birthDay.Text, out DateTime birthDate)) 
            { 
                MessageBox.Show("La fecha de nacimiento no tiene un formato válido."); 
                return; 
            }
            //Agregar el miembro a la familia si paso todas las validaciones
            ((App)Application.Current).Family.Add( 
                this.name.Text,
                coordsText,
                birthDate,
                age,
                bitmapGlobal,
                id,
                imagePathGlobal
            );
            //Navegar de página
            NavigationService?.Navigate(new Uri("Pages/Page5.xaml", UriKind.Relative)); 
        }

    }
}
