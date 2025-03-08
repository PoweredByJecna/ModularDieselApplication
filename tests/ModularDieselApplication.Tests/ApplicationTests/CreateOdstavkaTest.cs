using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;
using Moq;
using Xunit;
using ModularDieselApplication.Application.Common.Models;

public class CreateOdstavkaTest
{
    private readonly Mock<IOdstavkyService> _odstavkyServiceMock;
    public CreateOdstavkaTest()
    {
        _odstavkyServiceMock = new Mock<IOdstavkyService>();
    }
    [Fact]
    public async Task CreateOdstavka_ValidData_ShouldCreateOdstavkaSuccessfully()
    {
        // Arrange
        var expectedKlasifikace = "B2";
        var fromDateTime = DateTime.Now;
        var toDateTime = fromDateTime.AddHours(1);
        var expectedBaterie = 50;
        var testPopis = "test";
        var testLokalita = new Lokalita
        {
            DA = false,
            Zasuvka = true,
            Klasifikace = expectedKlasifikace,
            Baterie = expectedBaterie,
            Nazev = "TesLokalita"
        };

        var expectedOdstavka = new Odstavka
        {
            Od = fromDateTime,
            Do = toDateTime,
            Lokality = testLokalita,
            Popis = testPopis
        };

        var handleResult = new HandleResult
        {
            Success = true,
            Odstavka = expectedOdstavka
        };

        _odstavkyServiceMock
            .Setup(service => service.CreateOdstavkaAsync(
                testLokalita.Nazev, fromDateTime, toDateTime, testPopis, "default"))
            .ReturnsAsync(handleResult);

        // Act
        var result = await _odstavkyServiceMock.Object.CreateOdstavkaAsync(
            testLokalita.Nazev, fromDateTime, toDateTime, testPopis, "default");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);

        var createdOdstavka = result.Odstavka;
        Assert.NotNull(createdOdstavka);
        Assert.Equal(expectedKlasifikace, createdOdstavka.Lokality.Klasifikace);
        Assert.Equal(fromDateTime, createdOdstavka.Od);
        Assert.Equal(toDateTime, createdOdstavka.Do); 
        Assert.Equal(expectedBaterie, createdOdstavka.Lokality.Baterie);
        Assert.Equal(testPopis, createdOdstavka.Popis);
        Assert.Equal(testLokalita.Nazev, createdOdstavka.Lokality.Nazev);
        Assert.Equal(testLokalita.DA, createdOdstavka.Lokality.DA);
        Assert.Equal(testLokalita.Zasuvka, createdOdstavka.Lokality.Zasuvka);

        _odstavkyServiceMock.Verify(
            s => s.CreateOdstavkaAsync(testLokalita.Nazev, fromDateTime, toDateTime,
            testPopis, "default"),
            Times.Once
        );
        
    }
}
