// ----------------------------------------
// Initialize the DataTable for displaying Lokality records.
// ----------------------------------------
$('#lokalityTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Lokality/GetTableData', // Server-side method to fetch data.
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
            // Render a clickable link for the Lokalita name.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.nazev}">
                ${data.nazev}</a>`;
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
            // Render the address of the Lokalita.
            // ----------------------------------------
            data: 'adresa' 
        },
        { 
            // ----------------------------------------
            // Render the region name.
            // ----------------------------------------
            data: 'region' 
        },
        { 
            // ----------------------------------------
            // Render the battery capacity.
            // ----------------------------------------
            data: 'baterie' 
        },
        {
            // ----------------------------------------
            // Render an icon indicating whether a socket (Zásuvka) is available.
            // ----------------------------------------
            data: 'zasuvka',
            render: function (data) {
                if (data == true) {
                    return '<i class="fa-solid fa-circle-check" style="color: #51fe06;"></i>';
                } else {
                    return '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                }
            }
        },
        {
            // ----------------------------------------
            // Render an icon indicating whether a stationary generator (DA) is available.
            // ----------------------------------------
            data: 'da',
            render: function (data) {
                if (data === "TRUE") {
                    return '<i class="fa-solid fa-circle-check" style="color: #51fe06;"></i>';
                }
                return '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
            }
        },
        {
            // ----------------------------------------
            // Render the source (Zdroj) column.
            // ----------------------------------------
            data: 'zdroj'
        }
    ],
    // ----------------------------------------
    // Configure pagination and table behavior.
    // ----------------------------------------
    pageLength: 20, // Set the number of rows per page.
    lengthChange: false, // Disable the ability to change the number of rows per page.
    ordering: false // Disable column ordering.
}).on('draw', function () {
    // ----------------------------------------
    // Adjust the pagination and wrapper styles after the table is drawn.
    // ----------------------------------------
    $('#lokalityTable_wrapper .dataTables_paginate').css({
        position: 'absolute',
        bottom: '4px',
        right: '10px'
    });
    $('#lokalityTable_wrapper').css({
        position: 'relative',
        height: '455px' // Set the height for the table wrapper.
    });
});
