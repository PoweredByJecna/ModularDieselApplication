using System;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;
using Xunit;

public class IsDieselRequiredTest
{
    // ----------------------------------------
    // Test: Verify "Hello, World!" string.
    // ----------------------------------------
    [Fact]
    public void HelloWorld_ReturnsExpectedString()
    {
        var result = "Hello, World!";
        Assert.Equal("Hello, World!", result);
    }

    // ----------------------------------------
    // Test: Should return false when Lokality has a stationary generator (DA).
    // ----------------------------------------
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
        var dieslovaniRules = new DieslovaniRules();
        var actualResult = await dieslovaniRules.IsDieselRequired(klasifikace, od, do_, baterie, newOdstavka, result);

        // Assert
        Assert.False(actualResult.Success); // Verify that diesel is not required.
        Assert.Equal("na lokalitě není potřeba dieslovat, nachází se tam stacionární generátor.", actualResult.Duvod);
    }

    // ----------------------------------------
    // Test: Should return false when Lokality has no socket (Zasuvka).
    // ----------------------------------------
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
        var dieslovaniRules = new DieslovaniRules();
        var actualResult = await dieslovaniRules.IsDieselRequired(klasifikace, od, do_, baterie, newOdstavka, result);

        // Assert
        Assert.False(actualResult.Success); // Verify that diesel is not required.
        Assert.Equal("na lokalitě se nedá dieslovat, protože tam není zásuvka.", actualResult.Duvod);
    }

    // ----------------------------------------
    // Add more tests to cover other scenarios.
    // ----------------------------------------
}