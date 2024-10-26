


$(document).ready(function () {

    tablaData = $('#tbdataPedido').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Pedido/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPedido", "searchable": false },
            { "data": "codigo" },
            { "data": "dni" },
            { "data": "nombresCompletos" },
            { "data": "montoTotal" },
            { "data": "estado" },
            {"data":  "fechaPedido"},
/*            { "data": "nombreRol" },*/
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"

            }
        ],

       
    });
})

