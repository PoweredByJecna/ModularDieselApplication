using System;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;
using Xunit;

public class IsDieselRequiredTest
{
    [Fact]
    public void HelloWorld_ReturnsExpectedString()
    {
        var result = "Hello, World!";
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public async Task IsDieselRequired_ShouldReturnFalse_WhenLokalityHasDA()
    {
        // Arrange
        var klasifikace = "A1";
        var od = DateTime.Now;
        var do_ = DateTime.Now.AddHours(1);
        var baterie = 100;
        var newOdstavka = new Odstavka { Lokality = new Lokalita { DA = true } };
        var result = new HandleResult();

        // Act
        var actualResult = await DieslovaniRules.IsDieselRequired(klasifikace, od, do_, baterie, newOdstavka, result);

        // Assert
        Assert.False(actualResult.Success);
        Assert.Equal("na lokalitě není potřeba dieslovat, nachází se tam stacionární generátor.", actualResult.Duvod);
    }

    [Fact]
    public async Task IsDieselRequired_ShouldReturnFalse_WhenLokalityHasNoZasuvka()
    {
        // Arrange
        var klasifikace = "C";
        var od = DateTime.Now;
        var do_ = DateTime.Now.AddHours(1);
        var baterie = 100;
        var newOdstavka = new Odstavka { Lokality = new Lokalita { Zasuvka = false } };
        var result = new HandleResult();

        // Act
        var actualResult = await DieslovaniRules.IsDieselRequired(klasifikace, od, do_, baterie, newOdstavka, result);

        // Assert
        Assert.False(actualResult.Success);
        Assert.Equal("na lokalitě se nedá dieslovat, protože tam není zásuvka.", actualResult.Duvod);
    }

    // Add more tests to cover other scenarios
}