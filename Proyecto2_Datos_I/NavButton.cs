
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

namespace SideBar_Nav
{
 
    /// Clase que define un botón de navegación (NavButton) reutilizable para un sidebar.
    /// Hereda de ListBoxItem para integrarse fácilmente con una ListBox que actúe como menú.
    public class NavButton : ListBoxItem
    {

        /// Inicializador estático de la clase.
        /// Configura el estilo por defecto del control para que busque un Style/ControlTemplate
        /// asociado al tipo NavButton (normalmente definido en Generic.xaml o recursos).
        static NavButton()
        {
            // OverrideMetadata permite reemplazar el StyleKey por defecto del control,
            // de modo que WPF aplique el template definido para NavButton.
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
        }

        /// Propiedad de dependencia que representa el destino de navegación del botón.
        /// Puede enlazarse (Binding) a un Frame, NavigationService, etc. para navegar a una página.
        public Uri NavLink
        {
            get { return (Uri)GetValue(NavLinkProperty); }
            set { SetValue(NavLinkProperty, value); }
        }

        // Registro de la DependencyProperty "NavLink".
        // Al ser DP, soporta: estilos, animaciones, binding y cambios de tema/recursos.
        public static readonly DependencyProperty NavLinkProperty =
            DependencyProperty.Register(
                "NavLink",                  // Nombre público de la propiedad
                typeof(Uri),                // Tipo almacenado
                typeof(NavButton),          // Owner (tipo que la declara)
                new PropertyMetadata(null)  // Valor por defecto y opciones de metadatos
            );

        /// Propiedad de dependencia para el ícono del botón.
        /// Permite asignar una imagen (ImageSource) que el template del control mostrará.
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Registro de la DependencyProperty "Icon".
        // Esto permite usar cualquier ImageSource (BitmapImage, DrawingImage, etc.) vía XAML o código.
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                "Icon",                     // Nombre de la propiedad
                typeof(ImageSource),        // Tipo del valor
                typeof(NavButton),          // Tipo propietario
                new PropertyMetadata(null)  // Valor por defecto (sin ícono)
            );
    }
}
