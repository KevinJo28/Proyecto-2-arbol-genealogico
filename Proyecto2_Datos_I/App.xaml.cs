using Proyecto2_Datos_I_Grafo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Proyecto2_Datos_I.Services;

namespace Proyecto2_Datos_I
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Grafo Family { get; private set; } = new Grafo();//Instancia principal del grafo

        protected override void OnStartup(StartupEventArgs e) // al iniciar la aplicación cargar el grafo desde JSON
        { 
            base.OnStartup(e); 

            // Cargar el grafo desde JSON al iniciar la aplicación 
            Family = FamilyJsonRepository.LoadFamily(); 
        } 

        protected override void OnExit(ExitEventArgs e) 
        { 
            // Guardar el grafo en JSON al cerrar la aplicación 
            FamilyJsonRepository.SaveFamily(Family); 

            base.OnExit(e); 
        } 
    }
}
