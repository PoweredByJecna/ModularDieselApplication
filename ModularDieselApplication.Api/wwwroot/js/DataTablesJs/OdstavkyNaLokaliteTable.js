$(document).ready(function () {
    // ----------------------------------------
    // Function to get the Lokalita name (nazev) from the URL.
    // ----------------------------------------
    function getOdstavkyFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get("nazev"); // Returns the value of the 'nazev' parameter.
    }

    // ----------------------------------------
    // Initialize the DataTable for displaying Odstavky on a specific Lokalita.
    // ----------------------------------------
    $('#OdstavkyNaLokaliteTable').DataTable({
        ajax: {
            // ----------------------------------------
            // Configure the AJAX request to fetch data from the server.
            // ----------------------------------------
            url: '/Lokality/GetOdstavkynaLokalite', // Server-side method to fetch data.
            type: 'GET',
            data: function (d) {
                const nazev = getOdstavkyFromUrl();
                console.log("ID odesláno na server:", nazev); // Debugging log.
                d.nazev = nazev; // Add the Lokalita name to the request data.
            },
            dataSrc: function (json) {
                console.log("Data vrácená serverem:", json); // Debugging log.
                return json.data;
            }
        },
        columns: [
            {
                // ----------------------------------------
                // Render a clickable link for the Odstavka ID.
                // ----------------------------------------
                data: 'odstavkaId',
                render: function (data) {
                    return `
                        <a href="/Odstavky/DetailOdstavky?id=${data}">
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
                    let logo = '';
                    switch (data) {
                        case 'ČEZ':
                            logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            break;
                        case 'EGD':
                            logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            break;
                        case 'PRE':
                            logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            break;
                        default:
                            logo = '';
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
                    return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.lokalita}">
                    ${data.lokalita}</a>`;
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
            {
                // ----------------------------------------
                // Format the start date of the Odstavka.
                // ----------------------------------------
                data: 'zacatekOdstavky',
                render: data => formatDate(data)
            },
            {
                // ----------------------------------------
                // Format the end date of the Odstavka.
                // ----------------------------------------
                data: 'konecOdstavky',
                render: data => formatDate(data)
            },
            {
                // ----------------------------------------
                // Render the address of the Lokalita.
                // ----------------------------------------
                data: 'adresa'
            },
            {
                // ----------------------------------------
                // Render the description (Popis) column.
                // ----------------------------------------
                data: 'popis'
            },
            {
                // ----------------------------------------
                // Render an icon indicating whether a socket (Zásuvka) is available.
                // ----------------------------------------
                data: 'zasuvka',
                render: function (data) {
                    return data
                        ? '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>'
                        : '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                }
            }
        ],
        // ----------------------------------------
        // Apply custom row styling based on the data.
        // ----------------------------------------
        rowCallback: function (row, data) {
            const today = new Date().setHours(0, 0, 0, 0);
            const startDate = new Date(data.od).setHours(0, 0, 0, 0);

            if (data.zadanOdchod && !data.zadanVstup) {
                $(row).addClass('row-ukoncene');
            } else if (data.zadanVstup && !data.zadanOdchod) {
                $(row).addClass('row-aktivni');
            } else if (!data.zadanOdchod && !data.zadanVstup && today === startDate && data.idTechnika !== "606794494" && data.idTechnika != null) {
                $(row).addClass('row-cekajici');
            } else if (!data.idTechnika) {
                $(row).addClass('row-nedieslujese');
            } else if (data.idTechnika === "606794494") {
                $(row).addClass('row-neprirazeno');
            } else {
                $(row).addClass('row-standart');
            }
        },
        paging: false,        // Disable pagination.
        searching: false,     // Disable searching.
        ordering: false,      // Disable ordering.
        lengthChange: false,  // Disable length change.
        pageLength: 1         // Number of rows per page.
    });
});
