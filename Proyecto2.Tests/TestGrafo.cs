

using Proyecto2_Datos_I_Grafo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using Xunit;




public class TestGrafoUnitTests
{
    [Fact]
    public void AgregarPersonaConDatosValidosSeGuardaYSeRecupera()
    {
        var grafo = new Grafo();
        var birthdate = new DateTime(2006, 10, 28);
        grafo.Add("Kevin", "9.94, -84.10", birthdate, 19, null, 123123, "ruta.jpg");

        var p = grafo.Get(123123);

        Assert.NotNull(p);
        Assert.Equal("Kevin", p!.FullName);
        Assert.Equal("9.94, -84.10", p.Coords);
        Assert.Equal(birthdate, p.BirthDate);
        Assert.Equal(19, p.Age);
    }
    [Fact]
    public void PruebaEnlazarPadres()
    {
        var grafo = new Grafo();
        var birthdate = new DateTime(2006, 10, 28);
        var birthdate2 = new DateTime(1986, 02, 09);
        grafo.Add("Kevin", "9.94, -84.10", birthdate, 19, null, 123123, "ruta.jpg");
        grafo.Add("Denis", "9.94, -84.10", birthdate, 39, null, 343545, "ruta.jpg");

        var hijo = grafo.Get(123123);
        var padre = grafo.Get(343545);
       
        grafo.LinkParentChild(padre, hijo);

        Assert.NotNull(hijo);
        Assert.NotNull(padre);
        Assert.Contains(padre, hijo.Parents);
        Assert.Contains(hijo, padre.Children);






    }

    [Fact]
    public void PruebaEnlazarParejas()
    {
        var grafo = new Grafo();
        var birthdate = new DateTime(2006, 10, 28);
        var birthdate2 = new DateTime(1986, 02, 09);
        grafo.Add("Paco", "9.94, -84.10", birthdate, 19, null, 123123, "ruta.jpg");
        grafo.Add("Federica", "9.94, -84.10", birthdate, 39, null, 343545, "ruta.jpg");

        var novio = grafo.Get(123123);
        var novia = grafo.Get(343545);

        grafo.LinkPartners(novio, novia);

        Assert.NotNull(novio);
        Assert.NotNull(novia);
        Assert.Contains(novia, novio.Partners);
        Assert.Contains(novio, novia.Partners);









    }


}






