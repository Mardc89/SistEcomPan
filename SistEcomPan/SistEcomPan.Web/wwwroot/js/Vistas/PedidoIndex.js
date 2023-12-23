

//let ValorImpuesto = 0;
//$(document).ready(function () {

//    fetch("/Pedido/ListaClientes")
//        .then(response => {
//            return response.ok ? response.json() : Promise.reject(response);
//        })
//        .then(responseJson => {
//            if (responseJson.length > 0) {
//                responseJson.forEach((item) => {
//                    $("#cboNombreCliente").append(
//                        $("<option>").val(item.idCliente).text(item.nombreCompleto)
//                    )
//                })
//            }
//        })

//})



//$("#cboNombreCliente").change(function () {
//    var nombreCliente = $("#cboNombreCliente").val();
//    fetch(`/Pedido/ListaNumeroDocumento?nombreCompleto=${nombreCliente}`)
//        .then(response => {
//            return response.ok ? response.json() : Promise.reject(response);
//        })
//        .then(responseJson => {
//            if (responseJson.length > 0) {
//                responseJson.forEach((item) => {
//                    $("#txtDocumentoCliente").val(item.dni)
//                })
//            }
//        })

//})

//$("#txtDocumentoCliente").click(function () {
//    var numeroDocumento = $("#txtDocumentoCliente").val();
//    fetch(`/Pedido/ListaClientes?numeroDocumento=${numeroDocumento}`)
//        .then(response => {
//            return response.ok ? response.json() : Promise.reject(response);
//        })
//        .then(responseJson => {
//            if (responseJson.length > 0) {
//                responseJson.forEach((item) => {
//                    $("#cboNombreCliente").append(
//                        $("<option>").val(item.idCliente).text(item.nombreCompleto)
//                    )
//                })
//            }
//        })

//})



//function mostrarModal() {
//    buscarProductos();
//    $("#modalData").modal("show");
//}

//$("#btnGuardar").click(function () {
//    mostrarModal()
//})

const itemsPerPage = 5; // Cantidad de productos por página
let currentPage = 1; // Página actual al cargar

