    function formatTimestamp(timestamp) {
    const date = new Date(timestamp);
    const year = date.getFullYear();
    const month = ("0" + (date.getMonth() + 1)).slice(-2);
    const day = ("0" + date.getDate()).slice(-2);
    const hours = ("0" + date.getHours()).slice(-2);
    const minutes = ("0" + date.getMinutes()).slice(-2);
    return `${year}-${month}-${day} ${hours}:${minutes}`;
}



$(document).ready(function () {
    // Získání parametru "id" z query stringu (např. ?id=156587)
    const params = new URLSearchParams(window.location.search);
    const id = params.get("id");    
    
    console.log("ID z URL:", id);
    
    if (id) {
        $.ajax({
            url: '/LogApi/GetLogByEntity',
            type: 'GET',
            data: {
                entityId: id
            },
            success: function (response) {
                let html = "<ul>";
                console.log("Data:", response.data);
                if (response.data && response.data.length > 0) {
                    response.data.forEach(function (log) {
                        const formattedTime = formatTimestamp(log.timeStamp);
                        html += '<li style="padding:10px;">' +
                                '<a style="font-size:small;">' +
                                '<i class="fa-regular fa-clock"></i> ' +
                                '<strong>' + formattedTime + '</strong> ' +
                                '<i class="fa-solid fa-message"></i> ' +
                                log.logMessage +
                                '</a>' +
                                '</li>';
                    });
                } else {
                    html += "<li>Nebyly nalezeny žádné logy.</li>";
                }
                html += "</ul>";
                $("#logsContainer").html(html);
            },
            error: function () {
                $("#logsContainer").html("<p>Chyba při načítání logů.</p>");
            }
        });
    } else {
        $("#logsContainer").html("<p>ID nebylo nalezeno v URL.</p>");
    }
});


