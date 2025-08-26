$(document).ready(function () {
    if ($('#table-livros').length) {
        getDatatable('#table-livros');
    }
    if ($('#table-usuarios').length) {
        getDatatable('#table-usuarios');
    }
});

function getDatatable(id) {
    $(id).DataTable({
        ordering: true,
        paging: true,
        searching: true,
        responsive: true,
        language: {
            emptyTable: "Nenhum registro encontrado na tabela",
            info: "Mostrando _START_ até _END_ de _TOTAL_ registros",
            infoEmpty: "Mostrando 0 até 0 de 0 registros",
            infoFiltered: "(filtrado de _MAX_ registros no total)",
            lengthMenu: "Mostrar _MENU_ registros por página",
            loadingRecords: "Carregando...",
            processing: "Processando...",
            search: "Pesquisar:",
            zeroRecords: "Nenhum registro encontrado",
            paginate: {
                first: "<<",
                last: ">>",
                next: ">",
                previous: "<"
            },
            aria: {
                sortAscending: ": ativar para classificar a coluna em ordem crescente",
                sortDescending: ": ativar para classificar a coluna em ordem decrescente"
            }
        }
    });
}

    $('.close-alert').click(function () {
        $('.alert').hide('hide');
    });

    $(document).on('click', '.open-delete-modal', function () {
        var url = $(this).data('url');

        $('#modal-placeholder').load(url, function () {
            var modalElement = $('#modal-placeholder').find('.modal');
            var modalInstance = new bootstrap.Modal(modalElement[0]);
            modalInstance.show();

            modalElement.on('hidden.bs.modal', function () {
                $('#modal-placeholder').empty();
            });
        });
    });
