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
        private readonly OdstavkyActionService _service;

        public ChangeTimeOdstavkaWithDieslovaniTest()
        {
            _mockOdstavkyRepository = new Mock<IOdstavkyRepository>();
            _mockDieslovaniService = new Mock<IDieslovaniService>();
            _mockTechnikService = new Mock<ITechnikService>();

            _service = new OdstavkyActionService(
                _mockOdstavkyRepository.Object,
                _mockDieslovaniService.Object,
                _mockTechnikService.Object
            );
        }

        [Fact]
        public async Task ChangeTimeOdstavkyAsync_WhenTimeIsChanged_CallsDieslovaniAndReturnsSuccess()
        {
            // Arrange
            int idodstavky = 1;
            DateTime newTime = new DateTime(2025, 1, 1, 12, 0, 0);
            string type = "zacatek";

            // Připravíme odstávku s vyplněnou lokalitou 
            // (pokud "IsDieselRequired" čte Klasifikaci, Baterii atd., vyplňte je).
            var odstavka = new Odstavka 
            { 
                ID = idodstavky,
                Lokality = new Lokalita
                {
                    Klasifikace = "A1",
                    Baterie = 123,
                }
            };

            // V repository simuluji, že odstávka existuje:
            _mockOdstavkyRepository
                .Setup(repo => repo.GetByIdAsync(idodstavky))
                .ReturnsAsync(odstavka);

            // Musíme zajistit, že se v HandleOdstavkyDieslovani nenastaví success = false:
            // - nastavit, že se najde technik
            _mockTechnikService
                .Setup(x => x.GetTechnikByIdAsync("606794494"))
                .ReturnsAsync(new Technik 
                {
                    ID = "606794494",
                });

            // - a nastavit, že dieslování dopadne úspěšně (result.Success = true)
            _mockDieslovaniService
                .Setup(service => service.HandleOdstavkyDieslovani(It.IsAny<Odstavka>(), It.IsAny<HandleResult>()))
                .ReturnsAsync((Odstavka odst, HandleResult res) =>
                {
                    // Tady simulujeme, že "IsDieselRequired" došla k závěru, že je dieslování OK, 
                    // a pak byl technik nalezen => success = true
                    res.Success = true;
                    res.Message = "Vytvořeno nové dieslování.";  // nebo cokoliv
                    return res;
                });

            // Mock pro uložení
            _mockOdstavkyRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Odstavka>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ChangeTimeOdstavkyAsync(idodstavky, newTime, type);

            // Assert
            Assert.True(result.Success);                      // Ověřím, že se vrátil úspěch
            Assert.Equal("Čas byl úspěšně změněn.", result.Message);  // "Čas byl úspěšně změněn."

            // Zkontrolujeme, zda se odstávka opravdu změnila
            Assert.Equal(newTime, odstavka.Od);              
            Assert.Null(odstavka.Do);  // protože měníme jen zacatek

            // Ověříme, že se skutečně zavolala metoda dieslování
            _mockDieslovaniService.Verify(
                x => x.HandleOdstavkyDieslovani(It.IsAny<Odstavka>(), It.IsAny<HandleResult>()),
                Times.Once
            );
        }
    }
}
