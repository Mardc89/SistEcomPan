

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

function ajustarDimension() {
    let celda = document.getElementById("textoTotalDetalle");
    if (window.innerWidth < 768) {
        celda.colSpan = 3;
    }
    else {

        celda.colSpan = 5;
    }
    
}


ajustarDimension();
window.addEventListener("resize", ajustarDimension);
function ObtenerDatosCliente() {
    fetch("/Home/ObtenerCliente")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto
                $("#userDropdown img.img-profile").attr("src",`/ImagenesPerfil/${d.nombreFoto}`);
            }
            else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
}

$(document).ready(function () {

    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone || UTC
    ObtenerDatosCliente();
    let busqueda = "";
    let busquedaDetalle = document.getElementById("DniPersonal").textContent;
    tablaDataMisPedidos = $('#tbDataMisPedidos').DataTable({
        responsive: {
            details: false
        },
        "ajax": {
            "url": `/Pedido/ObtenerMisPedidos?searchTerm=${busquedaDetalle}&busqueda=${busqueda}`,
            "type": "GET",
            "headers":{
                "X-TimeZone":timeZone
            },
            "dataType": "json"
        },
        "columns": [
            { "data": "idPedido", "searchable": false, responsivePriority: 100 },
            { "data": "codigo", responsivePriority: 100 },
            { "data": "montoTotal", responsivePriority: 100 },
            {
                "data": "fechaPedido",responsivePriority: 1 ,render: function (data) {
                    return cambiarFecha(data);
                }
            },
            {
                "data": "estado",responsivePriority: 2,  render: function (data) {
                    if (data == "Pagado")
                        return '<span class="badge badge-info">Pagado</span>';
                    else if (data == "Existe Deuda")
                        return '<span class="badge badge-danger">Existe Deuda</span>';
                    else
                        return '<span class="badge badge-success">Nuevo</span>';


                }

            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": true,
                "width": "80px",
                responsivePriority: 3
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
            <td class="d-none d-md-table-cell">${pedido.idPedido}</td>
            <td class="d-none d-md-table-cell">${pedido.idDetallePedido}</td>
            <td>${pedido.descripcionProducto}</td>
            <td>${parseFloat(pedido.precio).toFixed(2)}</td>
            <td>${pedido.cantidad}</td>
            <td>${pedido.total}</td>
          `;
                productTable.appendChild(row);
            });
            calcularTotal();
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

function calcularTotal() {
    let total = 0;
    const tabla = document.getElementById('tbProductosDetalle').getElementsByTagName('tbody')[0];
    const filas = tabla.getElementsByTagName('tr');


    for (let i = 0; i < filas.length; i++) {
        const fila = filas[i];
        const totalFila = parseFloat(fila.cells[5].textContent);
        total += totalFila;


    }

    document.getElementById('montoTotalDetalle').textContent = total.toFixed(2);
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



function mostrarModalDetallePedido(modelo = MODELO_BASE) {

    let idPedidos = modelo.idPedido;
    buscarDetallePedido(idPedidos)
   
    $("#modalDataMiDetalle").modal("show")
}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
})


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