$(document).ready(function () {
    // Získání ID z URL
    function getDieslovaniIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get("id"); // Vrátí hodnotu parametru 'id'
    }

    $('#OdDetail').DataTable({
        ajax: {
            url: '/Odstavky/GetTableDataOdDetail',
            type: 'GET',
            data: function (d) {
                const id = getDieslovaniIdFromUrl();
                console.log("ID odesláno na server:", id); // Pro ladění
                d.id = id; // Přidání ID do dotazu
            },
            dataSrc: function (json) {
                console.log("Data vrácená serverem:", json); // Pro ladění
                return json.data;
            }
        },
      
        columns: [
            
            { data: 'id',
                render: function(data)
                {
                    return `
                        <a href="/Odstavky/DetailOdstavky?id=${data}">
                        ${data}
                        </a>
                     `;
                }
            },
            {
                data: 'distributor',
                render: function (data, type, row) {
                    let logo = '';
                    switch (data) {
                        case 'ČEZ':
                            logo = '<img src="/Images/CEZ-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            break;
                        case 'EGD':
                            logo = '<img src="/Images/EGD-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            break;
                        case 'PRE':
                            logo = '<img src="/Images/PRE-Logo.jpg" width="25" height="25" style="border-radius: 20px; border: 0.5px solid grey;">';
                            break;
                        default:
                            logo = '';
                    }
                    return logo;
                }
            },
            { data: null,
                render: function (data, type, row) {
                    return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.nazevLokality}">
                    ${data.nazevLokality}</a>`;
                } 
            },
            {
                data: 'klasifikace',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
                }   
            },
            { data: 'zacatekOdstavky', render: data => formatDate(data) },
            { data: 'konecOdstavky', render: data => formatDate(data) },
            { data: 'adresa' },
            { data: 'vydrzBaterie' },
            { data: 'popis' },
            {
                data: 'zasuvka',
                render: function (data) {
                    return data
                        ? '<i class="fa-solid fa-circle-check socket-icon" style="color: #51fe06;"></i>'
                        : '<i class="fa-solid fa-ban" style="color: #ea0606;"></i>';
                }
            }
        ],
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
        paging: false,
        searching: false,
        ordering: false,
        lengthChange: false,
        pageLength: 1 // Počet řádků na stránku
    });
});
