function renderBadge(data, type, row) {
    let badgeClass = "badge-phoenix-success";
    let badgeStyle = "background-color: yellow; border-radius: 5px;";
    let labelStyle = "color: black; padding: 1px; font-size: small;";
    let labelText = "Čekající";
    let iconClass = "fa-clock-rotate-left";
    let iconColor = "black";

    let minDateString = "0001-01-01T00:00:00"; 

    let zadanVstup = row.zadanVstup && row.zadanVstup !== minDateString;
    let zadanOdchod = row.zadanOdchod && row.zadanOdchod !== minDateString;

    if (zadanOdchod == true) {
        badgeClass = "badge-phoenix-danger";
        badgeStyle = "background-color: red; border-radius: 5px;";
        labelStyle = "color: white; padding: 1px; font-size: small;";
        labelText = "Ukončené";
        iconClass = "fa-check-circle";
        iconColor = "black";
    } else if (zadanVstup == true && zadanOdchod == false) {
        badgeClass = "badge-phoenix-primary";
        badgeStyle = "background-color: green; border-radius: 5px;";
        labelStyle = "color: white; padding: 1px; font-size: small;";
        labelText = "Aktivní";
        iconClass = "fa-clock-rotate-left";
        iconColor = "black";
    } else if (row.idtechnika == "606794494") {
        badgeClass = "badge-phoenix-warning";
        badgeStyle = "background-color: orange; border-radius: 5px;";
        labelText = "Nepřiřazeno";
        iconClass = "fa-clock-rotate-left";
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
            url: `/Dieslovani/DetailDieslovaniJson?id=${id}`,
            type: 'GET',
            
            success: function (response) {
                const data = response.data;
                console.log(response.data);
                if (data) {
                    $('#dieslovaniId').append(data.dieslovaniId);
                    $('#odstavkaId').append(data.odstavkaId);
                    $('#lokalita').append(data.lokalita);
                    $('#adresa').append(data.adresa);
                    $('#klasifikace').append(data.klasifikace);
                    $('#baterie').append(data.baterie);
                    $('#region').append(data.region);
                    $('#popis').append(data.popis);
                    $('#technik').append(`<a href="/User/Index?id=${data.technik}">${data.jmenoTechnika} ${data.prijmeniTechnika}</a>`);
                    const badgeHTML = renderBadge(null, null, data);
                    $('#statusBadge').html(badgeHTML);
                    const deleteBadgeHTML = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="DeleteWithoutConfirm(${data.dieslovaniId})">
                                <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                                <i class="fa-solid fa-xmark"></i>
                            </span>`;
                    $('#deleteBadge').html(deleteBadgeHTML);

                } else {
                $('#dieslovani-detail').html('<p>Data nebyla nalezena.</p>');
                }
            },
            error: function () {
                $('#dieslovani-detail').html('<p>Chyba při načítání dat.</p>');
            }
        });
    } else {
        $('#dieslovani-detail').html('<p>ID dieslování nebylo poskytnuto.</p>');
    }
});

