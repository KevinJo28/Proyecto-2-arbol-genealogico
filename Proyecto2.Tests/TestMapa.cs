using System;
using Proyecto2_Datos_I_Grafo;
using Xunit;

public class TestMapa
{
    private const double MinX = 50;
    private const double MaxX = 1450;
    private const double MinY = 100;
    private const double MaxY = 800;

    private PersonNode P(string coords)
    {
        return new PersonNode("Test", coords, DateTime.Now, 20, null!, 1, "");
    }

    private double D(string c1, string c2)
    {
        var a = c1.Split(',');
        var b = c2.Split(',');

        double x1 = double.Parse(a[0]) + 29;
        double y1 = double.Parse(a[1]) + 29;
        double x2 = double.Parse(b[0]) + 29;
        double y2 = double.Parse(b[1]) + 29;

        double dx = x2 - x1;
        double dy = y2 - y1;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    [Fact]
    public void DistanciaEntreDosPuntos()
    {
        var p1 = P("100,100");
        var p2 = P("100,200");

        double distancia = D(p1.Coords, p2.Coords);

        Assert.Equal(100, distancia, 0);
    }

    [Fact]
    public void CoordenadaDentroYFueraDelMapa()
    {
        double validX = (MinX + MaxX) / 2.0;
        double validY = (MinY + MaxY) / 2.0;

        bool dentro =
            validX >= MinX && validX <= MaxX &&
            validY >= MinY && validY <= MaxY;

        double badX = MinX - 100;
        double badY = MinY - 100;

        bool fuera =
            badX >= MinX && badX <= MaxX &&
            badY >= MinY && badY <= MaxY;

        Assert.True(dentro);
        Assert.False(fuera);
    }
}
