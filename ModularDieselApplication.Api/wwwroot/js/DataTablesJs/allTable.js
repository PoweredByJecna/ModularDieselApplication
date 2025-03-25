// ----------------------------------------
// Initialize the DataTable for displaying data.
// ----------------------------------------
$('#allTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Dieslovani/GetTableDataAllTable', // Server-side method to fetch data.
        type: 'GET',
        dataSrc: function (json) {
            // Log the response from the server for debugging.
            console.log(json);
            return json.data;
        }
    },
    columns: [
        {
            // ----------------------------------------
            // Render the status badge based on the row data.
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
            // Render the "Uzavřít" button for each row.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="deleteRecordDieslovani(this, ${row.id})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                    <i class="fa-solid fa-xmark"></i>
                </span>
                `;
            }
        },
        {
            // ----------------------------------------
            // Render a clickable link for the ID column.
            // ----------------------------------------
            data: 'id',
            render: function (data, type, row) {
                return `
                    <a href="/Dieslovani/DetailDieslovani?id=${data}">
                    ${data}
                    </a>
                `;
            }
        },
        {
            // ----------------------------------------
            // Render the distributor logo based on the distributor name.
            // ----------------------------------------
            data: 'distributor',
            render: function (data, type, row) {
                var logo = '';
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
            // Render a clickable link for the Lokalita name.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.lokalitaNazev}">
                ${data.lokalitaNazev}</a>`;
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
                return `<a class="userA" href="/User/Index?id=${data.user}">
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
            // Render an icon indicating whether a socket (Zásuvka) is available.
            // ----------------------------------------
            data: 'zasuvka',
            render: function (data, type, row) {
                var zasuvkaHtml = '';
                if (data == true) {
                    zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                } else if (data == false) {
                    zasuvkaHtml = '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                }
                return zasuvkaHtml;
            }
        }
    ],
    // ----------------------------------------
    // Apply custom row styling based on the data.
    // ----------------------------------------
    rowCallback: function (row, data, index) {
        var today = new Date().setHours(0, 0, 0, 0);
        var startDate = new Date(data.odstavkaZacatek).setHours(0, 0, 0, 0);
        var minDateString = "0001-01-01T00:00:00";
        var zadanVstup = data.zadanVstup && data.zadanVstup !== minDateString;
        var zadanOdchod = data.zadanOdchod && data.zadanOdchod !== minDateString;

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
    paging: true,
    searching: true,
    ordering: true,
    lengthChange: false,
    pageLength: 9
});
