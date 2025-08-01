﻿
// Přidej event listener na všechny inputy s třídou 'InputSearching'
document.querySelectorAll('.InputSearching').forEach(input => {
    input.addEventListener('input', function () {
        let query = this.value; // Získej hodnotu z aktuálního inputu
        let inputId = this.id; // Získej ID aktuálního inputu
        let endpoint = ''; // Nastav endpoint na základě ID inputu
        
        // Urči endpoint podle ID (přizpůsob si podle potřeby)
        if (inputId === 'search') {
            endpoint = '/Odstavky/Search';
        } else if (inputId === 'search-lokality') {
            endpoint = '/Lokality/Search';
        } else {
            console.error(`Neznámý input ID: ${inputId}`);
            return;
        }
        
        // Fetch výsledků
        fetch(`${endpoint}?query=${query}`)
            .then(response => response.text())
            .then(data => {
                document.getElementById('table-body').innerHTML = data; // Aktualizuj tabulku
                
                // Fetch pro stránkování
                fetch(`${endpoint.replace('Search', 'Paging')}?query=${query}`)
                    .then(pagingResponse => pagingResponse.text())
                    .then(pagingData => {
                        document.getElementById('paging-controls').innerHTML = pagingData; // Aktualizuj stránkování
                    });
            })
            .catch(error => console.error('Chyba při načítání dat:', error));
    });
});

/*--------------------------------------------
 * Nastavení výchozího zobrazení a přepínání mezi Odstavky a Dieslování
 */
document.addEventListener('DOMContentLoaded', function () {
  
    document.getElementById("odstavkyVazby").classList.add("active");
  
    document.getElementById("content-odstavkyVazby").style.display = "block";

    document.getElementById("content-dieslovaniVazby").style.display = "none";


    document.getElementById("odstavkyVazby").addEventListener("click", function () {
 
      this.classList.add("active");
  
      document.getElementById("dieslovaniVazby").classList.remove("active");
      
 
      document.getElementById("content-odstavkyVazby").style.display = "block";
      document.getElementById("content-dieslovaniVazby").style.display = "none";
    });


    document.getElementById("dieslovaniVazby").addEventListener("click", function () {

      this.classList.add("active");
     
      document.getElementById("odstavkyVazby").classList.remove("active");
   
      document.getElementById("content-odstavkyVazby").style.display = "none";
      document.getElementById("content-dieslovaniVazby").style.display = "block";
    });
});


/*--------------------------------------------
 * Nastavení výchozího zobrazení a přepínání mezi LOG a VAZBY
 */
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("log").classList.add("active");
  
    document.getElementById("content-log").style.display = "block";

    document.getElementById("content-vazby").style.display = "none";

    document.getElementById("log").addEventListener("click", function () {
      this.classList.add("active");
      document.getElementById("vazby").classList.remove("active");
      
      document.getElementById("content-log").style.display = "block";
      document.getElementById("content-vazby").style.display = "none";
    });

    document.getElementById("vazby").addEventListener("click", function () {
      this.classList.add("active");
      document.getElementById("log").classList.remove("active");
      document.getElementById("content-log").style.display = "none";
      document.getElementById("content-vazby").style.display = "block";
    });
});

/*--------------------------------------------
 * Získá ID dieslování z URL
 */
function getDieslovaniIdFromUrl() {
    const urlParts = window.location.pathname.split('/'); 
    return urlParts[urlParts.length - 1]; 
}

/*--------------------------------------------
 * Automatické obnovení stránky každých 5 minut
 */
setInterval(function () {
    location.reload();
}, 600000);

/*--------------------------------------------
 * Odešle email
 */
function sendEmail() {
    fetch('http://localhost:5025/api/email/send', {
        method: 'POST'
    })
    .then(response => response.text())
    .then(data => alert(data))
    .catch(error => alert('Chyba: ' + error));
}

/*--------------------------------------------
 * Formátuje datum do českého formátu
 */
function formatDate(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');

    return `${day}.${month}.${year} ${hours}:${minutes}`;
}

// Příklad použití při plnění tabulky
const data = [
    { date: '2024-12-08T17:18:36.3655145' },
    { date: '2024-11-30T10:45:12.1234567' }
];

// Vykreslování dat do tabulky
data.forEach(item => {
    const formattedDate = formatDate(item.date);
    console.log(formattedDate); // Pro ukázku: 08.12.2024 17:18
    // Zde vložte `formattedDate` do tabulky
});