function buscarProductos(searchTerm = '', page = 1) {
    fetch(`/Pedido/ObtenerProductos?searchTerm=${searchTerm}&page=${page}&itemsPerPage=${itemsPerPage}`)
        .then(response => response.json())
        .then(data => {
            const productos = data.productos; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            const categoria = data.categoria;
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('ProductoBuscado');
            productTable.innerHTML = '';
      
            productos.forEach(producto => {
                const row =  document.createElement('tr');
                row.innerHTML = `
            <td>${producto.idProducto}</td>
            <td>${producto.descripcion}</td>
            <td>${categoria}</td>
            <td>${producto.stock}</td>
            <td>${producto.precio}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td>
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm">Add</button>
            <button onclick="eliminarProducto(this)"class="btn btn-primary btn-sm">De</button>
            </td>
          `;
            productTable.appendChild(row);
         });
         
            // Generar la paginación
            const totalPages = Math.ceil(totalItems / itemsPerPage);
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

                if (i === currentPage) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    currentPage = i;
                    buscarProductos(searchTerm, currentPage);
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
    const precio = parseFloat(row.cells[4].textContent);
    let cantidad = parseFloat(row.cells[5].querySelector('input').value);

   
    let total = 0;
    let cantidadTotal = 0, nuevaCantidad=0;
    const tablaProductos = document.getElementById('tbProductosSeleccionados');
    const filas = tablaProductos.getElementsByTagName('tr');

    for (let i = 1; i < filas.length; i++) {
        const fila = filas[i];
        if (fila.cells[0].textContent === IdProducto) {
            // El producto ya está en laP tabla, incrementar la cantidad
            const cantidadExistente = parseFloat(fila.cells[3].textContent);
            nuevaCantidad = cantidadExistente + cantidad;
            fila.cells[3].textContent = nuevaCantidad;
            return;
        }
        cantidadTotal = nuevaCantidad;
        cantidad = cantidadTotal;
       
    }
    total = precio * cantidad;
    const nuevaFila = `
      <tr>
        <td>${IdProducto}</td>
        <td>${descripcion}</td>
        <td>${categoria}</td>
        <td>${cantidad}</td>
        <td>${precio}</td>
        <td>${total}</td>
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

    document.getElementById('montoTotal').textContent = total;
}






// Función para resaltar la página actual
function resaltarPaginaActual() {
    const paginationItems = document.querySelectorAll('#pagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === currentPage.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('searchInput').addEventListener('input', function (event) {
    const searchTerm = event.target.value;
    currentPage = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    buscarProductos(searchTerm, currentPage);
});

 //Llamada inicial para cargar productos al abrir la tabla modal
$('#modalData').on('show.bs.modal', function () {
    buscarProductos();
});








let ProductosParaVenta = [];

$("#cboBuscarProducto").on("select2:select", function (e) {

    const data = e.params.data;

    let producto_encontrado = ProductosParaVenta.filter(p => p.idProducto == data.id)
    if (producto_encontrado.length > 0) {

        $("#cboBuscarProducto").val("").trigger("change")
        toastr.warning("", "El producto ya fue agregado")
        return false
    }

    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        type: "input",
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: "Ingrese Cantidad"
    },
        function (valor) {

            if (valor === false) return false;

            if (valor === "") {
                toastr.warning("", "Necesita Ingresar la Cantidad")
                return false;
            }

            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe Ingresar un Valor Numerico")
                return false;
            }

            let Producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.text,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor) * data.precio).toString()
            }

            ProductosParaVenta.push(Producto)
            mostrarProductos_Precios();
            $("#cboBuscarProducto").val("").trigger("change")
            swal.close()
        }
    )

})


function mostrarProductos_Precios() {

    let total = 0;
    let igv = 0;
    let subtotal = 0;
    let porcentaje = ValorImpuesto / 100;

    $("#tbProducto tbody").html("")

    ProductosParaVenta.forEach((item) => {

        total = total + parseFloat(item.total)

        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto)
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    subtotal = total / (1 + porcentaje);
    igv = total - subtotal;

    $("#txtSubTotal").val(subtotal.toFixed(2))
    $("#txtIGV").val(igv.toFixed(2))
    $("#txtTotal").val(total.toFixed(2))


}






$(document).on("click", "button.btn-eliminar", function () {

    const _idProducto = $(this).data("idProducto")
    ProductosParaVenta = ProductosParaVenta.filter(p => p.idProducto != _idProducto);
    mostrarProductos_Precios();
})

$("#btnTerminarVenta").click(function () {

    if (ProductosParaVenta.length < 1) {
        toastr.warning("", "Debes Ingresar Productos")
        return;
    }

    const vmDetalleVenta = ProductosParaVenta;

    const venta = {
        idTipoDocumentoVenta: $("#cboTipoDocumentoVenta").val(),
        documentoCliente: $("#txtDocumentoCliente").val(),
        nombreCliente: $("#txtNombreCliente").val(),
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        DetalleVenta: vmDetalleVenta

    }

    console.log(venta);

    $("#btnTerminarVenta").LoadingOverlay("show");

    fetch("/Venta/RegistrarVenta", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(venta)
    })
        .then(response => {
            $("#btnTerminarVenta").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                ProductosParaVenta = [];
                mostrarProductos_Precios();

                $("#txtDocumentoCliente").val("")
                $("#txtNombreCliente").val("")
                $("#cboTipoDocumentoVenta").val($("#cboTipoDocumentoVenta option:first").val())

                swal("Registrado", `Numero Venta:${responseJson.objeto.numeroVenta}`, "success")
            }
            else {
                swal("Lo sentimos", "No se pudo Registrar la venta", "error")

            }
        })
})