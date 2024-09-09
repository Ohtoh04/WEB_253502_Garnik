// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.page-link').forEach(function(link) {
        link.addEventListener('click', function(event) {
            event.preventDefault();
            var page = this.getAttribute('data-page');
            loadPage(page);
        });
    });
});

function loadPage(page) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `/Product/Catalog?pageNo=` + page, true);  // URL может быть изменен в зависимости от маршрутов
    xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
        
    xhr.onload = function() {
        if (xhr.status >= 200 && xhr.status < 400) {
            document.querySelector('#product-list').innerHTML = xhr.responseText;
        }
    };

    xhr.send();
}
