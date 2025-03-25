// ----------------------------------------
// Function to render a badge indicating the status of the Dieslovani record.
// ----------------------------------------
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

    // If ZadanOdchod is set, mark as "Ukončené".
    if (zadanOdchod == true) {
        badgeClass = "badge-phoenix-danger";
        badgeStyle = "background-color: red; border-radius: 5px;";
        labelStyle = "color: white; padding: 1px; font-size: small;";
        labelText = "Ukončené";
        iconClass = "fa-check-circle";
        iconColor = "black";
    }
    // If ZadanVstup is set but ZadanOdchod is not, mark as "Aktivní".
    else if (zadanVstup == true && zadanOdchod == false) {
        badgeClass = "badge-phoenix-primary";
        badgeStyle = "background-color: green; border-radius: 5px;";
        labelStyle = "color: white; padding: 1px; font-size: small;";
        labelText = "Aktivní";
        iconClass = "fa-clock-rotate-left";
        iconColor = "black";
    }
    // If the technician ID matches a specific value, mark as "Nepřiřazeno".
    else if (row.idtechnika == "606794494") {
        badgeClass = "badge-phoenix-warning";
        badgeStyle = "background-color: orange; border-radius: 5px;";
        labelText = "Nepřiřazeno";
        iconClass = "fa-clock-rotate-left";
        iconColor = "black";
    }

    // Return the HTML for the badge.
    return `
        <span class="badge fs-10 ${badgeClass}" style="${badgeStyle}">
            <span class="badge-label" style="${labelStyle}">${labelText}</span>
            <i class="fa-solid ${iconClass}" style="color: ${iconColor};"></i>
        </span>
    `;
}

$(document).ready(function () {
    // ----------------------------------------
    // Extract the Dieslovani ID from the URL.
    // ----------------------------------------
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");
    const min = "0001-01-01T00:00:00"; 

    if (id) {
        // ----------------------------------------
        // Fetch Dieslovani details from the server.
        // ----------------------------------------
        $.ajax({
            url: `/Dieslovani/DetailDieslovaniJson?id=${id}`,
            type: 'GET',
            success: function (response) {
                const data = response.data;
                console.log(response.data);

                if (data) {
                    // ----------------------------------------
                    // Populate the Dieslovani details in the view.
                    // ----------------------------------------
                    $('#dieslovaniId').append(data.dieslovaniId);
                    $('#odstavkaId').append(data.odstavkaId);
                    $('#lokalita').append(data.lokalita);
                    $('#adresa').append(data.adresa);
                    $('#klasifikace').append(data.klasifikace);
                    $('#baterie').append(data.baterie);
                    $('#region').append(data.region);
                    $('#popis').append(data.popis);

                    // Format and display the entry date (ZadanVstup).
                    if (data.zadanVstup == min) {
                        $('#zadanVstup').append("--");
                    } else {
                        $('#zadanVstup').append(formatDate(data.zadanVstup));
                    }

                    // Format and display the exit date (ZadanOdchod).
                    if (data.zadanOdchod == min) {
                        $('#zadanOdchod').append("--");
                    } else {
                        $('#zadanOdchod').append(formatDate(data.zadanOdchod));
                    }

                    // Display the technician's name as a clickable link.
                    $('#technik').append(`<a href="/User/Index?id=${data.technik}">${data.jmenoTechnika} ${data.prijmeniTechnika}</a>`);

                    // Render and display the status badge.
                    const badgeHTML = renderBadge(null, null, data);
                    $('#statusBadge').html(badgeHTML);

                    // Render and display the delete badge.
                    const deleteBadgeHTML = `
                        <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="DeleteWithoutConfirm(${data.dieslovaniId})">
                            <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                            <i class="fa-solid fa-xmark"></i>
                        </span>`;
                    $('#deleteBadge').html(deleteBadgeHTML);

                    // Render and display the "Change Time" button for entry (Vstup).
                    const buttonHtmlVstup = `
                        <button type="button"
                                class="button VstupOdchod"
                                onclick="ChangeTime(${data.dieslovaniId}, document.getElementById('vstupChange').value, 'vstup')">
                            Změnit čas
                        </button>`;
                    $('#changeTimeVstup').html(buttonHtmlVstup);

                    // Render and display the "Change Time" button for exit (Odchod).
                    const buttonHtmlOdchod = `
                        <button type="button"
                                class="button VstupOdchod"
                                onclick="ChangeTime(${data.dieslovaniId}, document.getElementById('odchodChange').value, 'odchod')">
                            Změnit čas
                        </button>`;
                    $('#changeTimeOdchod').html(buttonHtmlOdchod);

                } else {
                    // ----------------------------------------
                    // Display a message if no data is found.
                    // ----------------------------------------
                    $('#dieslovani-detail').html('<p>Data nebyla nalezena.</p>');
                }
            },
            error: function () {
                // ----------------------------------------
                // Display an error message if the request fails.
                // ----------------------------------------
                $('#dieslovani-detail').html('<p>Chyba při načítání dat.</p>');
            }
        });
    } else {
        // ----------------------------------------
        // Display a message if no ID is provided in the URL.
        // ----------------------------------------
        $('#dieslovani-detail').html('<p>ID dieslování nebylo poskytnuto.</p>');
    }
});

