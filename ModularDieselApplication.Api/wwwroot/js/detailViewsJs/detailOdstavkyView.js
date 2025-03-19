$(document).ready(function () {
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
    console.log(data);
    console.log(id);

    if (id) {
        $.ajax({
            url: `/Odstavky/DetailOdstavkyJson?id=${id}`,
            type: 'GET',
            
            success: function (response) {
                const data = response.data;
                console.log(response.data);
                if (data) {
                    $('#odstavkaId').append(data.odstavkaId);
                    $('#lokalita').append(data.lokalita);
                    $('#adresa').append(data.adresa);
                    $('#klasifikace').append(data.klasifikace);
                    $('#baterie').append(data.baterie);
                    $('#region').append(data.region);
                    $('#popis').append(data.popis);
                    const buttonZacatek = `
                    <button type="button"
                            class="button VstupOdchod"
                            onclick="ChangeTime(${data.odstavkaId}, document.getElementById('zacatekChange').value, 'zacatek')">
                        Změnit čas
                    </button>
                    `;
                    $('#changeTimeZacatek').html(buttonKonec);
                    const buttonKonec = `
                    <button type="button"
                            class="button VstupOdchod"
                            onclick="ChangeTime(${data.odstavkaId}, document.getElementById('konecChange').value, 'konec')">
                        Změnit čas
                    </button>
                    `;
                    $('#changeTimeKonec').html(buttonZacatek);

                    $('#zacatekOdstavky').append(formatDate(data.zacatekOdstavky));
                    $('#konecOdstavky').append(formatDate(data.konecOdstavky));
                } else {
                }
            },
            error: function () {
            }
        });
    } else {
    }
});
