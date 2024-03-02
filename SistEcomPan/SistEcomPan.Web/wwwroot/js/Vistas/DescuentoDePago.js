﻿
function mostrarModal2() {
    buscarProductos();
    $("#modalDataDescuento").modal("show");
}

$("#opcion1").click(function () {
    mostrarModal2()
})

const itemPagina = 5; // Cantidad de productos por página
let  paginaActual= 1; // Página actual al cargar

function buscarProductos(busquedaDetallePedido= '', pagina = 1) {
    fetch(`/Pedido/ObtenerDetallePedido?searchTerm=${busquedaDetallePedido}&page=${pagina}&itemsPerPage=${itemPagina}`)
        .then(response => response.json())
        .then(data => {
            const detallePedidos = data.pedidos;
            const codigo = data.codigos;
            const nombreProducto = data.NombreProducto; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            const nombreCliente = data.nombreCliente;
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('ProductoDevuelto');
            productTable.innerHTML = '';

            detallePedidos.forEach( detallePedido=> {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${codigo}</td>
            <td>${nombreCliente}</td>
            <td>${detallePedido.cantidad}</td>
            <td>${detallePedido.precio}</td>
            <td>${detallePedido.total}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td>
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm">Add</button>
            <button onclick="eliminarProducto(this)"class="btn btn-primary btn-sm">De</button>
            </td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / itemPagina);
            const pagination = document.getElementById('pagination');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === paginaActual) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    paginaActual = i;
                    buscarProductos(busquedaDetallePedido, paginaActual);
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


function agregarProducto(button) {
    const row = button.parentNode.parentNode;
    const IdProducto = row.cells[0].textContent;
    const descripcion = row.cells[1].textContent;
    const categoria = row.cells[2].textContent;
    const stock = parseFloat(row.cells[3].textContent);
    const precio = parseFloat(row.cells[4].textContent).toFixed(2);
    let cantidad = parseFloat(row.cells[5].querySelector('input').value);


    let total = 0;
    let cantidadTotal = 0, nuevaCantidad = 0;
    const tablaProductos = document.getElementById('tbProductosSeleccionados');
    const filas = tablaProductos.getElementsByTagName('tr');

    for (let i = 1; i < filas.length; i++) {
        const fila = filas[i];
        if (fila.cells[0].textContent === IdProducto) {
            // El producto ya está en laP tabla, incrementar la cantidad
            let cantidadExistente = parseFloat(fila.cells[3].textContent);
            nuevaCantidad = cantidadExistente + cantidad;
            fila.cells[3].textContent = nuevaCantidad;
            cantidadTotal = nuevaCantidad;
            cantidad = cantidadTotal;
            total = precio * cantidad;
            fila.cells[5].textContent = total.toFixed(2);
            calcularTotal();
            return;
        }


    }
    total = precio * cantidad;
    const nuevaFila = `
      <tr>
        <td>${IdProducto}</td>
        <td>${descripcion}</td>
        <td>${categoria}</td>
        <td>${cantidad}</td>
        <td>${precio}</td>
        <td>${total.toFixed(2)}</td>
        <td><button onclick="eliminarProducto(this)">Eliminar</button></td>
      </tr>
    `;

    document.getElementsByTagName('tbody')[0].insertAdjacentHTML('beforeend', nuevaFila);

    calcularTotal();
}


function eliminarProducto(button) {
    const row = button.parentNode.parentNode;
    row.parentNode.removeChild(row);

    calcularTotal();
}

// Función para calcular el monto total
function calcularTotal() {
    let total = 0;
    const tabla = document.getElementById('tbProductosSeleccionados').getElementsByTagName('tbody')[0];
    const filas = tabla.getElementsByTagName('tr');


    for (let i = 0; i < filas.length; i++) {
        const fila = filas[i];
        const totalFila = parseFloat(fila.cells[5].textContent);
        total += totalFila;


    }

    document.getElementById('montoTotal').textContent = total.toFixed(2);
}






// Función para resaltar la página actual
function resaltarPaginaActual() {
    const paginationItems = document.querySelectorAll('#pagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === paginaActual.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('searchInput').addEventListener('input', function (event) {
    const busquedaDetallePedido = event.target.value;
    paginaActual = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    buscarProductos(busquedaDetallePedido, paginaActual);
});

//Llamada inicial para cargar productos al abrir la tabla modal
//$('#modalData').on('show.bs.modal', function () {
//    buscarProductos();
//});

$("#btnEnviarPedido").click(function () {
    debugger;
    const tablaProductos = document.getElementById('tbProductosSeleccionados');
    const filas = tablaProductos.getElementsByTagName('tr');

    let productosPedidos = [];

    for (let i = 1; i < filas.length - 1; i++) {
        const fila = filas[i];
        const idProducto = fila.cells[0].textContent;
        const cantidad = fila.cells[3].textContent;
        const total = fila.cells[5].textContent;

        const producto = {
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

    const pedido = {
        dni: $("#txtDocumentoCliente").val(),
        montoTotal: $("#montoTotal").text(),
        estado: $("#txtEstado").val(),
        DetallePedido: vmDetallePedido

    }



    $("#btnEnviarPedido").LoadingOverlay("show");
    debugger;
    fetch("/Pedido/Crear", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(pedido)
    })
        .then(response => {
            $("#btnEnviarPedido").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                productosPedidos = [];
                $("#txtDocumentoCliente").val("")
                swal("Registrado", `Codigo de Producto:${responseJson.objeto.codigo}`, "success")
            }
            else {
                swal("Lo sentimos", "No se pudo Registrar la venta", "error")

            }
        })
})



















