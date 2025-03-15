$(document).ready(function () {
    // Získání ID z URL
    function getDieslovaniIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get("nazev"); 
    }

    $('#DieslovaniNaLokaliteTable').DataTable({
        ajax: {
            url: '/Lokality/GetDieslovaniNaLokalite',
            type: 'GET',
            data: function (d) {
                const nazev = getDieslovaniIdFromUrl();
                console.log("ID odesláno na server:", nazev); // Pro ladění
                d.nazev = nazev; // Přidání ID do dotazu
            },
            dataSrc: function (json) {
                console.log("Data vrácená serverem:", json); // Pro ladění
                return json.data;
            }
        },
      
        columns: [
            { data: 'iDdieslovani',
                render: function (data, type, row) {
                    return `
                        <a href="/Dieslovani/DetailDieslovani?id=${data}">
                        ${data}
                        </a>
                    `;
                }
            },
            {
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
            { data: null,
                render: function (data, type, row) {
                    return `<a style="font-weight: 700;" href="/Lokality/DetailLokality?nazev=${data.lokalitaNazev}">
                    ${data.lokalitaNazev}</a>`;
                } 
            },
            {
                data: 'klasifikaceLokality',
                render: function (data, type, row) {
                    return `<span style="font-weight: 700;">${data}</span>`;
                }
            },
            { 
                data: null, 
                            render: function(data, type, row) {
                            return `<a class="userA" href="/User/Index?id=${data.userId}">
                            ${data.jmenoTechnikaDA} ${data.prijmeniTechnikaDA}
                        </a>`;}
            },    
            {
                data: 'vstupnaLokalitu',
                render: function(data) {
                    // Zkontroluje, jestli je datum ve formátu "01.01.1 00:00" nebo je null/undefined
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "-";  // Zobrazí pomlčku
                    } else {
                        return formatDate(data);  // Jinak použije formátování
                    }
                }
            },
            {data: 'odchodzLokality', 
                render: function(data) {
                    // Zkontroluje, jestli je datum ve formátu "01.01.1 00:00" nebo je null/undefined
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "-";  // Zobrazí pomlčku
                    } else {
                        return formatDate(data);  // Jinak použije formátování
                    }
                }
            }, 
            {data:'adresaLokality'}
         
                   
              
        
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
        pageLength: 1
    });
});
