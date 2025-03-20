function renderBadge(data, type, row) {
    let badgeClass = "badge-phoenix-success";
    let badgeStyle = "background-color: yellow; border-radius: 5px;";
    let labelStyle = "color: black; padding: 1px; font-size: small;";
    let labelText = "Čekající";
    let iconClass = "fa-clock-rotate-left";
    let iconColor = "black";

    let today = new Date().setHours(0, 0, 0, 0); 

    let od = row.zacatekOdstavky;
    let doo = row.konecOdstavky;
    let zacatek = od == today
    let konec = doo > today    

    if (doo == true) {
        badgeClass = "badge-phoenix-danger";
        badgeStyle = "background-color: red; border-radius: 5px;";
        labelStyle = "color: white; padding: 1px; font-size: small;";
        labelText = "Ukončené";
        iconClass = "fa-check-circle";
        iconColor = "black";
    }
    return `
        <span class="badge fs-10 ${badgeClass}" style="${badgeStyle}">
            <span class="badge-label" style="${labelStyle}">${labelText}</span>
            <i class="fa-solid ${iconClass}" style="color: ${iconColor};"></i>
        </span>
    `;
}

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
                    $('#zacatekOdstavky').append(formatDate(data.zacatekOdstavky));
                    $('#konecOdstavky').append(formatDate(data.konecOdstavky));
               
                    const deleteBadgeHTML = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="DeleteWithoutConfirmOdstavka(${data.odstavkaId})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                    <i class="fa-solid fa-xmark"></i>
                    </span>`;
                    $('#deleteBadge').html(deleteBadgeHTML);

                    const calldieslovaniHTML = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color:rgb(185, 113, 30); border-radius: 5px; cursor: pointer" onclick="CallDieslovani(${data.odstavkaId})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Objednat DA</span>
                    <i class="fa-solid fa-plus"></i>
                    </span>`;
                    $('#calldieslovaniBadge').html(calldieslovaniHTML);

                    const badgeHTML = renderBadge(null, null, data);
                    $('#statusBadge').html(badgeHTML);

                    const buttonZacatek = `
                    <button type="button"
                            class="button VstupOdchod"
                            onclick="ChangeTimeOdstavky(${data.odstavkaId}, document.getElementById('zacatekChange').value, 'zacatek')">
                        Změnit čas
                    </button>
                    `;
                    $('#changeTimeZacatek').html(buttonZacatek);
                    const buttonKonec = `
                    <button type="button"
                            class="button VstupOdchod"
                            onclick="ChangeTimeOdstavky(${data.odstavkaId}, document.getElementById('konecChange').value, 'konec')">
                        Změnit čas
                    </button>
                    `;
                    $('#changeTimeKonec').html(buttonKonec);
                } else {
                }
            },
            error: function () {
            }
        });
    } else {
    }
});
