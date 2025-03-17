$(document).ready(function() {
  $.ajax({
      url: '/Regiony/GetRegionDataJizniMorava',
      type: 'GET',
      success: function(response) {
          const regiony = response.data;
          if (!regiony || regiony.length === 0) {
              console.log("Žádná data.");
              return;
          }

          // Vezmeme první region
          const firstRegion = regiony[0];

          // Naplníme data – předáme selektory pro jednotlivé prvky
          processRegionData(firstRegion, {
              distributor: '#jm-distributor',
              firma: '#jm-firma',
              pocetOdstavek: '#jm-pocet-odstavek',
              pocetLokalit: '#jm-pocet-lokalit',
              technici: '#jm-technici'
          });
      },
      error: function() {
          console.error('Nepodařilo se načíst data pro Jižní Moravu');
      }
  });
});

function processRegionData(region, selectors) {
  $(selectors.firma).text(region.firma);
  $(selectors.distributor).text(region.distributor);
  $(selectors.pocetOdstavek).text(region.pocetOdstavek);
  $(selectors.pocetLokalit).text(region.pocetLokalit);

  const techniciContainer = $(selectors.technici);
  techniciContainer.empty();

  region.technici.forEach(function(tech) {
      const techLink = `
          <div>
              <a class="userA" href="/User/Index?id=${tech.userId}">
                  ${tech.jmeno}
              </a>
              ${tech.maPohotovost ? ' <i class="fa-solid fa-circle fa-sm navA" style="color: #1dd510;"></i>' : ' <i class="fa-solid fa-circle fa-sm navA" style="color: #eb0a0a;"></i>'}
          </div>`;
      techniciContainer.append(techLink);
  });
}
