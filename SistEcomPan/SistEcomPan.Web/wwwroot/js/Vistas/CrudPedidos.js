



const MODELO_BASE = {
    idPedido: "",
    codigo: "",
    fechaPedido: "",
    estado: "",
    montoTotal: "",
    nombresCompletos: "",


}

document.getElementById("btnNuevoPedido").addEventListener("click", function () {
    window.location.href = 'NuevoPedido';
});



let tablaAllPedidos;

const itemPagina = 4; // Cantidad de productos por página


function ModalPedidos() {
    MostrarProduct();
    $("#modalDataPedido").modal("show");

}
$("#btnAgregar").click(function () {             
    ModalPedidos();
})




function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}


let NombresCompletos,Codigo,Estado;
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
            {
                "data": "codigo", render: function (data) {
                    Codigo = data;
                    return Codigo;
                }
            },
            {
                "data": "nombresCompletos", render: function (data) {
                    NombresCompletos = data;
                    return NombresCompletos;
                }
            },
            { "data": "montoTotal" },
            {
                "data": "fechaPedido", render: function (data) {
                    return cambiarFecha(data);
                }
            },
            {
                "data": "estado", render: function (data) {
                    if (data == "Pagado") {
                        Estado = data;
                        return '<span class="badge badge-info">Pagado</span>';
                    }
                    else if (data == "Existe Deuda") {
                        Estado = data;
                        return '<span class="badge badge-danger">Existe Deuda</span>';
                    }
                    else if (data == "Nuevo"){
                        Estado = data;
                        return '<span class="badge badge-success">Nuevo</span>';
                    }

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
            <td>${(producto.precio).toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td class="d-flex">
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm mr-2" style="display:inline-block;">Add</button>

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
            <td>${pedido.idProducto}</td>
            <td>${pedido.descripcionProducto}</td>
            <td>${precio.toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad" value=${pedido.cantidad} disabled></td>
            <td>${pedido.total}</td>
            <td class="d-flex">
            <button onclick="EliminarProducto(this)"class="btn btn-primary btn-sm btnEliminarProducto">Eliminar</button>
            </td>
          `;
                productTable.appendChild(row);
            });
            VerificarEstado(Estado);
            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPagina);
            const pagination = document.getElementById('DetallePedidoPagination');
            pagination.innerHTML = '';


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
    let IdProducto = row.cells[0].textContent;
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
        //const filaID = filas[0];
        //IdProducto = filaID.cells[0].textContent;
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
                    cantidadInicial.value = nuevaCantidad;
                    alert(nuevaCantidad);
                    cantidadTotal = nuevaCantidad;
                    total = precio * cantidadTotal;
                    fila.cells[4].textContent = total.toFixed(2);
                    cantidadInicial.value = cantidadTotal;
                  /*  cantidadInicial.disabled = true;*/
                    alert(fila.cells[4].textContent);
                    calcularTotal();
                    return;
                }
                else {
                    alert("cantidad superar al stock:"+ nuevaCantidad);
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



let totalPedido = 0;
function calcularTotal() {
    totalPedido = 0;
    const tabla = document.getElementById('tbDetallePedidos').getElementsByTagName('tbody')[0];
    const filas = tabla.getElementsByTagName('tr');
/*    let suma = 0;*/
    for (let i = 0; i < filas.length; i++) {
            const fila = filas[i];
            const totalFila = parseFloat(fila.cells[4].textContent);
            totalPedido += totalFila;

    }
    document.getElementById('txtFinalDetallePedido').value = totalPedido.toFixed(2);
    alert("el total es:" + totalPedido);
/*    suma = suma + total;*/
   
}



let idPedidos = 0;

function mostrarModalDetallePedido(modelo = MODELO_BASE) {

    idPedidos = modelo.idPedido;
    let montoTotal = modelo.montoTotal;
/*    suma = 0, total = 0;*/
    BuscarDetallePedido(idPedidos);
    $("#txtInicialDetallePedido").val(modelo.montoTotal);
    $("#txtInicialDetallePedido").prop('disabled',true);
    $("#txtFinalDetallePedido").prop('disabled',true);
    $("#modalDataDetallePedido").modal("show");

    document.getElementById('txtFinalDetallePedido').value = "";

}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
})




let filaSeleccionada;

$("#tbDataAllPedidos tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaAllPedidos.row(filaSeleccionada).data();
    NombresCompletos = data.nombresCompletos;
    Codigo = data.codigo;
    Estado = data.estado;

    mostrarModalDetallePedido(data);
})


function VerificarEstado(estado) {

    const botonAgregar = document.getElementById("btnAgregar");
    const botonActualizar = document.getElementById("btnActualizarPedido");
    const botonEliminar = document.getElementsByClassName("btnEliminarProducto");
    if (estado === "Nuevo" ) {
        botonAgregar.disabled = false;
        botonActualizar.disabled = false;

        for (let i = 0; i < botonEliminar.length; i++) {
            botonEliminar[i].disabled = false;
        }
    }
    else {
        botonAgregar.disabled = true;
        botonActualizar.disabled = true;

        for (let i = 0; i < botonEliminar.length; i++) {
            botonEliminar[i].disabled = true;
        }
    }

}


$("#tbDataAllPedidos tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaAllPedidos.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al usuario "${data.idPedido}"`,
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
                fetch(`/Pedido/Eliminar?IdPedido=${data.idPedido}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaAllPedidos.row(fila).remove().draw();
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



$("#btnActualizarPedido").click(function () {
    debugger;
    const tablaProductos = document.getElementById('tbDetallePedidos');
    const filas = tablaProductos.getElementsByTagName('tr');

    let productosPedidos = [];

    for (let i = 1; i < filas.length; i++) {
        const fila = filas[i];
        const idPedido = idPedidos;
        const idProducto = fila.cells[0].textContent;
        let cantidadInicial = fila.cells[3].querySelector('input');
        const cantidad = cantidadInicial.value;
        const total = fila.cells[4].textContent;

        const producto = {
            idPedido: idPedido,
            idProducto: idProducto,
            cantidad: cantidad,
            total: total
        };
        productosPedidos.push(producto);
    }


    if (productosPedidos.length < 1) {
        toastr.warning("", "Debes Ingresar Productos")
        return;
    }

    const vmDetallePedido = productosPedidos;

    const pedidoActualizado = {
        idPedido: idPedidos,
        codigo: Codigo,
        estado: Estado,
        nombresCompletos: NombresCompletos,
        montoTotal: $("#txtFinalDetallePedido").val(),
        DetallePedido: vmDetallePedido

    }
 /*   alert("los pedidos:" + pedido[0].montoTotal);*/



    $("#btnActualizarPedido").LoadingOverlay("show");
    
    fetch("/Pedido/ActualizarPedido", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(pedidoActualizado)
    })
        .then(response => {
            $("#btnActualizarPedido").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                productosPedidos = [];
                /*      $("#txtDocumentoCliente").val("")*/
                tablaAllPedidos.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                filaSeleccionada = null;
                $("#modalDataDetallePedido").modal("hide")
                swal("Pedido Actualizado", `Nuevo Monto:${responseJson.objeto.montoTotal}`, "success")
            }
            else {
                swal("Lo sentimos", "No se pudo Actualizar el Pedido", "error")

            }
        })
})
