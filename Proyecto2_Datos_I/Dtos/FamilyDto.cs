// Ruta sugerida: Dtos/FamilyDto.cs
using System.Collections.Generic;

namespace Proyecto2_Datos_I.Dtos
{
	public class FamilyDto
	{
		public List<PersonDto> People { get; set; } = new List<PersonDto>(); // lista de personas en la familia almacenadas en el JSON
    }
}
