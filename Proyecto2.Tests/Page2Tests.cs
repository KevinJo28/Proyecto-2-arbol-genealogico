using System;
using System.Collections.Generic;
using Xunit;
using Proyecto2_Datos_I_Grafo;
using SideBar_Nav.Pages;

public class Page2Tests
{
    private PersonNode CrearPersona(int id, string name = "")
    {
        return new PersonNode(
            name,
            "0,0",
            DateTime.Now,
            30,
            null!,
            id,
            ""
        );
    }

    [Fact]
    public void GenerarKey_DebeSerSimetrica()
    {
        var p1 = CrearPersona(10);
        var p2 = CrearPersona(5);

        var k1 = Page2.GenerarKey(p1, p2);
        var k2 = Page2.GenerarKey(p2, p1);

        Assert.Equal("5-10", k1);
        Assert.Equal(k1, k2);
    }

    [Fact]
    public void BuildLevelsConParejas_PadreEnNivel0_HijoEnNivel1()
    {
        var padre = CrearPersona(1, "Padre");
        var hijo = CrearPersona(2, "Hijo");

        padre.Children.Add(hijo);
        hijo.Parents.Add(padre);

        var lista = new List<PersonNode> { padre, hijo };

        var niveles = Page2.BuildLevelsConParejas(lista);

        Assert.Contains(padre, niveles[0]);
        Assert.Contains(hijo, niveles[1]);
    }

    [Fact]
    public void EncontrarParejaConHijosCompartidos_DevuelveLaParejaCorrecta()
    {
        var padre = CrearPersona(1, "Padre");
        var madre = CrearPersona(2, "Madre");
        var hijo = CrearPersona(3, "Hijo");

        padre.Partners.Add(madre);
        madre.Partners.Add(padre);

        padre.Children.Add(hijo);
        madre.Children.Add(hijo);
        hijo.Parents.Add(padre);
        hijo.Parents.Add(madre);

        var result = Page2.EncontrarParejaConHijosCompartidos(padre);

        Assert.Same(madre, result);
    }
}
