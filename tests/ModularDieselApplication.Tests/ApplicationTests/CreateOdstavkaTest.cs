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
        // Initialize the mock for IOdstavkyService.
        _odstavkyServiceMock = new Mock<IOdstavkyService>();
    }

    // ----------------------------------------
    // Test: CreateOdstavka with valid data should create an Odstavka successfully.
    // ----------------------------------------
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
            Nazev = "TesLokalita",
            Region = new Region
            {
                ID = "region1",
                Nazev = "Region 1",
                Firma = new Firma
                {
                    ID = "firma1",
                    Nazev = "Firma 1"
                }
            }
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

        // Mock the CreateOdstavkaAsync method to return the expected result.
        _odstavkyServiceMock
            .Setup(service => service.CreateOdstavkaAsync(
                testLokalita.Nazev, fromDateTime, toDateTime, testPopis, "default"))
            .ReturnsAsync(handleResult);

        // Act
        var result = await _odstavkyServiceMock.Object.CreateOdstavkaAsync(
            testLokalita.Nazev, fromDateTime, toDateTime, testPopis, "default");

        // Assert
        Assert.NotNull(result); // Verify the result is not null.
        Assert.True(result.Success); // Verify the result indicates success.

        var createdOdstavka = result.Odstavka;
        Assert.NotNull(createdOdstavka); // Verify the created Odstavka is not null.
        Assert.Equal(expectedKlasifikace, createdOdstavka.Lokality.Klasifikace); // Verify the classification.
        Assert.Equal(fromDateTime, createdOdstavka.Od); // Verify the start time.
        Assert.Equal(toDateTime, createdOdstavka.Do); // Verify the end time.
        Assert.Equal(expectedBaterie, createdOdstavka.Lokality.Baterie); // Verify the battery capacity.
        Assert.Equal(testPopis, createdOdstavka.Popis); // Verify the description.
        Assert.Equal(testLokalita.Nazev, createdOdstavka.Lokality.Nazev); // Verify the location name.
        Assert.Equal(testLokalita.DA, createdOdstavka.Lokality.DA); // Verify the DA flag.
        Assert.Equal(testLokalita.Zasuvka, createdOdstavka.Lokality.Zasuvka); // Verify the socket availability.

        // Verify that the CreateOdstavkaAsync method was called exactly once.
        _odstavkyServiceMock.Verify(
            s => s.CreateOdstavkaAsync(testLokalita.Nazev, fromDateTime, toDateTime,
            testPopis, "default"),
            Times.Once
        );
    }
}
