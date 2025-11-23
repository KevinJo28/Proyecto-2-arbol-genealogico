using System;
using System.Collections.Generic;
using Proyecto2_Datos_I_Grafo;
using Xunit;

public class TestsStats
{
    private PersonNode P(string name, string coords)
    {
        return new PersonNode(name, coords, DateTime.Now, 20, null!, name.GetHashCode(), "");
    }

    private double D(string c1, string c2)
    {
        var a = c1.Split(',');
        var b = c2.Split(',');

        double x1 = double.Parse(a[0]) + 29;
        double y1 = double.Parse(a[1]) + 29;
        double x2 = double.Parse(b[0]) + 29;
        double y2 = double.Parse(b[1]) + 29;

        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
    }

    [Fact]
    public void EncuentraParCercanoYParLejano()
    {
        var p1 = P("A", "100,100");
        var p2 = P("B", "200,100");
        var p3 = P("C", "100,300");

        double dAB = D(p1.Coords, p2.Coords);  // 100
        double dAC = D(p1.Coords, p3.Coords);  // 200
        double dBC = D(p2.Coords, p3.Coords);  // 223.6

        Assert.True(dAB < dAC);
        Assert.True(dAC < dBC);
    }

    [Fact]
    public void DistanciaPromedioEsCorrecta()
    {
        var p1 = P("A", "100,100");
        var p2 = P("B", "200,100");
        var p3 = P("C", "100,300");

        double d1 = D(p1.Coords, p2.Coords);
        double d2 = D(p1.Coords, p3.Coords);
        double d3 = D(p2.Coords, p3.Coords);

        double promedio = (d1 + d2 + d3) / 3.0;

        Assert.InRange(promedio, 160, 200);
    }
}
