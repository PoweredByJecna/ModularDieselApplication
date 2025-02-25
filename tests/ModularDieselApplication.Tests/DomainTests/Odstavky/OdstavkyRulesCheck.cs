using System;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;
using Xunit;

public class OdstavkyRulesTests
{
    [Fact]
    public async Task OdstavkyCheck_ShouldReturnFalse_WhenExistingOdstavkaIsFalse()
    {
        // Arrange
        var od = DateTime.Now;
        var do_ = DateTime.Now.AddHours(1);
        var result = new HandleResult();
        var existingOdstavka = false;

        // Act
        var actualResult = await OdstavkyRules.OdstavkyCheck(od, do_, result, existingOdstavka);

        // Assert
        Assert.False(actualResult.Success);
        Assert.Equal("Již existuje jiná odstávka.", actualResult.Message);
    }

    [Fact]
    public async Task OdstavkyCheck_ShouldReturnFalse_WhenDateRangeIsInvalid()
    {
        // Arrange
        var od = DateTime.Now.AddDays(-1);
        var do_ = DateTime.Now.AddHours(1);
        var result = new HandleResult();
        var existingOdstavka = true;

        // Act
        var actualResult = await OdstavkyRules.OdstavkyCheck(od, do_, result, existingOdstavka);

        // Assert
        Assert.False(actualResult.Success);
        Assert.Equal("Špatně zadané datum.", actualResult.Message);
    }

    [Fact]
    public async Task OdstavkyCheck_ShouldReturnTrue_WhenDateRangeIsValidAndExistingOdstavkaIsTrue()
    {
        // Arrange
        var od = DateTime.Now;
        var do_ = DateTime.Now.AddHours(1);
        var result = new HandleResult();
        var existingOdstavka = true;

        // Act
        var actualResult = await OdstavkyRules.OdstavkyCheck(od, do_, result, existingOdstavka);

        // Assert
        Assert.True(actualResult.Success);
    }
}