using System;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;
using Xunit;

public class OdstavkyRulesTests
{
    // ----------------------------------------
    // Test: Should return false when an existing Odstavka is false.
    // ----------------------------------------
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
        Assert.False(actualResult.Success); // Verify that the result indicates failure.
        Assert.Equal("Již existuje jiná odstávka.", actualResult.Message); // Verify the failure message.
    }

    // ----------------------------------------
    // Test: Should return false when the date range is invalid.
    // ----------------------------------------
    [Fact]
    public async Task OdstavkyCheck_ShouldReturnFalse_WhenDateRangeIsInvalid()
    {
        // Arrange
        var od = DateTime.Now.AddDays(-1); // Invalid start date in the past.
        var do_ = DateTime.Now.AddHours(1);
        var result = new HandleResult();
        var existingOdstavka = true;

        // Act
        var actualResult = await OdstavkyRules.OdstavkyCheck(od, do_, result, existingOdstavka);

        // Assert
        Assert.False(actualResult.Success); // Verify that the result indicates failure.
        Assert.Equal("Špatně zadané datum.", actualResult.Message); // Verify the failure message.
    }

    // ----------------------------------------
    // Test: Should return true when the date range is valid and an existing Odstavka is true.
    // ----------------------------------------
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
        Assert.True(actualResult.Success); // Verify that the result indicates success.
    }
}