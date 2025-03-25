$(document).ready(function () {
    // ----------------------------------------
    // Function to get the User ID from the URL.
    // ----------------------------------------
    function getUserIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get("id"); // Returns the value of the 'id' parameter.
    }

    // ----------------------------------------
    // Initialize the DataTable for displaying technician Dieslovani records.
    // ----------------------------------------
    $('#technikDieslovaniTable').DataTable({
        ajax: {
            // ----------------------------------------
            // Configure the AJAX request to fetch data from the server.
            // ----------------------------------------
            url: '/User/VazbyJson', // Server-side method to fetch data.
            type: 'GET',
            data: function (d) {
                const id = getUserIdFromUrl();
                console.log("ID sent to the server:", id); // Debugging log.
                d.id = id; // Add the User ID to the request data.
            },
            dataSrc: function (json) {
                console.log("Data returned by the server:", json); // Debugging log.
                return json.data;
            }
        },
        columns: [
            {
                // ----------------------------------------
                // Render a badge indicating the status of the record.
                // ----------------------------------------
                render: function (data, type, row) {
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
            },
            {
                // ----------------------------------------
                // Render a button to close the Dieslovani record.
                // ----------------------------------------
                data: null,
                render: function (data, type, row) {
                    return `
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="deleteRecordDieslovani(this, ${row.id})">
                        <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                        <i class="fa-solid fa-xmark"></i>
                    </span>`;
                }
            },
            {
                // ----------------------------------------
                // Render a clickable link for the Dieslovani ID.
                // ----------------------------------------
                data: 'id',
                render: function (data, type, row) {
                    return `
                        <a href="/Dieslovani/DetailDieslovani?id=${data}">
                        ${data}
                        </a>`;
                }
            },
            {
                // ----------------------------------------
                // Render the distributor logo based on the distributor name.
                // ----------------------------------------
                data: 'distributor',
                render: function (data, type, row) {
                    let logo = '';
                    if (data === 'ČEZ') {
                        logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    } else if (data === 'EGD') {
                        logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    } else if (data === 'PRE') {
                        logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                    }
                    return logo;
                }
            },
            {
                // ----------------------------------------
                // Render the Lokalita name.
                // ----------------------------------------
                data: 'lokalitaNazev',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
                }
            },
            {
                // ----------------------------------------
                // Render the classification (Klasifikace) column.
                // ----------------------------------------
                data: 'klasifikace',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
                }
            },
            { data: 'adresa' },
            { data: 'technikFirma' },
            {
                // ----------------------------------------
                // Render a clickable link for the technician's name.
                // ----------------------------------------
                data: null,
                render: function (data, type, row) {
                    return `<a class="userA" href="/User/Index?id=${data.idUser}">
                        ${data.jmenoTechnika} ${data.prijmeniTechnika}
                    </a>`;
                }
            },
            { data: 'nazevRegionu' },
            {
                // ----------------------------------------
                // Format the start date of the Odstavka.
                // ----------------------------------------
                data: 'odstavkaZacatek',
                render: function (data) {
                    return formatDate(data);
                }
            },
            {
                // ----------------------------------------
                // Format the end date of the Odstavka.
                // ----------------------------------------
                data: 'odstavkaKonec',
                render: function (data) {
                    return formatDate(data);
                }
            },
            {
                // ----------------------------------------
                // Format the entry date (ZadanVstup).
                // ----------------------------------------
                data: 'zadanVstup',
                render: function (data) {
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "-"; // Display a dash if the date is not set.
                    } else {
                        return formatDate(data); // Format the date.
                    }
                }
            },
            {
                // ----------------------------------------
                // Format the exit date (ZadanOdchod).
                // ----------------------------------------
                data: 'zadanOdchod',
                render: function (data) {
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "-"; // Display a dash if the date is not set.
                    } else {
                        return formatDate(data); // Format the date.
                    }
                }
            },
            { data: 'popis' },
            { data: 'vydrzBaterie' },
            {
                // ----------------------------------------
                // Render an icon indicating whether a socket (Zásuvka) is available.
                // ----------------------------------------
                data: 'zasuvka',
                render: function (data, type, row) {
                    return data
                        ? '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>'
                        : '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                }
            }
        ],
        // ----------------------------------------
        // Apply custom row styling based on the data.
        // ----------------------------------------
        rowCallback: function (row, data, index) {
            const today = new Date().setHours(0, 0, 0, 0);
            const startDate = new Date(data.odstavkaZacatek).setHours(0, 0, 0, 0);
            const minDateString = "0001-01-01T00:00:00";
            const zadanVstup = data.zadanVstup && data.zadanVstup !== minDateString;
            const zadanOdchod = data.zadanOdchod && data.zadanOdchod !== minDateString;

            if (zadanOdchod == true) {
                $(row).addClass('row-ukoncene');
            } else if (zadanVstup == true && zadanOdchod == false) {
                $(row).addClass('row-aktivni');
            } else if (data.idtechnika == "606794494") {
                $(row).addClass('row-neprirazeno');
            } else if (zadanOdchod == false && zadanVstup == false && today == startDate) {
                $(row).addClass('row-cekajici');
            } else {
                $(row).addClass('row-standart');
            }
        },
        paging: true,        // Enable pagination.
        searching: true,     // Enable searching.
        ordering: true,      // Enable column ordering.
        lengthChange: false, // Disable length change.
        pageLength: 9        // Set the number of rows per page.
    });
});


