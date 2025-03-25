// ----------------------------------------
// Initialize the DataTable for displaying upcoming Dieslovani records.
// ----------------------------------------
$('#upcomingTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Dieslovani/GetTableUpcomingTable', // Server-side method to fetch data.
        type: 'GET',
        dataSrc: function (json) {
            // Log the server response for debugging.
            console.log(json);
            return json.data; // Return the data array from the server response.
        }
    },
    columns: [
        {
            // ----------------------------------------
            // Render a badge indicating the record is "Čekající" (Waiting).
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: yellow; border-radius: 5px;">
                        <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Čekající</span>
                        <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                    </span>
                `;
            }
        },
        {
            // ----------------------------------------
            // Render a button to mark the technician's entry (Vstup).
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="Vstup(${row.id})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Vstup</span>
                    <i class="fa-solid fa-person-walking-arrow-right fa-flip-horizontal"></i>
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
        {
            // ----------------------------------------
            // Format the start date of the Odstavka and add a warning icon if overdue.
            // ----------------------------------------
            data: 'odstavkaZacatek',
            render: function (data) {
                let odstavkaDate = new Date(data);
                let now = new Date();
                let warningIcon = '';

                if (odstavkaDate < now) {
                    warningIcon = ' <i class="fa-solid fa-exclamation fa-lg" style="color: #e31b0d;"></i>';
                }

                return formatDate(data) + warningIcon;
            }
        },
        {
            // ----------------------------------------
            // Render the description (Popis) column.
            // ----------------------------------------
            data: 'popis'
        },
        {
            // ----------------------------------------
            // Render the battery capacity.
            // ----------------------------------------
            data: 'vydrzBaterie'
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
    // Apply custom row styling for waiting records.
    // ----------------------------------------
    rowCallback: function (row, data, index) {
        $(row).addClass('row-cekajici'); // Mark rows as waiting.
    },
    paging: true,        // Enable pagination.
    searching: true,     // Enable searching.
    ordering: false,     // Disable column ordering.
    lengthChange: false, // Disable length change.
    pageLength: 4        // Set the number of rows per page.
});

