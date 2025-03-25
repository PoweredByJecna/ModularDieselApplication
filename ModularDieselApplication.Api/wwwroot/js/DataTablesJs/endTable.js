// ----------------------------------------
// Initialize the DataTable for displaying ended Dieslovani records.
// ----------------------------------------
$('#endTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Dieslovani/GetTableDataEndTable', // Server-side method to fetch data.
        type: 'GET',
        dataSrc: function (json) {
            console.log(json); // Log the server response for debugging.
            return json.data; // Return the data array from the server response.
        }
    },
    columns: [
        {
            // ----------------------------------------
            // Render a badge indicating the record is "Ukončeno" (Ended).
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: red; border-radius: 5px;">
                        <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Ukončeno</span>
                        <i class="fa-solid fa-circle-check" style="color: Black;"></i>
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
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="deleteRecordDieslovani(this, ${row.idDieslovani})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Uzavřít</span>
                    <i class="fa-solid fa-xmark"></i>
                </span>  
                `;
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
            // Render the classification (Klasifikace) column with a color-coded badge.
            // ----------------------------------------
            data: 'klasifikace',
            render: function (data, type, row) {
                var klasifikaceBadge = '';
                var colorMap = {
                    'A1': '#c91829',
                    'A2': 'orange',
                    'B1': 'yellow',
                    'B2': 'lightgreen',
                    'B': 'green',
                    'C': 'green',
                    'D1': 'blue'
                };
                if (colorMap[data]) {
                    klasifikaceBadge = `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: ${colorMap[data]}; border-radius: 5px;">
                        <span class="badge-label" style="color: black; padding: 2px; margin-right: 0px;">${data}</span>
                    </span>`;
                }
                return klasifikaceBadge;
            }
        },
        {
            // ----------------------------------------
            // Format the exit date (ZadanOdchod).
            // ----------------------------------------
            data: 'zadanOdchod',
            render: function (data) {
                return formatDate(data); // Format the date using a helper function.
            }
        }
    ],
    // ----------------------------------------
    // Apply custom row styling for ended records.
    // ----------------------------------------
    rowCallback: function (row, data, index) {
        $(row).addClass('row-ukoncene'); // Add a class to style ended rows.
    },
    paging: true,        // Enable pagination.
    searching: true,     // Enable searching.
    ordering: false,     // Disable ordering.
    lengthChange: false, // Disable length change.
    pageLength: 4        // Set the number of rows per page.
});
