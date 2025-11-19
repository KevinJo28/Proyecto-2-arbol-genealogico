// Ruta sugerida: Dtos/PersonDto.cs
using System;
using System.Collections.Generic;

namespace Proyecto2_Datos_I.Dtos
{
	public class PersonDto
	{
		public int Id { get; set; }

		public string FullName { get; set; } = string.Empty;

		// Coordenadas en el sistema que definas (por ejemplo "0.25,0.7" o "350,420")
		public string Coords { get; set; } = string.Empty;

		public DateTime BirthDate { get; set; }

		public int Age { get; set; }

		// Ruta de la imagen en disco o relativa al proyecto, NO el BitmapImage
		public string ImagePath { get; set; } = string.Empty;

		// Relaciones representadas por IDs
		public List<int> ParentsIds { get; set; } = new List<int>();

		public List<int> ChildrenIds { get; set; } = new List<int>();

		public List<int> PartnersIds { get; set; } = new List<int>();
	}
}
