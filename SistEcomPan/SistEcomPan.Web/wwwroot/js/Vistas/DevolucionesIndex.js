

const MODELO_BASE = {
    idDevolucion: "",
    codigoDevolucion: "",
    codigoPedido: "",
    nombresCompletos: "",
    montoPedido: "",
    descuento: "",
    montoAPagar: "",
    fechaDevolucion: "",


}




let tablaDataDevoluciones;

const itemPagina = 4; // Cantidad de productos por página



function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}

function ObtenerDatosUsuario() {
    fetch("/Home/ObtenerUsuario")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto
                $("#userDropdown img.img-profile").attr("src", `/ImagenesPerfil/${d.nombreFoto}`);

            }
            else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
}

$(document).ready(function () {


    ObtenerDatosUsuario();

    tablaDataDevoluciones = $('#tbDataDevoluciones').DataTable({
        responsive: {
            details: false
        },
        "ajax": {
            "url": `/Devolucion/ListaDevoluciones`,
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "idDevolucion", "searchable": false },
            { "data": "nombresCompletos" },
            { "data": "codigoPedido" },
            { "data": "codigoDevolucion" },
            { "data": "montoPedido" },
            { "data": "descuento" },
            { "data": "montoAPagar" },
            {
                "data": "fechaDevolucion", render: function (data) {
                    return cambiarFecha(data);
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


function buscarDetalleDevolucion(idDevolucion, page = 1) {
    fetch(`/Devolucion/ObtenerDetalleDevolucion?idDevolucion=${idDevolucion}&page=${page}&itemsPerPage=${ProductosPorPagina}`)
        .then(response => response.json())
        .then(data => {
            const DetalleDevolucion = data.detalleDevolucion; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('DetalleDevolucion');
            productTable.innerHTML = '';

            DetalleDevolucion.forEach(devolucion => {
                const row = document.createElement('tr');
                row.innerHTML = `  
            <td>${devolucion.idDetalleDevolucion}</td>
            <td>${devolucion.categoria}</td>
            <td>${devolucion.descripcion}</td>
            <td>${devolucion.precio}</td>
            <td>${devolucion.cantidadPedido}</td>            
            <td>${devolucion.total}</td>
            <td>${devolucion.cantidadDevolucion}</td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPagina);
            const pagination = document.getElementById('DetalleDevolucionPagination');
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
                    buscarDetalleDevolucion(idPedido, PaginaInicial);
                    resaltarPaginaDevolucionActual();
                });

                pagination.appendChild(li);
            }

            resaltarPaginaDevolucionActual();
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}


// Función para resaltar la página actual
function resaltarPaginaDevolucionActual() {
    const paginationItems = document.querySelectorAll('#DetalleDevolucionPagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PaginaInicial.toString()) {
            item.classList.add('active');
        }
    });
}



function mostrarModalDetalleDevolucion(modelo = MODELO_BASE) {

    let idDevolucion = modelo.idDevolucion;
    buscarDetalleDevolucion(idDevolucion)

    $("#modalDataDetalleDevolucion").modal("show")
}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
})




let filaSeleccionada;

$("#tbDataDevoluciones tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataDevoluciones.row(filaSeleccionada).data();
    mostrarModalDetalleDevolucion(data);
})


$("#tbDataDevoluciones tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataDevoluciones.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al usuario "${data.codigoDevolucion}"`,
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
                fetch(`/Devolucion/Eliminar?IdDevolucion=${data.idDevolucion}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataDevoluciones.row(fila).remove().draw();
                            swal("Listo", "La Devolucion fue eliminada", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})