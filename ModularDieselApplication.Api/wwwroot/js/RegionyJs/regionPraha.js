$(document).ready(function() {
  $.ajax({
    url: '/Regiony/GetRegionDataPraha',
    type: 'GET',
    success: function(response) {
      const regiony = response.data;
      if (!regiony || regiony.length === 0) {
        console.log("Žádná data.");
        console.log(data);
        return;
      }

      const firstRegion = regiony[0];
      
      processRegionData(firstRegion, {
        distributor: '#psc-distributor',
        firma: '#psc-firma',
        pocetOdstavek: '#psc-pocet-odstavek',
        pocetLokalit: '#psc-pocet-lokalit',
        technici: '#psc-technici'
      });
    },
    error: function() {
      console.error('Nepodařilo se načíst data pro Prahu + Střední Čechy');
    }
  });
});