/*--------------------------------------------
 * Zpracuje data regionu a vloží je do kontejnerů
 */
function processRegionData(region, containerIds) {
    $(containerIds.distributor).append(region.distributor);
    $(containerIds.firma).append(region.firma);
    $(containerIds.pocetOdstavek).append(region.pocetOdstavek);
    $(containerIds.pocetLokalit).append(region.pocetLokalit);
  
    $(containerIds.technici).empty();
    if (region.technici && region.technici.length > 0) {
      region.technici.forEach(function(tech) {
        var pohotovostIkona = (tech.maPohotovost === true)
          ? '<i class="fa-solid fa-circle" style="color: #28a745; margin-left: 10px"></i>'
          : '<i class="fa-solid fa-circle" style="color: #dc3545; margin-left: 10px"></i>';
  
        var pHtml = '<div> <span class="Infowrapperspan"><i class="fa-solid fa-wrench navA"></i> Technik: </span><p> '
                    + tech.jmeno
                    + ' ' + pohotovostIkona
                    + '</p></div>';
        $(containerIds.technici).append(pHtml);
      });
    } else {
      $(containerIds.technici).append('<p>Žádní technici</p>');
    }
}
/*--------------------------------------------
 * Volání metody pro objednání okamžitého dieslovaní na odstávku, kde dieslovaní objednáno nebylo
 */
function CallDieslovani(odstavkaId) {
    $.ajax({    
        url: '/Dieslovani/CallDieslovani',
        type: 'POST',
        data: { odstavkaId: odstavkaId },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    }); 
}    

/*--------------------------------------------
 * Přepínání zobrazení modálního okna uživatele
 */
document.addEventListener("DOMContentLoaded", function () {
    const userModal = document.getElementById("user-modal");
    const modalUser = document.getElementById("modal-user");

    userModal.addEventListener("click", function (event) {
        // Přepíná zobrazení modal-user
        modalUser.style.display = (modalUser.style.display === "block") ? "none" : "block";

        // Zastaví propagaci, aby modal nezmizel hned po kliknutí
        event.stopPropagation();
    });

    // Skryje modal při kliknutí mimo něj
    document.addEventListener("click", function (event) {
        if (!userModal.contains(event.target)) {
            modalUser.style.display = "none";
        }
    });
});

/*--------------------------------------------
 * Přepínání zobrazení postranního menu a ukládání stavu do localStorage
 */
const menuToggle = document.getElementById('menu-toggle');
const sideMenu = document.getElementById('sidemenu');
const con = document.getElementById('con');

// Načtení stavu z localStorage při načtení stránky
document.addEventListener('DOMContentLoaded', () => {
    const isVisible = localStorage.getItem('sidebarVisible');
    if (isVisible === 'true') {
        sideMenu.classList.add('visible');
        con.classList.add('visible');
    }
});

// Přidání event listeneru pro kliknutí na tlačítko
menuToggle.addEventListener('click', () => {
    sideMenu.classList.toggle('visible');
    con.classList.toggle('visible');

    // Ulož stav do localStorage
    const isVisible = sideMenu.classList.contains('visible');
    localStorage.setItem('sidebarVisible', isVisible);
});

/*--------------------------------------------
 * Provádí AJAX akci a aktualizuje tabulky po úspěchu
 */
