

const MODELO_BASE = {
    idPedido: "",
    codigo: "",
    fechaPedido: "",
    estado: "",
    montoTotal:"",
    nombresCompletos:"",


}

document.getElementById("btnNuevoPedido").addEventListener("click", function () {
    window.location.href = 'NuevoPedido';
});


let tablaDataMisPedidos;

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
    ObtenerDatosCliente();
    let busqueda = "";
    let busquedaDetalle = document.getElementById("DniPersonal").textContent;
    tablaDataMisPedidos = $('#tbDataMisPedidos').DataTable({
        responsive: true,
        "ajax": {
            "url": `/Pedido/ObtenerMisPedidos?searchTerm=${busquedaDetalle}&busqueda=${busqueda}`,
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "idPedido", "searchable": false },
            { "data": "codigo" },
            { "data": "montoTotal" },
            {
                "data": "fechaPedido", render: function (data) {
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
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },
    });
})


const ProductosPorPagina = 4; // Cantidad de productos por página
let PaginaInicial = 1; // Página actual al cargar


function buscarDetallePedido(idPedido, page = 1) {
    fetch(`/Pedido/ObtenerMiDetallePedido?idPedido=${idPedido}&page=${page}&itemsPerPage=${ProductosPorPagina}`)
        .then(response => response.json())
        .then(data => {
            const DetallePedidos = data.detallePedido; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('DetalleBuscado');
            productTable.innerHTML = '';

            DetallePedidos.forEach(pedido => {
                const row = document.createElement('tr');
                row.innerHTML = `  
            <td>${pedido.idPedido}</td>
            <td>${pedido.idDetallePedido}</td>
            <td>${pedido.descripcionProducto}</td>
            <td>${pedido.precio}</td>
            <td>${pedido.cantidad}</td>
            <td>${pedido.total}</td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPagina);
            const pagination = document.getElementById('DetallePagination');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === PaginaInicial) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    PaginaInicial = i;
                    buscarDetallePedido(idPedido, PaginaInicial);
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
    const paginationItems = document.querySelectorAll('#DetallePagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PaginaInicial.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
//document.getElementById('searchInputs').addEventListener('input', function (event) {
//    const searchTer = event.target.value;
//    PaginaInicial = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
//    buscarDetallePedido(searchTer, PaginaInicial);
//});

function mostrarModalDetallePedido(modelo = MODELO_BASE) {

    let idPedidos = modelo.idPedido;
    buscarDetallePedido(idPedidos)
   
    $("#modalDataMiDetalle").modal("show")
}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
})


//$("#btnGuardarProducto").click(function () {



//    const modelo = structuredClone(MODELO_BASE);
//    modelo["idProducto"] = parseInt($("#txtIdProducto").val())
//    modelo["descripcion"] = $("#txtDescripcion").val()
//    modelo["precio"] = $("#txtPrecio").val()
//    modelo["stock"] = $("#txtStock").val()
//    modelo["idCategoria"] = $("#cboCategoria").val()
//    modelo["estado"] = $("#cboEstado").val()

//    const inputFoto = document.getElementById("txtImagen")

//    const formData = new FormData();

//    formData.append("foto", inputFoto.files[0])
//    formData.append("modelo", JSON.stringify(modelo))

//    $("#modalDataProducto").find("div.modal-content").LoadingOverlay("show");

//    if (modelo.idUsuario == 0) {

//        fetch("/Producto/Crear", {
//            method: "POST",
//            body: formData
//        })
//            .then(response => {
//                $("#modalDataProducto").find("div.modal-content").LoadingOverlay("hide");
//                return response.ok ? response.json() : Promise.reject(response);
//            })
//            .then(responseJson => {
//                if (responseJson.estado) {
//                    tablaDataProducto.row.add(responseJson.objeto).draw(false)
//                    $("#modalDataProducto").modal("hide")
//                    swal("Listo", "el producto fue creado", "success")
//                }
//                else {
//                    swal("Lo sentimos", responseJson.mensaje, "error")
//                }
//            })
//    }
//    else {

//        fetch("/Producto/Editar", {
//            method: "PUT",
//            body: formData
//        })
//            .then(response => {
//                $("#modalDataProducto").find("div.modal-content").LoadingOverlay("hide");
//                return response.ok ? response.json() : Promise.reject(response);
//            })
//            .then(responseJson => {

//                if (responseJson.estado) {

//                    tablaDataProducto.row(filaSeleccionada).data(responseJson.objeto).draw(false);
//                    filaSeleccionada = null;
//                    $("#modalDataProducto").modal("hide")
//                    swal("Listo", "el producto fue modificado", "success")
//                } else {
//                    swal("Lo sentimos", responseJson.mensaje, "error")
//                }
//            })





//    }
//})

let filaSeleccionada;

$("#tbDataMisPedidos tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMisPedidos.row(filaSeleccionada).data();
    mostrarModalDetallePedido(data);
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