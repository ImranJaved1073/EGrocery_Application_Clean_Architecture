// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


// Write your JavaScript code.

$(document).ready(function () {
    $('#srchbtn').click(function () {
        var search = $('#srchtxt').val();
        console.log("successful");
        $.get('/Home/Load', { x: search }, function (result) {
            $('#shopitems').html(result).fadeIn('slow');
        });
    });
});


//$(document).ready(function () {
//    $('#srchbtn').click(function (event) {
//        event.preventDefault(); // Prevent default form submission

//        var searchText = $('#srchtxt').val(); // Get the search text

//        // Make AJAX request
//        $.ajax({
//            url: '/Home/Load',
//            type: 'GET',
//            data: { search: searchText },
//            success: function (data) {
//                $('#productList').html(data); // Update the product list
//            },
//            error: function (xhr, status, error) {
//                console.error(xhr.responseText);
//            }
//        });
//    });
//});