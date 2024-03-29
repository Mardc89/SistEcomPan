﻿
$(document).ready(function () {
    obtenerFecha();
    fetch("/Pedido/ListaNombres")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboNombreCliente").append(
                        $("<option>").val(item.idCliente).text(item.nombreCompleto)
                    )
                })
            }
        })

})


function obtenerFecha() {
    var fechaActual = new Date();
    var fechaTexto = document.getElementById("txtFecha");
    var dia = fechaActual.getDate();
    var mes = fechaActual.getMonth() + 1;
    var anio = fechaActual.getFullYear();

    if (dia < 10) {
        dia = '0' + dia;
    }
    if (mes<10) {
        mes = '0' + mes;
    }
    var fechaformateada = dia + '/' + mes + '/' + anio;
    fechaTexto.value = fechaformateada;
}



$("#cboNombreCliente").change(function () {
    var nombreCliente = $("#cboNombreCliente option:selected").text();
    fetch(`/Pedido/ListaNumeroDocumento?nombreCompleto=${nombreCliente}`)
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#txtDocumentoCliente").val(item.dni)
                })
            }
        })

})

$("#txtDocumentoCliente").click(function () {
    if ($("#txtDocumentoCliente").val().length == 8) {
        var numeroDocumento = $("#txtDocumentoCliente").val();
        fetch(`/Pedido/ListaClientes?numeroDocumento=${numeroDocumento}`)
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.length > 0) {
                    responseJson.forEach((item) => {
                        $("#cboNombreCliente").val(item.idCliente).text(item.nombreCompleto);                       
                    })
                }
            })
    }

})



function ModalPedidos() {
    MostrarProductos();
    $("#modalData").modal("show");
}

$("#btnGuardar").click(function () {
    ModalPedidos();
})

const ElementosDePagina = 5; // Cantidad de productos por página
let actualDePagina = 1; // Página actual al cargar

function MostrarProductos(TerminoBusqueda = '', pagina = 1) {
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
                const row =  document.createElement('tr');
                row.innerHTML = `
            <td>${producto.idProducto}</td>
            <td>${producto.descripcion}</td>
            <td>${categoria[i]}</td>
            <td>${producto.stock}</td>
            <td>${producto.precio.toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td>
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm">Add</button>
            <button onclick="eliminarProducto(this)"class="btn btn-primary btn-sm">De</button>
            </td>
          `;
                i++;
            productTable.appendChild(row);
         });
         
            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ElementosDePagina);
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

                if (i === actualDePagina) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    actualDePagina = i;
                    MostrarProductos(TerminoBusqueda, actualDePagina);
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
    let cantidadTotal = 0, nuevaCantidad=0;
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
        if (link.textContent === actualDePagina.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('BusquedaPedidos').addEventListener('input', function (event) {
    const TerminoBusqueda = event.target.value;
    actualDePagina = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    MostrarProductos(TerminoBusqueda, actualDePagina);
});

 //Llamada inicial para cargar productos al abrir la tabla modal
//$('#modalData').on('show.bs.modal', function () {
//    buscarProductos();
//});
























