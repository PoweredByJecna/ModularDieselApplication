$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const nazev = params.get("nazev");
    console.log(data);

    if (nazev) {
        $.ajax({
            url: `/Lokality/DetailLokalityJson?nazev=${nazev}`,
            type: 'GET',
            
            success: function (response) {
                const data = response.data;
                console.log(response.data);
                if (data) {
                    $('#iDOdstavky').append(data.IDodstavka);
                    $('#iDdieslovani').append(data.IDdislovani);
                    $('#lokalita').append(data.lokalita);
                    $('#adresa').append(data.adresa);
                    $('#klasifikace').append(data.klasifikace);
                    $('#baterie').append(data.baterie);
                    $('#region').append(data.region);
                    $('#popis').append(data.popis);
                } else {
                }
            },
            error: function () {
            }
        });
    } else {
    }
});
