// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('.close-alert').click(function () {
    $('.alert').hide('hide');
})

$(function () {
    $('.open-delete-modal').on('click', function () {
        var url = $(this).data('url');

        $('#modal-placeholder').load(url, function () {

            var modalElement = $('#modal-placeholder').find('.modal');
            var modalInstance = new bootstrap.Modal(modalElement[0]);
            modalInstance.show();

            // Opcional: limpa o placeholder quando o modal é fechado
            modalElement.on('hidden.bs.modal', function () {
                $('#modal-placeholder').empty();
            });
        });
    });
});