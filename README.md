# Proyecto II CE1103: Árbol Genealógico

Este proyecto es una aplicación WPF desarrollada en C# cuyo objetivo principal es gestionar y visualizar un árbol genealógico interactivo, que permite al usuario agregar familiares, verlos ubicados en un mapa y obtener estadísticas útiles sobre sus distancias relativas.

Fue desarrollado como solución al Proyecto II del curso de Algoritmos y Estructuras de Datos I (CE1103) de la carrera de Ingeniería en Computadores del Tecnológico de Costa Rica.

---

## Funcionalidades principales

### 1. **Gestión de familiares**
- Registro de familiares con:
  - Nombre completo  
  - Fecha de nacimiento  
  - Padres   
  - Pareja  
  - Foto de perfil 
  - Coordenadas en el mapa  

Todos los datos se almacenan de manera persistente en un archivo JSON.

---

### 2. **Mapa interactivo**
- Muestra a cada familiar como un marcador circular sobre un mapa.
- Permite seleccionar una persona para ver sus distancias respecto a los demás.
- Dibuja líneas hacia cada familiar y muestra las distancias.


---

### 3. **Estadísticas**
Página dedicada a mostrar:
- Par de familiares más cercano entre sí.  
- Par de familiares más lejano entre sí.  
- Distancia promedio entre pares de familiares.

Las estadísticas se presentan en un dashboard con cuadros y un resumen de datos.

---

### 4. **Persistencia automática**
- Toda la información se guarda en `family.json`.
- Se carga y sincroniza automáticamente al iniciar la app.
- No usa base de datos externa, es completamente local.

---

## Estructura del proyecto

El proyecto está organizado por páginas:

```
Proyecto2_Datos_I/
│
├── Dtos/
│ ├── FamilyDto.cs
│ └── PersonDto.cs
│
├── Images/
│ ├── home.png
│ ├── map.png
│ ├── mapworld.jpg
│ ├── mapas-de-google.png
│ ├── statistics.png
│ └── tree.png
│
├── Pages/
│ ├── Page1.xaml (Mapa interactivo)
│ ├── Page1.xaml.cs
│ ├── Page2.xaml (Arbol genealógico)
│ ├── Page2.xaml.cs
│ ├── Page3.xaml (Página de estadísticas)
│ ├── Page3.xaml.cs
│ ├── Page4.xaml (Formulario)
│ ├── Page4.xaml.cs
│ ├── Page5.xaml
│ ├── Page5.xaml.cs
│ ├── Page6.xaml
│ └── Page6.xaml.cs
│
├── Services/
│ ├── FamilyJsonRepository.cs
│ └── GraphMapper.cs
│
├── Themes/
│ ├── Estilos.xaml
│ └── Generic.xaml
│
├── App.xaml
├── App.xaml.cs
├── AssemblyInfo.cs
├── Grafo.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── MapBounds.cs
├── NavButton.cs
├── Proyecto2_Datos_I.csproj
└── Proyecto2_Datos_I.sln
```
---

## Arquitectura general

El proyecto sigue una estructura sencilla:

- **Modelo:**  
  `PersonNode`, `Grafo` y DTOs para persistencia.

- **Vista:**  
  Cada página XAML define una sección distinta de la aplicación.

- **Lógica de vista de cada ventana**  

- **Persistencia:**  
  Serialización/deserialización JSON usando `System.Text.Json`.

---

## Requisitos para ejecutar

- .NET 7.0 Desktop Runtime  
- Windows (WPF)
- Visual Studio 2022 recomendado  




