
function mostrarModal2() {
    buscarProductos();
    $("#modalDataDescuento").modal("show");
}

$("#opcion1").click(function () {
    
    let busquedaCodPedido = $("#txtCodigo").val();
    $("#searchInput").val(busquedaCodPedido);
    let monto = $("#txtMontoPedido").val();
    $("#txtImportePedido").val(monto);
    mostrarModal2();
   
})




document.getElementById("txtPagoCliente").addEventListener("click", function () {



    let pago = document.getElementById("txtPagoCliente").value;
    let descuento = document.getElementById("txtDescuento").value;
    let monto = document.getElementById("txtMontoPedido").value;

    if (!isNaN(pago) && !isNaN(descuento) && !isNaN(monto)) {
        if (pago == "") pago = 0, descuento = 0;
        Evaluar(pago, descuento, monto);
    }

    else {

        alert("Ingrese numeros validos");
    }



});





const itemPagina = 5; // Cantidad de productos por página
let  paginaActual= 1; // Página actual al cargar


function buscarProductos(busquedaDetallePedido = '', pagina = 1) {
    const busquedaDetalle = document.getElementById("searchInput").value;
    fetch(`/Pedido/ObtenerDetalleFinal?searchTerm=${busquedaDetalle}&page=${pagina}&itemsPerPage=${itemPagina}`)
        .then(response => response.json())
        .then(data => {
            const detallePedidos = data.pedidos;
            const totalItems = data.totalItems; // Total de productos encontrados
            let i = 1, unidades = 27;
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('ProductoDevuelto');
            productTable.innerHTML = '';

            detallePedidos.forEach(detallePedido => {

                let descripcionfinal = detallePedido.descripcionProducto;

                let descripcionProduct = detallePedido.descripcionProducto.toLowerCase();
                if (/lata de pan|latas de panes|latas de pan/i.test(descripcionProduct)) {
                    const numeroUnidades = descripcionfinal.match(/\d+/);
                    if (numeroUnidades) {
                        unidades = parseInt(numeroUnidades[0]);
                        descripcionfinal = `Latas de Panes de ${unidades} unidades`;
                    }
                    else {
                        descripcionfinal = "Latas de Panes";
                    }

                }
                else if (/pasteles|pastel/i.test(descripcionProduct)) {
                    descripcionfinal = "Unidades de Pasteles";
                }

                const row = document.createElement('tr');
                row.innerHTML = `
                
            
            <td id="categoriaProducto${i}">${detallePedido.categoriaProducto}</td>
            <td id="descripcionProducto${i}">${descripcionfinal}</td>
            <td id="precioProducto${i}">${detallePedido.precio}</td>
            <td id="cantidadProducto${i}">${detallePedido.cantidad}</td>
            <td>${detallePedido.total}</td>
            <td><input type="text" class="form-control form-control-sm" id="cantidadDescuento${i}" placeholder="Ingrese Unidades a Descontar"></td>
          `;
                productTable.appendChild(row);
                i++;
            });

            Devoluciones();   
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


function Devoluciones() {


    const Montofinal = document.getElementById("txtImporteFinal");
    const MontoInicial = document.getElementById("txtImportePedido");
    const Descuentos = document.getElementById("txtDescuentoPedido");
    for (let i = 1; i <= totalItems; i++) {
        const CantidadDescuento = document.getElementById(`cantidadDescuento${i}`);
        CantidadDescuento.addEventListener("input", function () {
            let descuentoTotal = 0, subtotal = 0, unidadDePan = 27;
            for (let j = 1; j <= totalItems; j++) {
                let categorias = document.getElementById(`categoriaProducto${j}`).textContent;
                let precio = document.getElementById(`precioProducto${j}`).textContent;
                let cantidad = document.getElementById(`cantidadProducto${j}`).textContent;
                let unidadDescuento = document.getElementById(`cantidadDescuento${j}`);
                const descuento = parseFloat(unidadDescuento.value) || 0;
                if (categorias == "Panes" && descuento > 0 && descuento <= unidadDePan * cantidad) {
                    let descripcionNueva = document.getElementById(`descripcionProducto${j}`).textContent;
                    let precioUnitario;
                    const UnidadesPan = descripcionNueva.match(/\d+/);
                    if (UnidadesPan) {
                        unidadDePan = parseInt(UnidadesPan[0]);
                        precioUnitario = precio / unidadDePan;
                    }
                    else {
                        precioUnitario = precio / unidadDePan;
                    }
                    subtotal = descuento * precioUnitario.toFixed(4);
                    descuentoTotal += subtotal;
                }
                else if (categorias != "Panes" && descuento > 0 && descuento <= cantidad) {
                    subtotal = descuento * precio;
                    descuentoTotal += subtotal;
                }
                else if (descuento == 0 || descuento > cantidad || descuento > unidadDePan * cantidad) {
                    alert("La Cantidad debe ser menor");
                }
            }
            Descuentos.value = descuentoTotal;
            Montofinal.value = MontoInicial.value - descuentoTotal;
        });
    }





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

























