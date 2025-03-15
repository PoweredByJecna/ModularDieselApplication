
$('#thrashTable').DataTable({ajax: {
    url: '/Dieslovani/GetTableDatathrashTable', // Cesta na vaši serverovou metodu
    type: 'GET',
    dataSrc: function (json) {
        // Zkontrolujte, co se vrací z API
        console.log(json);
        return json.data;
    }
    },
    columns:[
    {
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
        data: null,
        render: function (data, type, row) {
            return `       
            <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: #28a745; border-radius: 5px; cursor: pointer" onclick="Take(${row.id})">
                <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Převzít</span>
                <i class="fa-solid fa-user-plus"></i>
            </span>  
        `;

        
    }
    }, 
    { data: 'id',
        render: function (data, type, row) {
            return `
                 <a href="/Dieslovani/DetailDieslovani?id=${data}">
                ${data}
                </a>
            `;
        }
    },
    {
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
    { data: null,
        render: function (data, type, row) {
            return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.lokalitaNazev}">
            ${data.lokalitaNazev}</a>`;
        } 
    },
    {
        data: 'klasifikace',
        render: function (data, type, row) {
            return `<span style="font-weight: 700;">${data}</span>`;
        }
    },
    {data: 'technikFirma'},
    ],
    rowCallback: function(row, data, index) {
        $(row).addClass('row-neprirazeno');
    },
        paging: true,        
        searching: true,
        ordering: false, 
        lengthChange: false,     
        pageLength: 4
    });    