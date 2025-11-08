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

   
    public class NavButton : ListBoxItem
    {
        static NavButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
        }


        public Uri NavLink
        {
            get { return (Uri)GetValue(NavLinkProperty); }
            set { SetValue(NavLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavLinkProperty = DependencyProperty.Register("NavLink", typeof(Uri), typeof(NavButton), new PropertyMetadata(null));



        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NavButton), new PropertyMetadata(null));





    }
}
