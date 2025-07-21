using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Application.Interfaces.Services;

namespace ModularDieselApplication.Tests.ApplicationTests
{
    public class ChangeTimeOdstavkaWithDieslovaniTest
    {
        private readonly Mock<IOdstavkyRepository> _mockOdstavkyRepository;
        private readonly Mock<IDieslovaniService> _mockDieslovaniService;
        private readonly Mock<ITechnikService> _mockTechnikService;
        private readonly Mock<ILogService> _mockLogService;
        private readonly OdstavkyActionService _service;

        public ChangeTimeOdstavkaWithDieslovaniTest()
        {
            // Initialize mocks for dependencies.
            _mockOdstavkyRepository = new Mock<IOdstavkyRepository>();
            _mockDieslovaniService = new Mock<IDieslovaniService>();
            _mockTechnikService = new Mock<ITechnikService>();
            _mockLogService = new Mock<ILogService>();

            // Initialize the service under test with mocked dependencies.
            _service = new OdstavkyActionService(
                _mockOdstavkyRepository.Object,
                _mockDieslovaniService.Object,
                _mockTechnikService.Object,
                _mockLogService.Object
            );
        }

        // ----------------------------------------
        // Test: ChangeTimeOdstavkyAsync should call Dieslovani and return success.
        // ----------------------------------------
        [Fact]
        public async Task ChangeTimeOdstavkyAsync_WhenTimeIsChanged_CallsDieslovaniAndReturnsSuccess()
        {
            // Arrange
            string idodstavky = "1";
            DateTime newTime = new DateTime(2025, 1, 1, 12, 0, 0);
            string type = "zacatek";

            // Prepare an Odstavka with a Lokality object.
            var odstavka = new Odstavka
            {
                ID = idodstavky,
                Lokality = new Lokalita
                {
                    Klasifikace = "A1",
                    Baterie = 123,
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
                }
            };

            // Mock repository to return the prepared Odstavka.
            _mockOdstavkyRepository
                .Setup(repo => repo.GetByIdAsync(idodstavky))
                .ReturnsAsync(odstavka);

            // Mock TechnikService to return a valid Technik.
            _mockTechnikService
                .Setup(x => x.GetTechnikByIdAsync(FiktivniTechnik.Id))
                .ReturnsAsync(new Technik
                {
                    ID = FiktivniTechnik.Id,
                    User = new User
                    {

                    },
                    Firma = new Firma
                    {
                        ID = "firma1",
                        Nazev = "Firma 1"
                    }

                });

            // Mock DieslovaniService to simulate successful dieslovani handling.
            _mockDieslovaniService
                .Setup(service => service.HandleOdstavkyDieslovani(It.IsAny<Odstavka>(), It.IsAny<HandleResult>()))
                .ReturnsAsync((Odstavka odst, HandleResult res) =>
                {
                    res.Success = true;
                    res.Message = "Vytvořeno nové dieslování.";
                    return res;
                });

            // Mock repository to simulate successful update.
            _mockOdstavkyRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Odstavka>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ChangeTimeOdstavkyAsync(idodstavky, newTime, type);

            // Assert
            Assert.True(result.Success); // Verify the result indicates success.
            Assert.Equal("Čas byl úspěšně změněn.", result.Message); // Verify the success message.

            // Verify that the Odstavka's time was updated.
            Assert.Equal(newTime, odstavka.Od);

            // Verify that DieslovaniService was called exactly once.
            _mockDieslovaniService.Verify(
                x => x.HandleOdstavkyDieslovani(It.IsAny<Odstavka>(), It.IsAny<HandleResult>()),
                Times.Once
            );
        }
    }
}
