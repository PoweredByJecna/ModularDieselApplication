$(document).ready(function () {
    // ----------------------------------------
    // Function to get the Dieslovani ID from the URL.
    // ----------------------------------------
    function getDieslovaniIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get("id"); // Returns the value of the 'id' parameter.
    }

    // ----------------------------------------
    // Initialize the DataTable for Diesel Detail.
    // ----------------------------------------
    $('#DieselDetailTable').DataTable({
        ajax: {
            // ----------------------------------------
            // Configure the AJAX request to fetch data from the server.
            // ----------------------------------------
            url: '/Dieslovani/GetTableDataDetailDieslovani', // Server-side method to fetch data.
            type: 'GET',
            data: function (d) {
                const id = getDieslovaniIdFromUrl();
                console.log("ID sent to the server:", id); // Debugging log.
                d.id = id; // Add the ID to the request data.
            },
            dataSrc: function (json) {
                console.log("Data returned by the server:", json); // Debugging log.
                return json.data;
            }
        },
        columns: [
            {
                // ----------------------------------------
                // Render a clickable link for the Dieslovani ID.
                // ----------------------------------------
                data: 'iDdieslovani',
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
                data: 'distrib',
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
                data: 'klasifikaceLokality',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
                }
            },
            {
                // ----------------------------------------
                // Render a clickable link for the technician's name.
                // ----------------------------------------
                data: null,
                render: function (data, type, row) {
                    return `<a class="userA" href="/User/Index?id=${data.userId}">
                    ${data.jmenoTechnikaDA} ${data.prijmeniTechnikaDA}
                    </a>`;
                }
            },
            {
                // ----------------------------------------
                // Format the entry date (Vstup) or display a dash if not set.
                // ----------------------------------------
                data: 'vstupnaLokalitu',
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
                // Format the exit date (Odchod) or display a dash if not set.
                // ----------------------------------------
                data: 'odchodzLokality',
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
                // Render the address of the Lokalita.
                // ----------------------------------------
                data: 'adresaLokality'
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
        paging: false, // Disable pagination.
        searching: false, // Disable searching.
        ordering: false, // Disable ordering.
        lengthChange: false, // Disable length change.
        pageLength: 1 // Number of rows per page.
    });

    // ----------------------------------------
    // Function to get the Lokalita name (nazev) from the URL.
    // ----------------------------------------
    function getLokalitaNazevFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get("nazev"); // Returns the value of the 'nazev' parameter.
    }

    // ----------------------------------------
    // Initialize the DataTable for Dieslovani on a specific Lokalita.
    // ----------------------------------------
    $('#DieslovaniNaLokaliteTable').DataTable({
        ajax: {
            // ----------------------------------------
            // Configure the AJAX request to fetch data from the server.
            // ----------------------------------------
            url: '/Lokality/GetDieslovaniNaLokalite', // Server-side method to fetch data.
            type: 'GET',
            data: function (d) {
                const nazev = getLokalitaNazevFromUrl();
                console.log("ID sent to the server:", nazev); // Debugging log.
                d.nazev = nazev; // Add the Lokalita name to the request data.
            },
            dataSrc: function (json) {
                console.log("Data returned by the server:", json); // Debugging log.
                return json.data;
            }
        },
        columns: [
            {
                // ----------------------------------------
                // Render a clickable link for the Dieslovani ID.
                // ----------------------------------------
                data: 'iDdieslovani',
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
                data: 'distrib',
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
                data: 'klasifikaceLokality',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
                }
            },
            {
                // ----------------------------------------
                // Render a clickable link for the technician's name.
                // ----------------------------------------
                data: null,
                render: function (data, type, row) {
                    return `<a class="userA" href="/User/Index?id=${data.userId}">
                    ${data.jmenoTechnikaDA} ${data.prijmeniTechnikaDA}
                    </a>`;
                }
            },
            {
                // ----------------------------------------
                // Format the entry date (Vstup) or display a dash if not set.
                // ----------------------------------------
                data: 'vstupnaLokalitu',
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
                // Format the exit date (Odchod) or display a dash if not set.
                // ----------------------------------------
                data: 'odchodzLokality',
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
                // Render the address of the Lokalita.
                // ----------------------------------------
                data: 'adresaLokality'
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
        paging: false, // Disable pagination.
        searching: false, // Disable searching.
        ordering: false, // Disable ordering.
        lengthChange: false, // Disable length change.
        pageLength: 1 // Number of rows per page.
    });
});
