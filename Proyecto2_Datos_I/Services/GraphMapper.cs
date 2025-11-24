using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Proyecto2_Datos_I.Dtos;
using Proyecto2_Datos_I_Grafo;

namespace Proyecto2_Datos_I.Services
{
    public static class GraphMapper
    {
        // grafo a DTO para guardar en JSON
        public static FamilyDto ToDto(Grafo grafo)
        {
            var familyDto = new FamilyDto();

            foreach (var person in grafo.PeopleRedOnly)
            {
                var dto = new PersonDto
                {
                    Id = person.Id,
                    FullName = person.FullName,
                    Coords = person.Coords,
                    BirthDate = person.BirthDate,
                    Age = person.Age,
                    ImagePath = person.ImagePath ?? string.Empty,
                    ParentsIds = person.Parents.Select(p => p.Id).Distinct().ToList(),
                    ChildrenIds = person.Children.Select(c => c.Id).Distinct().ToList(),
                    PartnersIds = person.Partners.Select(pa => pa.Id).Distinct().ToList()
                };

                familyDto.People.Add(dto);
            }

            return familyDto;
        }

        // DTO a grafo para cargar desde JSON
        public static Grafo FromDto(FamilyDto familyDto)
        {
            var grafo = new Grafo();

            // personas sin relaciones primero
            var idToNode = new Dictionary<int, PersonNode>();

            foreach (var dto in familyDto.People)
            {
                BitmapImage bitmap = null;

                if (!string.IsNullOrWhiteSpace(dto.ImagePath))
                {
                    try
                    {
                        var uri = new Uri(dto.ImagePath, UriKind.RelativeOrAbsolute);
                        bitmap = new BitmapImage(uri);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                    }
                }

                grafo.Add(
                    dto.FullName,
                    dto.Coords,
                    dto.BirthDate,
                    dto.Age,
                    bitmap,
                    dto.Id,
                    dto.ImagePath
                );

                var node = grafo.Get(dto.Id);
                if (node != null)
                {
                    idToNode[dto.Id] = node;
                }
            }

            // reconstruir relaciones
            foreach (var dto in familyDto.People)
            {
                if (!idToNode.TryGetValue(dto.Id, out var me))
                    continue;

                // padres
                foreach (var parentId in dto.ParentsIds)
                {
                    if (idToNode.TryGetValue(parentId, out var parent))
                    {
                        // máximo 2 padres
                        grafo.LinkParentChild(parent, me, onlyTwoParents: false);
                    }
                }

                // parejas
                foreach (var partnerId in dto.PartnersIds)
                {
                    if (idToNode.TryGetValue(partnerId, out var partner))
                    {
                        grafo.LinkPartners(me, partner);
                    }
                }
            }

            return grafo;
        }
    }
}