function ajaxAction(url, data, successTables) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
                reloadTables(successTables);
            } else {
                showModal(response.message || 'Akce se nezdařila.', false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}

/*--------------------------------------------
 * Smaže záznam odstávky
 */
function deleteRecord(element, id) {
    const row = $(element).closest('tr');
    const offset = row.offset();
    var cislo1 = 50;
    var cislo2 = 100;

    $('#confirmModal').css({
        top: cislo1 + offset.top + row.height() + 'px', 
        left: cislo2 + offset.left + 'px',
        position: 'absolute'
    });
    showConfirmModal('Opravdu chcete smazat tento záznam?', function() {
        console.log("Mazání záznamu s ID:", id);
        ajaxAction('/Odstavky/Delete', { id: id }, ['#odTable']);
        reloadTables();
    });
}
/*--------------------------------------------
 * Smaže záznam dieslovani bez potrvení0
 */
function DeleteWithoutConfirm(id) {
    $.ajax({
        url: '/Dieslovani/Delete',
        type: 'POST',  
        data: { id: id },
        success: function (result) {
            if (result.success) {
                window.location.href = '/Dieslovani/Index'; 
                showModal(response.message, true);
            } else {
                alert(result.message || 'Nepodařilo se uzavřít / smazat záznam.');
            }
        },
        error: function () {
            alert('Nastala chyba při mazání/uzavírání záznamu.');
        }
    });
}

function DeleteWithoutConfirmOdstavka(id) {
    $.ajax({
        url: '/Odstavky/Delete',
        type: 'POST',  
        data: { id: id },
        success: function (result) {
            if (result.success) {
                window.location.href = '/Odstavky/Index'; 
                showModal(response.message, true);
            } else {
                alert(result.message || 'Nepodařilo se uzavřít / smazat záznam.');
            }
        },
        error: function () {
            alert('Nastala chyba při mazání/uzavírání záznamu.');
        }
    });
}

/*--------------------------------------------
 * Smaže záznam dieslování
 */
function deleteRecordDieslovani(element, id) {
    const row = $(element).closest('tr');
    const offset = row.offset();
    var cislo1 = 50;
    var cislo2 = 100;

    $('#confirmModal').css({
        top: cislo1 + offset.top + row.height() + 'px', 
        left: cislo2 + offset.left + 'px',
        position: 'absolute'
    });

    showConfirmModal('Opravdu chcete smazat tento záznam?', function() {
        console.log("Mazání záznamu s ID:", id);
        ajaxAction('/Dieslovani/Delete', { id: id }, [
            '#allTable',
            '#upcomingTable',
            '#endTable',
            '#runningTable',
            '#thrashTable'
        ]);
    });
}
function ChangeTime(dieslovaniId, timeValue, type) {
    $.ajax({
        url: '/Dieslovani/ChangeTime',
        type: 'POST',
        data: {
            dieslovaniId: dieslovaniId,
            time: timeValue,  
            type: type        
        },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}
function ChangeTimeOdstavky(odstavkaId, timeValue, type) {
    $.ajax({
        url: '/Dieslovani/ChangeTimeOdstavky',
        type: 'POST',
        data: {
            dieslovaniId: odstavkaId,
            time: timeValue,  
            type: type       
        },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}

function ChangePassword() {
    var userId = $('#userId').val();
    var newPassword = $('#newPassword').val();
    $.ajax({
        url: '/User/ChangePassword',
        type: 'POST',
        data: {
            userId: userId,
            newPassword: newPassword
        },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}

function AddUser() {
    var jmeno = $('#firstName').val();
    var prijmeni = $('#lastName').val();
    var username = $('#username').val();
    var email = $('#email').val();
    var password = $('#password').val();
    var role = $('#roleSelect').val();

    $.ajax({
        url: '/User/AddUser',
        type: 'POST',
        data: {
            email: email,
            password: password,
            role: role,
            username: username,
            Jmeno: jmeno,
            Prijmeni: prijmeni
        },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);  
                reloadTables(); 
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}

/*--------------------------------------------
 * Zaznamená vstup dieslování
 */
function Vstup(idDieslovani) {
    console.log("Vstup z lokality ID:", idDieslovani);      
    ajaxAction('/Dieslovani/Vstup', { idDieslovani: idDieslovani }, [
        '#allTable',
        '#upcomingTable',
        '#endTable',
        '#runningTable',
        '#thrashTable'
    ]);
}

/*--------------------------------------------
 * Zaznamená odchod dieslování
 */
function Odchod(idDieslovani) {
    console.log("Odchod z lokality ID:", idDieslovani);
    ajaxAction('/Dieslovani/Odchod', { idDieslovani: idDieslovani }, [
        '#allTable',
        '#upcomingTable',
        '#endTable',
        '#runningTable',
        '#thrashTable'
    ]);
}

/*--------------------------------------------
 * Převzetí dieslování
 */
function Take(idDieslovani) {
    ajaxAction('/Dieslovani/Take', { idDieslovani: idDieslovani }, [
        '#allTable',
        '#upcomingTable',
        '#endTable',
        '#runningTable',
        '#thrashTable'
    ]);
}

/*--------------------------------------------
 * Testovací AJAX akce
 */
$(document).ready(function () {
    $('#testButton').on('click', function () {
        $.ajax({
            url: '/Odstavky/Test', 
            type: 'POST', 
            success: function (response) {
                if (response.success) {
                    showModal(response.message, true);
                    reloadTables(); 
                } else {
                    showModal(response.message, false); // Chybová hláška
                }
            },
            error: function () {
                showModal('Neočekávaná chyba při komunikaci se serverem.', false);
            }
        });
    });
});

/*--------------------------------------------
 * Přepíná zobrazení inputu pro objednání na konkrétní datum
 */
function toggleObjednatNa() {
    const daOption = document.getElementById('daOption').value;
}

/*--------------------------------------------
    * Zapis pohotovosti
*/
function Zapis() {
    var Zacatek = document.getElementById('zacatek').value;
    var Konec = document.getElementById('konec').value;
    console.log("Zapis:", {
        Zacatek: Zacatek,
        Konec: Konec
    });
    // AJAX volání na server
    $.ajax({
        url: '/Pohotovosti/Zapis', 
        type: 'POST',
        data: {
            Zacatek: Zacatek,
            Konec: Konec
        },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
                reloadTables();
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}


/*--------------------------------------------
 * Vytvoří novou odstávku
 */
function CreateOdstavku() {
    var lokalita = document.getElementById('lokalita').value;
    var od = document.getElementById('od').value;
    var DO = document.getElementById('do').value;
    var popis = document.getElementById('popis').value;
    var daOption = document.getElementById('daOption').value; 

    console.log("Vytvoření odstávky:", {
        lokalita: lokalita,
        od: od,
        DO: DO,
        popis: popis,
        daOption: daOption
    });

    $.ajax({
        url: '/Odstavky/Create',
        type: 'POST',
        data: {
            lokalita: lokalita,
            od: od,
            do: DO,
            popis: popis,
            daOption: daOption
        },
        success: function (response) {
            if (response.success) {
                showModal(response.message, true);
                reloadTables();
            } else {
                showModal(response.message, false);
            }
        },
        error: function () {
            showModal('Došlo k chybě při komunikaci se serverem.', false);
        }
    });
}

/*--------------------------------------------
 * Navrhuje lokalitu na základě zadaného textu
 */
function SuggestLokalita() {
    var lokalita = document.getElementById('lokalita').value;
    if (lokalita.length <= 1) {
        document.getElementById('lokality-suggestions').style.display = 'none';
        return;
    }

    $.ajax({
        url: '/Odstavky/SuggestLokalita',
        type: 'GET',
        data: { query: lokalita },
        success: function (data) {
            var suggestions = document.getElementById('lokality-suggestions');
            suggestions.innerHTML = '';
            if (data.length > 0) {
                data.forEach(function (item) {
                    var div = document.createElement('div');
                    div.innerHTML = item;
                    div.classList.add('suggestion-item');
                    div.onclick = function () {
                        document.getElementById('lokalita').value = item;
                        suggestions.style.display = 'none';
                    };
                    suggestions.appendChild(div);
                });
                suggestions.style.display = 'block';
            } else {
                suggestions.style.display = 'none';
            }
        }
    });
}

/*--------------------------------------------
 * Zobrazuje modální okno s danou zprávou
 */
function showModal(message, isSuccess) {
    const modal = $('#messageModal');
    const modalContent = $('#modalContent');
    const modalText = $('#modalText');

    modalText.text(message);
    modalContent.removeClass('success error');

    if (isSuccess) {
        modalContent.addClass('success');
    } else {
        modalContent.addClass('error');
    }

    modal.fadeIn();

    $('#closeModal').on('click', function () {
        modal.fadeOut();
    });

    $(window).on('click', function (event) {
        if ($(event.target).is(modal)) {
            modal.fadeOut();
        }
    });
}

/*--------------------------------------------
 * Zobrazuje modální okno pro změnu hesla
 */
function showModalHeslo() {
    document.getElementById('modalOverlayheslo').style.display = 'block';
}

/*--------------------------------------------
 * Zavře modální okno pro změnu hesla
 */
function closeModalHeslo() {
    document.getElementById('modalOverlayheslo').style.display = 'none';
}

/*--------------------------------------------
 * Zobrazuje modální okno pro přidaní uživatele
 */

function showModalAddUser() {
    document.getElementById('modalOverlayAddUser').style.display = 'block';
}

/*--------------------------------------------
 * Zavře modální okno pro přidaní uživatele
 */
function closeModalAddUser() {  
    document.getElementById('modalOverlayAddUser').style.display = 'none';
}
/*--------------------------------------------
 * Zavře modální okno při kliknutí mimo něj
 */
window.onclick = function(event) {
    var modal = document.getElementById("modalHeslo");
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

/*--------------------------------------------
 * Zobrazuje potvrzovací modální okno s danou zprávou
 */
function showConfirmModal(message, onConfirm) {
    const confirmModal = $('#confirmModal');
    const confirmModalText = $('#confirmModalText');
    
    confirmModalText.text(message);
    confirmModal.fadeIn();
    
    $('#confirmBtn').off('click').on('click', function () {
        confirmModal.fadeOut();
        if (onConfirm && typeof onConfirm === 'function') {
            onConfirm();
        }
    });
    
    $('#cancelBtn').off('click').on('click', function () {
        confirmModal.fadeOut();
    });
    
    $('#closeConfirmModal').on('click', function () {
        confirmModal.fadeOut();
    });

    $(window).on('click', function (event) {
        if ($(event.target).is(confirmModal)) {
            confirmModal.fadeOut();
        }
    });
}

/*--------------------------------------------
 * Slouží pro rozbalení seznamu v sidebaru
 */

document.addEventListener('DOMContentLoaded', function () {
    const sidebars = document.querySelectorAll('.sidebar');

    sidebars.forEach(sidebar => {
        // Použijeme správnou třídu ikony
        const icon = sidebar.querySelector('.ChevronSideMenu');
        // Vybereme submenu podle třídy, nikoliv podle id
        const submenu = sidebar.querySelector('.ul-sidebar');
        const id = sidebar.getAttribute('data-id');

        // Výchozí stav podle cookie: když je true, menu je rozbalené
        const isOpen = getCookie('sidebar_' + id);
        if (isOpen === 'true') {
            submenu.style.display = 'block';
            icon.classList.remove('fa-chevron-left');
            icon.classList.add('fa-chevron-down');
        } else {
            submenu.style.display = 'none';
            icon.classList.remove('fa-chevron-down');
            icon.classList.add('fa-chevron-left');
        }

        // Přidáme event listener pouze na ikonu
        icon.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (submenu.style.display === 'none') {
                submenu.style.display = 'block';
                setCookie('sidebar_' + id, 'true', 7);
                icon.classList.remove('fa-chevron-left');
                icon.classList.add('fa-chevron-down');
            } else {
                submenu.style.display = 'none';
                setCookie('sidebar_' + id, 'false', 7);
                icon.classList.remove('fa-chevron-down');
                icon.classList.add('fa-chevron-left');
            }
        });
    });

    function setCookie(name, value, days) {
        const d = new Date();
        d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000));
        const expires = "expires=" + d.toUTCString();
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    }

    function getCookie(name) {
        const ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i].trim();
            if (c.indexOf(name + "=") === 0) {
                return c.substring(name.length + 1, c.length);
            }
        }
        return "";
    }
});


/*--------------------------------------------
 * Aktualizuje všechny tabulky
 */
function reloadTables() {
    $('#upcomingTable').DataTable().ajax.reload();
    $('#allTable').DataTable().ajax.reload();
    $('#endTable').DataTable().ajax.reload();
    $('#runningTable').DataTable().ajax.reload();
    $('#thrashTable').DataTable().ajax.reload();
    $('#odTable').DataTable().ajax.reload();
}
    

    $(document).ready(function() 
    {   
        /////////////////////////////////////////////UPCOMING TABLE///////////////////////////////////////////
        /////////////////////////////////////////////END TABLE////////////////////////////////////////////////
        /////////////////////////////////////////////THRASH TABLE/////////////////////////////////////////////
        /////////////////////////////////////////////POHOTOVOSTI TABLE////////////////////////////////////////        
        /////////////////////////////////////////////LOKALITY TABLE///////////////////////////////////////////        
        /////////////////////////////////////////////OD TABLE/////////////////////////////////////////////////
        /////////////////////////////////////////////RUNNING TABLE////////////////////////////////////////////
        /////////////////////////////////////////////OdDetail TABLE///////////////////////////////////////////
        /////////////////////////////////////////////ALL TABLE////////////////////////////////////////////////

        $('#lokalityTable tbody').on('mouseenter', '.table-row', function () {
            $(this).find('.hidden-buttons').css('display', 'flex');
        }).on('mouseleave', '.table-row', function () {
            $(this).find('.hidden-buttons').css('display', 'none');
        });

        $('.dataTables_filter label').contents().filter(function () {
            return this.nodeType === 3; 
        }).remove(); 
        $('.dataTables_filter input').attr('placeholder', 'Hledat...'); 

    });


    



