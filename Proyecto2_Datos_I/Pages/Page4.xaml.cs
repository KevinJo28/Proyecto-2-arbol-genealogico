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




namespace SideBar_Nav.Pages
{
    /// <summary>
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        BitmapImage? bitmapGlobal;

        public Page4()
        {
            InitializeComponent();
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
                        bitmapGlobal = new BitmapImage(new Uri(file));
                    }
                }
            }
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            if(this.name.Text == null || this.location.Text == null || this.birthDay.Text == null || this.age.Text == null || bitmapGlobal == null || this.id.Text == null)
            {
                MessageBox.Show("Todos los valores tiene que estar llenos");
                return;

            }
            ((App)Application.Current).Family.Add(this.name.Text, this.location.Text, DateTime.Parse(this.birthDay.Text), int.Parse(this.age.Text), bitmapGlobal, int.Parse(this.id.Text));
            NavigationService?.Navigate(new Uri("Pages/Page5.xaml", UriKind.Relative));
        }
    }
}
