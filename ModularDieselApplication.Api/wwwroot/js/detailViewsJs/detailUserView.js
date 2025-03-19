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
        const params = new URLSearchParams(window.location.search);
        const id = params.get("id");
        console.log(data);
        console.log(id);

        if (id) {
            $.ajax({
                url: `/User/DetailUserJson?id=${id}`,
                type: 'GET',
                
                success: function (response) {
                    const data = response.data;
                    console.log(response.data);
                    if (data) {
                        $('#uzivatelskeJmeno').append(data.uzivatelskeJmeno);
                        const badgeHtml = data.stav 
                        ? `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: red; border-radius: 5px;"> 
                                <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Obsazen</span>
                        </span>`
                        : `<span class="badge badge-phoenix fs-10 badge-phoenix-success" style="background-color: green; border-radius: 5px;"> 
                                <span class="badge-label" style="color: black; padding: 1px; font-size: small;">Volný</span>
                        </span>`;
                        $('#stav').append(badgeHtml);  
                        $('#nadrizeny').append(data.nadrizeny);                  
                        $('#firma').append(data.firma);
                        $('#region').append(data.region);
                        $('#jmeno').append(data.jmeno);
                        $('#prijmeni').append(data.prijmeni);
                        $('#tel').append(data.tel);
                        $('#role').append(data.role);
                        if (data.pohotovostZacatek == null && data.pohotovostKonec == null) {
                            $('#pohotovost').append("Technik nemá vypsanou pohotovost");
                        } else {
                            const formattedZacatek = formatTimestamp(data.pohotovostZacatek);
                            const formattedKonec = formatTimestamp(data.pohotovostKonec);
                            $('#pohotovost').append(`Pohotovost od ${formattedZacatek} do ${formattedKonec}`);
                        }
                        
                        

                    } else {
                        $('#user-detail').html('<p>Data nebyla nalezena.</p>');
                    }
                },
                error: function () {
                    $('#user-detail').html('<p>Chyba při načítání dat.</p>');
                }
            });
        } else {
            $('#user-detail').html('<p>ID dieslování nebylo poskytnuto.</p>');
        }
    });