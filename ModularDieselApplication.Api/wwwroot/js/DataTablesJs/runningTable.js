// ----------------------------------------
// Initialize the DataTable for displaying active Dieslovani records.
// ----------------------------------------
$('#runningTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Dieslovani/GetTableDataRunningTable', // Server-side method to fetch data.
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
            // Render a badge indicating the record is "Aktivní" (Active).
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px;">
                        <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Aktivní</span>
                        <i class="fa-solid fa-clock-rotate-left" style="color: Black;"></i>
                    </span>
                `;
            }
        },
        {
            // ----------------------------------------
            // Render a button to mark the technician's departure (Odchod).
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: green; border-radius: 5px; cursor: pointer" onclick="Odchod(${row.id})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Odchod</span>
                    <i class="fa-solid fa-person-walking-arrow-right"></i>
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
            // Format the entry date (ZadanVstup).
            // ----------------------------------------
            data: 'zadanVstup',
            render: function (data) {
                return formatDate(data);
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
    // Apply custom row styling for active records.
    // ----------------------------------------
    rowCallback: function (row, data, index) {
        $(row).addClass('row-aktivni'); // Mark rows as active.
    },
    paging: true,        // Enable pagination.
    searching: true,     // Enable searching.
    ordering: false,     // Disable column ordering.
    lengthChange: false, // Disable length change.
    pageLength: 3        // Set the number of rows per page.
});