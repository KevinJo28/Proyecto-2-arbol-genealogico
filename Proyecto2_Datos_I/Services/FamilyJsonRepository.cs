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

        public static Grafo LoadFamily()
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

                return GraphMapper.FromDto(dto); 
            } 
            catch 
            { 
                return new Grafo(); 
            } 
        } 

        public static void SaveFamily(Grafo grafo)
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
                System.Diagnostics.Debug.WriteLine($"Error saving: {ex.Message}");
                MessageBox.Show("Error al guardar el archivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            } 
        } 
    } 
} 
