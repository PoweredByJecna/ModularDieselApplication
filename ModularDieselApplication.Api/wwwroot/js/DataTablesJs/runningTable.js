$('#runningTable').DataTable({
    ajax: {
        

        url: '/Dieslovani/GetTableDataRunningTable', // Cesta na vaši serverovou metodu
        type: 'GET',
        dataSrc: function (json) {
            // Zkontrolujte, co se vrací z API
            console.log(json);
            return json.data;
        }
    },
    columns: [
        {
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
            data: null,
            render: function (data, type, row) {
                return `       
                <span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: green; border-radius: 5px; cursor: pointer" onclick="Odchod(${row.id})">
                    <span class="badge-label" style="color: white; padding: 1px; font-size: small;">Odchod</span>
                    <i class="fa-solid fa-person-walking-arrow-right"></i>
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
        { 
            data: null, 
                    render: function(data, type, row) {
                    return `<a class="userA" href="/User/Index?id=${data.user}">
                    ${data.jmenoTechnika} ${data.prijmeniTechnika}
                </a>`;}
        },
        { data: 'zadanVstup', 
            render: function(data) {
                return formatDate(data);
            } },
        {data:'popis'},
        {data: 'vydrzBaterie'},    
        {
            data: 'zasuvka',
            render: function (data, type, row) {
                var zasuvkaHtml = '';
                if (data == true) {
                    zasuvkaHtml = '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>';
                } else if (data == false) {
                    zasuvkaHtml = '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                }
                return zasuvkaHtml;
            }
        },
       
        

    ],  
    rowCallback: function(row, data, index) {
    $(row).addClass('row-aktivni');
    },
    paging: true,        
    searching: true,
    ordering: false,  
    lengthChange: false,    
    pageLength: 3
    });        // Počet řádků na stránku