using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Proyecto2_Datos_I
{
    class Grafo
    {
        private readonly Dictionary<int, PersonNode> people = new();
        public IEnumerable<PersonNode> PeopleRedOnly => people.Values;

        public void Add(string name, string coords, DateTime birthdate, int age, BitmapImage bitmap, int id)
        {
            PersonNode p = new(name, coords, birthdate, age, bitmap, id) ;
            people[p.Id] = p;

        }
        public PersonNode? Get(int id)
        {
            if (people.TryGetValue(id, out var p))
            {
                return p; // Encontró a la persona
            }
            else
            {
                return null; // No existe
            }
        }


    }

    class PersonNode
    {
        int id;
        public string fullName;
        public string coords;
        public DateTime birthDate;
        public int age;
        public BitmapImage bitMap;

        public int Id   // property
        {
            get { return id; }   // get method
        }
        public string FullName  
        {
            get { return fullName; }   
            set { fullName = value; }  
        }
        public string Coords   
        {
            get { return coords; }   
            set { coords = value; } 
        }
        public DateTime BirthDate
        {
            get { return birthDate; }
            set { birthDate = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public BitmapImage BitMap
        {
            get { return bitMap; }
            set { bitMap = value; }
        }






        public PersonNode(string name, string coords, DateTime birthdate, int age, BitmapImage bitmap, int id)
        {
            //Atributos de cada instancia
            this.fullName = name;
            this.coords = coords;
            this.birthDate = birthdate;
            this.age = age;
            this.bitMap = bitmap; //Mapa de bits de la imagen de la persona 
            this.id = id; // Cédula personal
        }

        //Desplegue de la información completa de la persona
        public void Informacion() {
            Console.WriteLine("Nombre Completo: " + fullName);
            Console.WriteLine("Coordenadas de Recidencia: " + coords);
            Console.WriteLine("Fecha de Nacimiento: " + birthDate);
            Console.WriteLine("Edad: " + age.ToString());


        }


        // Relaciones:
        public List<PersonNode> Parents { get; } = new();
        public List<PersonNode> Children { get; } = new();
        public HashSet<PersonNode> Partners { get; } = new();

    }
}
