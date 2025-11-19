using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Proyecto2_Datos_I_Grafo
{
    public class Grafo
    {
        //Diccionario de nodos relacionando cada uno con su id
        private readonly Dictionary<int, PersonNode> people = new();
 
        //Lista de nodos para poder utilizar en otras partes (solo se puede ver no se puede editar desde afuera)
        public IEnumerable<PersonNode> PeopleRedOnly => people.Values;

        public void Add(string name, string coords, DateTime birthdate, int age, BitmapImage bitmap, int id, string imagePath)
        {
            PersonNode p = new(name, coords, birthdate, age, bitmap, id, imagePath) ;
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

        //Método para enlazar hijos y padres, recibe el nodo del padre y el del hijo. Esta solo para poder acceptar dos padres por hijo.
        public bool LinkParentChild(PersonNode parent, PersonNode child, bool onlyTwoParents = true)
        {
            if (parent == null || child == null)  return  false; 
            if (ReferenceEquals(parent, child)) return false;//throw new InvalidOperationException("Una persona no puede ser su propio padre/hijo.");
            if (DetectionCycle(parent, child)) return false; //throw new InvalidOperationException("El enlace crearía un ciclo en el árbol genealógico.");
            if (onlyTwoParents && child.Parents.Count >= 2)
                throw new InvalidOperationException("Este hijo ya tiene 2 padres asignados.");

            if (!parent.Children.Contains(child)) parent.Children.Add(child);
            if (!child.Parents.Contains(parent)) child.Parents.Add(parent);
            return true;
        }

        //Método para enlazar parejas
        public bool LinkPartners(PersonNode a, PersonNode b)
        {

            if (ReferenceEquals(a, b)) return false; //throw new InvalidOperationException("Una persona no puede ser su propia pareja.");
            a.Partners.Add(b);
            b.Partners.Add(a);
            return true;
        }

        private bool DetectionCycle(PersonNode parent, PersonNode child)
        {
            // ¿Existe un camino child -> ... -> parent?
            var visited = new HashSet<int>();
            var stack = new Stack<PersonNode>();
            stack.Push(child);

            while (stack.Count > 0)
            {
                var u = stack.Pop();
                if (!visited.Add(u.Id)) continue;
                if (ReferenceEquals(u, parent)) return true;
                foreach (var c in u.Children)
                    stack.Push(c);
            }
            return false;
        }


    }

    public class PersonNode
    {
        int id;
        public string fullName;
        public string coords;
        public DateTime birthDate;
        public int age;
        public BitmapImage bitMap;
        public string ImagePath { get; set; } = string.Empty; // ruta de la imagen

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





        //Constructor
        public PersonNode(string name, string coords, DateTime birthdate, int age, BitmapImage bitmap, int id, string imagePath)
        {
            //Atributos de cada instancia
            this.fullName = name;
            this.coords = coords;
            this.birthDate = birthdate;
            this.age = age;
            this.bitMap = bitmap; //Mapa de bits de la imagen de la persona 
            this.id = id; // Cédula personal
            this.ImagePath = imagePath ?? string.Empty; // ruta de la imagen
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
        public HashSet<PersonNode> Partners { get; } = new(); //Se usa un HasSet ya que no es necesario una lista al ser una relación solo de dos personas

    }
}
