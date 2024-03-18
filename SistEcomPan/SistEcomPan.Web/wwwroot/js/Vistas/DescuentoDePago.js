
function mostrarModal2() {
    buscarProductos();
    $("#modalDataDescuento").modal("show");
}

$("#opcion1").click(function () {
    
    let busquedaCodPedido = $("#txtCodigo").val();
    $("#searchInput").val(busquedaCodPedido);
    mostrarModal2();
   
})

const itemPagina = 5; // Cantidad de productos por página
let  paginaActual= 1; // Página actual al cargar


function buscarProductos(busquedaDetallePedido = '', pagina = 1) {
    const busquedaDetalle = document.getElementById("searchInput").value;
    fetch(`/Pedido/ObtenerDetalleFinal?searchTerm=${busquedaDetalle}&page=${pagina}&itemsPerPage=${itemPagina}`)
        .then(response => response.json())
        .then(data => {
            const detallePedidos = data.pedidos;
            const codigo = data.codigos;
            const nombreProducto = data.nombresProducto; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            const precioProducto = data.precios;
            let i = 0;
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('ProductoDevuelto');
            productTable.innerHTML = '';

            detallePedidos.forEach( detallePedido=> {
                const row = document.createElement('tr');
                row.innerHTML = `
 
            
            <td>${detallePedido.categoriaProducto}</td>
            <td>${detallePedido.descripcionProducto}</td>
            <td>${detallePedido.precio}</td>
            <td>${detallePedido.cantidad}</td>
            <td>${detallePedido.total}</td>
            <td><input type="text" class="form-control form-control-sm" id="txtCantidad" placeholder="Ingrese Unidades a Descontar"></td>
            <td>
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm">Descontar</button>
            </td>
          `;
                productTable.appendChild(row);
                i++;
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

























