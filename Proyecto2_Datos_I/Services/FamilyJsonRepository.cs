using System; 
using System.IO; 
using System.Text.Json; 
using Proyecto2_Datos_I.Dtos;
using Proyecto2_Datos_I_Grafo; 

namespace Proyecto2_Datos_I.Services 
{ 
    public static class FamilyJsonRepository 
    { 
        private static readonly string FilePath = 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "family.json");

        public static Grafo LoadFamily() // cargar el grafo desde el archivo JSON
        { 
            try 
            { 
                if (!File.Exists(FilePath)) 
                { 
                    return new Grafo(); 
                } 

                string json = File.ReadAllText(FilePath); 
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                }; 

                FamilyDto? dto = JsonSerializer.Deserialize<FamilyDto>(json, options); 
                if (dto == null)  // si el archivo está vacío o no es válido
                { 
                    return new Grafo(); // retornar un grafo vacío
                } 

                return GraphMapper.FromDto(dto); 
            } 
            catch 
            { 
                return new Grafo(); 
            } 
        } 

        public static void SaveFamily(Grafo grafo) // guardar el grafo en el archivo JSON
        { 
            try 
            { 
                var dto = GraphMapper.ToDto(grafo); 

                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                }; 

                string json = JsonSerializer.Serialize(dto, options);

                File.WriteAllText(FilePath, json);
            } 
            catch 
            { 
                
            } 
        } 
    } 
} 
