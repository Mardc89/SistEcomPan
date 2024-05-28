

const MODELO_BASE = {
    idPago: "",
    codigoPedido: "",
    montoDePedido:"",
    descuento:"",
    montoTotalDePago:"",
    montoDeuda: "",
    fechaPago: "",
    estado: "",
  

}

let tablaDataMisPagos;

const itemPagina = 4; // Cantidad de productos por página



function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}



$(document).ready(function () {
    
    let busquedaPago = "";
    let busquedaDetallePago = document.getElementById("DniPersonal").textContent;
    tablaDataMisPagos = $('#tbDataMisPagos').DataTable({
        responsive: true,
        "ajax": {
            "url": `/Pago/ObtenerMisPagos?searchTerm=${busquedaDetallePago}&busqueda=${busquedaPago}`,
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "idPago", "searchable": false },
            { "data": "codigoPedido" },
            { "data": "montoDePedido" },
            { "data": "descuento" },
            { "data": "montoTotalDePago" },
            { "data": "montoDeuda" },
            {
                "data": "fechaPago", render: function (data) {
                    return cambiarFecha(data);
                }
            },
            {
                "data": "estado", render: function (data) {
                    if (data == "Pagado")
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">No Activo</span>';


                }

            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": true,
                "width": "80px"

            }
        ],
        order: [[0, "desc"]],
        paging: true,
        pageLength: itemPagina,

    });
})


const ProductosPorPag = 4; // Cantidad de productos por página
let PaginaInicialPago = 1; // Página actual al cargar


function buscarDetallePago(idPago, page = 1) {
    fetch(`/Pago/ObtenerMiDetallePago?idPago=${idPago}&page=${page}&itemsPerPage=${ProductosPorPag}`)
        .then(response => response.json())
        .then(data => {
            const DetallePagos = data.detallePago; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('DetallePagoBuscado');
            productTable.innerHTML = '';

            DetallePagos.forEach(pago => {
                const row = document.createElement('tr');
                row.innerHTML = `  
            <td>${pago.idPago}</td>
            <td>${pago.idDetallePago}</td>
            <td>${pago.montoAPagar}</td>
            <td>${pago.pagoDelCliente}</td>
            <td>${pago.deudaDelCliente}</td>
            <td>${pago.cambioDelCliente}</td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPag);
            const pagination = document.getElementById('DetallePagoPagination');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === PaginaInicialPago) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    PaginaInicialPago = i;
                    buscarPagos(searchTer, PaginaInicialPago);
                    resaltarPaginaActual();
                });

                pagination.appendChild(li);
            }

            resaltarPaginaActual();
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}


// Función para resaltar la página actual
function resaltarPaginaActual() {
    const paginationItems = document.querySelectorAll('#DetallePagoPagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PaginaInicial.toString()) {
            item.classList.add('active');
        }
    });
}



function mostrarModalDetallePago(modelo = MODELO_BASE) {

    let idPagos = modelo.idPago;
    buscarDetallePago(idPagos)

    $("#modalDataMiDetallePago").modal("show")
}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
})




let filaSeleccionada;

$("#tbDataMisPagos tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMisPagos.row(filaSeleccionada).data();
    mostrarModalDetallePago(data);
})


$("#tbdataProducto tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataProducto.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al usuario "${data.nombre}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si,eliminar",
        cancelButtonText: "No,cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {

            if (respuesta) {

                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/Producto/Eliminar?IdProducto=${data.idProducto}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataProducto.row(fila).remove().draw();
                            swal("Listo", "el producto fue eliminado", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})