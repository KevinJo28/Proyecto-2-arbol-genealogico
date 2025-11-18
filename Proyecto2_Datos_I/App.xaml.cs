using Proyecto2_Datos_I_Grafo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Proyecto2_Datos_I
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Grafo Family { get; } = new Grafo();
    }
}
