// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', 'a.page-link', function (e) {
    if (/^\/Product\/Catalog/.test(window.location.pathname)) {
        e.preventDefault(); // Prevent the default link behavior (page reload)

        var url = $(this).attr('href'); // Get the URL from the clicked link

        $("#hideable").load(url);

        //$.ajax({
        //    url: url,
        //    type: 'GET',
        //    headers: {
        //        'X-Requested-With': 'XMLHttpRequest'
        //    },
        //    success: function (response) {
        //        $("#hideable").hide(); // Hide the div on success
        //        $("#part").load(url);
        //    },
        //    error: function (xhr, status, error) {
        //        console.error("AJAX request failed", status, error); // Log errors for debugging
        //        alert('An error occurred while loading the page.');
        //    }
        //});
    }
});