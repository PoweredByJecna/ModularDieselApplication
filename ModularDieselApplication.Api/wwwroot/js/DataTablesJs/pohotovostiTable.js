$('#pohotovostTable').DataTable({ajax: {
    url: '/Pohotovosti/GetTableDatapohotovostiTable', // Cesta na vaši serverovou metodu
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
            render: function(data, type, row) {
                return data.jmenoTechnika
                + ' ' + data.prijmeniTechnika; // Combine the two variables into one cell
            }
        },
    {   data: 'telTechnika'},
    {   data: 'firmaTechnika'},
    {   data: 'zacetekPohotovosti',
        render: function(data) {
            return formatDate(data);
        }
    },
    {   data: 'konecPohotovosti',
        render: function(data) {
            return formatDate(data);
        }
    },
    {
        data: null, 
        render: function(data, type, row) {
        return `<a class="userA" href="/User/Index?id=${data.idUser}">
        Obsazenost <i class="fa-solid fa-eye"></i>
        </a>`;}
    }
],
rowCallback: function(row, data, index) {
    if(data.firmaTechnika =='Fiktivni') {
        $(row).addClass('row-neprirazeno');
    }
    else if (data.technikStatus == true) {
        $(row).addClass('row-obsazeny');
    }else {
        $(row).addClass('row-volny');
    }
},
    paging: true,        
    searching: true,
    ordering: false, 
    lengthChange: false,     
    pageLength: 15
});    