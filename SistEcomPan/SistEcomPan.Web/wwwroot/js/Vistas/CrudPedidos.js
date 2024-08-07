﻿



const MODELO_BASE = {
    idPedido: "",
    codigo: "",
    fechaPedido: "",
    estado: "",
    montoTotal: "",
    nombresCompletos: "",


}

let tablaAllPedidos;

const itemPagina = 4; // Cantidad de productos por página


function ModalPedidos() {
    MostrarProduct();
    $("#modalDataPedido").modal("show");

}
$("#btnAgregar").click(function () {             
    ModalPedidos();
})

//$('#modalDataPedido').on('show.bs.modal', function () {
//    var zIndex = 1050 + (10 * $('.modal:visible').length);
//    $(this).css('z-index', zIndex);
//    setTimeout(function () {
//        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
//    }, 0);
//});

//$('#modalDataPedido').on('hidden.bs.modal', function () {
//    if ($('.modal:visible').length) {
//        setTimeout(function () {
//            $(document.body).addClass('modal-open');
//        }, 0);
//    }
//});


//$('#modalDataPedido').on('show.bs.modal', function () {
//    var zIndex = 1050 + (10 * $('.modal:visible').length);
//    $(this).css('z-index', zIndex);
//    setTimeout(function () {
//        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
//    }, 0);
//});

//$('#modalDataDetallePedido').on('hidden.bs.modal', function () {
//    if ($('.modal:visible').length) {
//        setTimeout(function () {
//            $(document.body).addClass('modal-open');
//        }, 0);
//    }
//});

// Ensure backdrop for first modal stays in place
//$('#modalDataDetallePedido').on('hidden.bs.modal', function () {
//    if ($('#modalDataPedido').is(':visible')) {
//        $('body').addClass('modal-open');
//    }
//});

function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}



