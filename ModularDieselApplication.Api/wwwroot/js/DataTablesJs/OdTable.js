// ----------------------------------------
// Initialize the DataTable for displaying Odstavky records.
// ----------------------------------------
$('#odTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Odstavky/GetTableData', // Server-side method to fetch data.
        type: 'POST',
        dataSrc: function (json) {
            // Log the server response for debugging.
            console.log(json);
            return json.data; // Return the data array from the server response.
        }
    },
    columns: [
        {
            // ----------------------------------------
            // Render a button to close the Odstavka record.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="deleteRecord(this, ${row.id})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                    <i class="fa-solid fa-xmark"></i>
                </span>`;
            }
        },
        {
            // ----------------------------------------
            // Render a clickable link for the Odstavka ID.
            // ----------------------------------------
            data: 'id',
            render: function (data) {
                return `
                    <a href="/Odstavky/DetailOdstavky?id=${data}">
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
            // Render a clickable link for the Lokalita name.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.nazevLokality}">
                ${data.nazevLokality}</a>`;
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
            render: function (data) {
                return formatDate(data);
            }
        },
        {
            // ----------------------------------------
            // Format the end date of the Odstavka.
            // ----------------------------------------
            data: 'konecOdstavky',
            render: function (data) {
                return formatDate(data);
            }
        },
        {
            // ----------------------------------------
            // Render the address of the Lokalita.
            // ----------------------------------------
            data: 'adresa'
        },
        {
            // ----------------------------------------
            // Render the battery capacity.
            // ----------------------------------------
            data: 'vydrzBaterie'
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
        const startDate = new Date(data.zacatekOdstavky).setHours(0, 0, 0, 0);
        const minDateString = "0001-01-01T00:00:00";
        const zadanVstup = data.zadanVstup !== minDateString;
        const zadanOdchod = data.zadanOdchod !== minDateString;

        if (zadanVstup && !zadanOdchod) {
            $(row).addClass('row-aktivni');
        } else if (!zadanOdchod && !zadanVstup && today === startDate && data.idTechnika !== "606794494" && data.idTechnika != null) {
            $(row).addClass('row-cekajici');
        } else if (data.idTechnika == null) {
            $(row).addClass('row-nedieslujese');
        } else if (data.idTechnika === "606794494") {
            $(row).addClass('row-neprirazeno');
        } else if (zadanOdchod && zadanVstup && data.idTechnika !== "606794494" && data.idTechnika != null) {
            $(row).addClass('row-ukoncene');
        } else {
            $(row).addClass('row-standart');
        }
    },
    paging: true,        // Enable pagination.
    searching: true,     // Enable searching.
    ordering: true,      // Enable column ordering.
    lengthChange: false, // Disable length change.
    pageLength: 15       // Set the number of rows per page.
});