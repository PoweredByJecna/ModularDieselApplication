// ----------------------------------------
// Initialize the DataTable for displaying Pohotovosti records.
// ----------------------------------------
$('#pohotovostTable').DataTable({
    ajax: {
        // ----------------------------------------
        // Configure the AJAX request to fetch data from the server.
        // ----------------------------------------
        url: '/Pohotovosti/GetTableDatapohotovostiTable', // Server-side method to fetch data.
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
            // Combine the technician's first and last name into one cell.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return data.jmenoTechnika + ' ' + data.prijmeniTechnika;
            }
        },
        {
            // ----------------------------------------
            // Render the technician's phone number.
            // ----------------------------------------
            data: 'telTechnika'
        },
        {
            // ----------------------------------------
            // Render the technician's company name.
            // ----------------------------------------
            data: 'firmaTechnika'
        },
        {
            // ----------------------------------------
            // Format the start date of the Pohotovost.
            // ----------------------------------------
            data: 'zacetekPohotovosti',
            render: function (data) {
                return formatDate(data);
            }
        },
        {
            // ----------------------------------------
            // Format the end date of the Pohotovost.
            // ----------------------------------------
            data: 'konecPohotovosti',
            render: function (data) {
                return formatDate(data);
            }
        },
        {
            // ----------------------------------------
            // Render a clickable link to view the user's details.
            // ----------------------------------------
            data: null,
            render: function (data, type, row) {
                return `
                    <a class="userA" href="/User/Index?id=${data.idUser}">
                    Obsazenost <i class="fa-solid fa-eye"></i>
                    </a>`;
            }
        }
    ],
    // ----------------------------------------
    // Apply custom row styling based on the technician's status.
    // ----------------------------------------
    rowCallback: function (row, data, index) {
        if (data.firmaTechnika === 'Fiktivni') {
            $(row).addClass('row-neprirazeno'); // Mark rows with "Fiktivni" company as unassigned.
        } else if (data.technikStatus === true) {
            $(row).addClass('row-obsazeny'); // Mark rows with active technicians as occupied.
        } else {
            $(row).addClass('row-volny'); // Mark rows with inactive technicians as free.
        }
    },
    paging: true,        // Enable pagination.
    searching: true,     // Enable searching.
    ordering: false,     // Disable column ordering.
    lengthChange: false, // Disable length change.
    pageLength: 15       // Set the number of rows per page.
});