$(document).ready(function () {
   
    tablaAllPedidos = $('#tbDataAllPedidos').DataTable({
        responsive: true,
        "ajax": {
            "url": `/Pedido/ObtenerListaPedidos`,
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "idPedido", "searchable": false },
            { "data": "codigo" },
            {"data":"nombresCompletos"},
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


const ElementosDePagina = 3; // Cantidad de productos por página
let actualDePagina = 1; // Página actual al cargar

function MostrarProduct(TerminoBusqueda = '', pagina = 1) {
    fetch(`/Pedido/ObtenerProductos?searchTerm=${TerminoBusqueda}&page=${pagina}&itemsPerPage=${ElementosDePagina}`)
        .then(response => response.json())
        .then(data => {
            const productos = data.productos; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            const categoria = data.categoria;
            let i = 0;
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('ProductoBuscado');
            productTable.innerHTML = '';

            productos.forEach(producto => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${producto.idProducto}</td>
            <td>${producto.descripcion}</td>
            <td>${categoria[i]}</td>
            <td>${producto.stock}</td>
            <td>${producto.precio.toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td class="d-flex">
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm mr-2" style="display:inline-block;">Add</button>
            <button onclick="eliminarProducto(this)"class="btn btn-primary btn-sm" style="display:inline-block;">De</button>
            </td>
          `;
                i++;
                productTable.appendChild(row);


            });
            SeleccionProductos();
            // Generar la paginación
            const totalPag = Math.ceil(totalItems / ElementosDePagina);
            const PaginaPag = document.getElementById('Paginations');
            PaginaPag.innerHTML = '';

            for (let i = 1; i <= totalPag; i++) {
                const liA = document.createElement('li');
                liA.classList.add('page-item');
                const linkA = document.createElement('a');
                linkA.classList.add('page-link');
                linkA.href = '#';
                linkA.textContent = i;
                liA.appendChild(linkA);

                if (i === actualDePagina) {
                    liA.classList.add('active');
                }

                linkA.addEventListener('click', () => {
                    actualDePagina = i;
                    MostrarProduct(TerminoBusqueda, actualDePagina);
                    resaltarPagActual();

                });

                PaginaPag.appendChild(liA);
            }

            resaltarPagActual();

        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}


function resaltarPagActual() {
    const paginationItems = document.querySelectorAll('#Paginations .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === actualDePagina.toString()) {
            item.classList.add('active');
        }
    });
}


document.getElementById('BusquedaPedidos').addEventListener('input', function (event) {
    const TerminoBusqueda = event.target.value;
    actualDePagina = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    MostrarProduct(TerminoBusqueda, actualDePagina);
});

function SeleccionProductos() {
    const filas = document.querySelectorAll("#ProductoBuscado tbody tr");
    filas.forEach(fila => {
        fila.addEventListener("click", function () {
            alert("filas filas");

            const stock = parseFloat(fila.cells[3].textContent);
            const cantidadIngresada = parseFloat(fila.cells[5].querySelector('input').value);

            if (isNaN(cantidadIngresada) || cantidadIngresada <= 0) {
                alert("Ingrese una cantidad valida");
            }
            else if (cantidadIngresada > stock) {
                alert("La cantidad supera al stock");
            }

        });
    });
}


const ProductosPorPagina = 4; // Cantidad de productos por página
let PaginaInicial = 1; // Página actual al cargar


function BuscarDetallePedido(idPedido) {
    fetch(`/Pedido/ObtenerAllDetallePedido?idPedido=${idPedido}`)
        .then(response => response.json())
        .then(data => {
            const DetallePedidos = data.detallePedido; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('DetallePedidoBuscado');
            productTable.innerHTML = '';

            DetallePedidos.forEach(pedido => {
                let precio = parseFloat(pedido.precio);
                if (isNaN(precio)) {
                    precio = 0;
                }
                const row = document.createElement('tr');
                row.innerHTML = `  
            <td>${pedido.idPedido}</td>
            <td>${pedido.descripcionProducto}</td>
            <td>${precio.toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad" value=${pedido.cantidad} disabled></td>
            <td>${pedido.total}</td>
            <td class="d-flex">
            <button onclick="EliminarProducto(this)"class="btn btn-primary btn-sm">Eliminar</button>
            </td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPagina);
            const pagination = document.getElementById('DetallePedidoPagination');
            pagination.innerHTML = '';

            //for (let i = 1; i <= totalPages; i++) {
            //    const li = document.createElement('li');
            //    li.classList.add('page-item');
            //    const link = document.createElement('a');
            //    link.classList.add('page-link');
            //    link.href = '#';
            //    link.textContent = i;
            //    li.appendChild(link);

            //    if (i === PaginaInicial) {
            //        li.classList.add('active');
            //    /*    calcularTotal(PaginaInicial);*/
            //    }

            //    link.addEventListener('click', () => {
            //        PaginaInicial = i;
            //        BuscarDetallePedido(idPedido, PaginaInicial);
                  
            //        resaltarPaginaDetalleActual();
            //    });

            //    pagination.appendChild(li);
            //}

          /*  resaltarPaginaDetalleActual();*/
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}


// Función para resaltar la página actual
function resaltarPaginaDetalleActual() {
    const paginationItems = document.querySelectorAll('#DetallePedidoPagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PaginaInicial.toString()) {
            item.classList.add('active');
        }
    });
}

function EliminarProducto(button) {
    const row = button.parentNode.parentNode;
    row.parentNode.removeChild(row);

    calcularTotal();
}

function agregarProducto(button) {
    const row = button.parentNode.parentNode;
    const IdProducto = row.cells[0].textContent;
    const descripcion = row.cells[1].textContent;
    const categoria = row.cells[2].textContent;
    const stock = parseFloat(row.cells[3].textContent);
    const precio = parseFloat(row.cells[4].textContent).toFixed(2);
    const cantidadIngresada = row.cells[5].querySelector('input');

    let cantidad = parseFloat(cantidadIngresada.value);

    if (isNaN(cantidad) || cantidad <= 0) {
        alert("Ingrese una cantidad valida");
    }
    else if (cantidad > stock) {
        alert("La cantidad supera al stock");
    }
    else if (cantidad <= stock) {

        let total = 0;
        let cantidadTotal = 0, nuevaCantidad = 0;
        const tablaProductos = document.getElementById('DetallePedidoBuscado');
        const filas = tablaProductos.getElementsByTagName('tr');

        for (let i = 0; i < filas.length; i++) {
            const fila = filas[i];
    /*        alert("soy:"+cantidadEscogida)*/
            if (fila.cells[1].textContent === descripcion) {
                alert("son iguales");
                let cantidadInicial = fila.cells[3].querySelector('input');
                let cantidadExistente = parseFloat(cantidadInicial.value);
                // El producto ya está en laP tabla, incrementar la cantidad
             /*   let cantidadExistente = parseFloat(fila.cells[3].querySelector('input'));*/
                alert(cantidadExistente);
                alert(cantidad);
                nuevaCantidad = cantidadExistente + cantidad;
                if (nuevaCantidad <= stock) {
                    fila.cells[3].textContent = nuevaCantidad;
                    alert(nuevaCantidad);
                    cantidadTotal = nuevaCantidad;
                    cantidad = cantidadTotal;
                    total = precio * cantidad;
                    fila.cells[4].textContent = total.toFixed(2);
                    cantidadInicial.value = total.toFixed(2);
                    cantidadInicial.disabled = true;
                    alert(fila.cells[4].textContent);
                    calcularTotal();
                    return;
                }
                return;
            }


        }
        total = precio * cantidad;
        const nuevaFila = `
      <tr>
        <td>${IdProducto}</td>
        <td>${descripcion}</td>        
        <td>${precio}</td>
        <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad" value=${cantidad}></td>      
        <td>${total.toFixed(2)}</td>
        <td><button class="btn btn-primary btn-sm" onclick="EliminarProducto(this)">Eliminar</button></td>
      </tr>
    `;

        document.getElementById('DetallePedidoBuscado').insertAdjacentHTML('beforeend', nuevaFila);

        calcularTotal();
    }
}




function calcularTotal() {
    let total = 0;
    const tabla = document.getElementById('tbDetallePedidos').getElementsByTagName('tbody')[0];
    const filas = tabla.getElementsByTagName('tr');
/*    let suma = 0;*/
    for (let i = 0; i < filas.length; i++) {
            const fila = filas[i];
            const totalFila = parseFloat(fila.cells[4].textContent);
            total += totalFila;

    }
    document.getElementById('txtFinalDetallePedido').value = total.toFixed(2);
    alert("el total es:" + total);
/*    suma = suma + total;*/
   
}

//function calcularTotal(paginas) {

//    const tabla = document.getElementById('tbDetallePedidos').getElementsByTagName('tbody')[0];
//    const filas = tabla.getElementsByTagName('tr');
//    for (let j = paginas; j <= paginas; j++) {
//        for (let i = 0; i < filas.length; i++) {
//            const fila = filas[i];
//            const totalFila = parseFloat(fila.cells[5].textContent);
//            total += totalFila;

//        }
//        suma = suma + total;
//    }
//    document.getElementById('montoTotalPedido').textContent = suma.toFixed(2);
    
//}

// Evento cuando el usuario escribe en la barra de búsqueda
//document.getElementById('searchInputs').addEventListener('input', function (event) {
//    const searchTer = event.target.value;
//    PaginaInicial = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
//    buscarDetallePedido(searchTer, PaginaInicial);
//});

function mostrarModalDetallePedido(modelo = MODELO_BASE) {

    let idPedidos = modelo.idPedido;
/*    suma = 0, total = 0;*/
    BuscarDetallePedido(idPedidos);
    $("#txtInicialDetallePedido").val(modelo.montoTotal);
    $("#modalDataDetallePedido").modal("show")
    document.getElementById('txtFinalDetallePedido').value ="";
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

$("#tbDataAllPedidos tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaAllPedidos.row(filaSeleccionada).data();
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
