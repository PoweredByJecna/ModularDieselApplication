// ----------------------------------------
// Initialize the DataTable for displaying unassigned Dieslovani records.
// ----------------------------------------
$('#thrashTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Dieslovani/GetTableDatathrashTable', // Server-side method to fetch data.
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
            // Render a badge indicating the record is "Nepřiřazeno" (Unassigned).
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `
                    <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: orange; border-radius: 5px;">
                        <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Nepřiřazeno</span>
                        <i class="fa-solid fa-clock-rotate-left" style="color: black;"></i>
                    </span>
                `;
            }
        },
        {
            // ----------------------------------------
            // Render a button to allow the user to take over the Dieslovani record.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="Take(${row.id})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Převzít</span>
                    <i class="fa-solid fa-user-plus"></i>
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
            // Render the technician's company name.
            // ----------------------------------------
            data: 'technikFirma'
        }
    ],
    // ----------------------------------------
    // Apply custom row styling for unassigned records.
    // ----------------------------------------
    rowCallback: function (row, data, index) {
        $(row).addClass('row-neprirazeno'); // Mark rows as unassigned.
    },
    paging: true,        // Enable pagination.
    searching: true,     // Enable searching.
    ordering: false,     // Disable column ordering.
    lengthChange: false, // Disable length change.
    pageLength: 4        // Set the number of rows per page.
});