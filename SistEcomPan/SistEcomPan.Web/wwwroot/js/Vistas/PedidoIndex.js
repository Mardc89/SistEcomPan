
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
    let fechaActual = new Date();
    let fechaTexto = document.getElementById("txtFecha");
    let dia = fechaActual.getDate();
    let mes = fechaActual.getMonth() + 1;
    let anio = fechaActual.getFullYear();

    if (dia < 10) {
        dia = '0' + dia;
    }
    if (mes<10) {
        mes = '0' + mes;
    }
    let fechaformateada = dia + '/' + mes + '/' + anio;
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
    MostrarProduct();
  
    $("#modalData").modal("show");
   
}

$("#btnGuardar").click(function () {
    ModalPedidos();
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
                const row =  document.createElement('tr');
                row.innerHTML = `
            <td>${producto.idProducto}</td>
            <td>${producto.descripcion}</td>
            <td>${categoria[i]}</td>
            <td>${producto.stock}</td>
            <td>${producto.precio.toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td>
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm" style="display:inline-block;">Add</button>
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

//document.addEventListener("DOMContentLoaded", function () {

//    ModalPedidos();
//});


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
    else if (cantidad < stock) {

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
        <td><button class="btn btn-primary btn-sm" onclick="eliminarProducto(this)">Eliminar</button></td>
      </tr>
    `;

        document.getElementsByTagName('tbody')[0].insertAdjacentHTML('beforeend', nuevaFila);

        calcularTotal();
    }
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

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('BusquedaPedidos').addEventListener('input', function (event) {
    const TerminoBusqueda = event.target.value;
    actualDePagina = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    MostrarProduct(TerminoBusqueda, actualDePagina);
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
                swal("Lo sentimos", "No se pudo Registrar el Pedido", "error")

            }
        })
})


























