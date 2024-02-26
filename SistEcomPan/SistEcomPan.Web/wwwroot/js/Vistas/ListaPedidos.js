
function mostrarModal3() {
    buscarPedidos();
    $("#modalDataPedidos").modal("show");
}

$("#btnCodPedido").click(function () {                                                                                                                        
    mostrarModal3();
})

const itemsPerPag = 4; // Cantidad de productos por página
let currentPag = 1; // Página actual al cargar

function buscarPedidos(searchTerm = '', page = 1) {
    fetch(`/Pedido/ObtenerPedidos?searchTerm=${searchTerm}&page=${page}&itemsPerPage=${itemsPerPag}`)
        .then(response => response.json())
        .then(data => {
            const pedidos = data.pedidos; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            const nombre = data.nombreCliente;
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('PedidoBuscado');
            productTable.innerHTML = '';

            pedidos.forEach(pedido => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${pedido.idPedido}</td>
            <td>${pedido.codigo}</td>
            <td>${nombre}</td>
            <td>${pedido.montoTotal}</td>
            <td>${pedido.estado}</td>
            <td>${pedido.fechaPedido}</td>
            <td>
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm">Add</button>
            <button onclick="eliminarProducto(this)"class="btn btn-primary btn-sm">De</button>
            </td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / itemsPerPag);
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

                if (i === currentPag) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    currentPag = i;
                    buscarPedidos(searchTerm, currentPag);
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







// Función para calcular el monto total







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
    buscarPedidos(searchTerm, currentPage);
});

//Llamada inicial para cargar productos al abrir la tabla modal
//$('#modalData').on('show.bs.modal', function () {
//    buscarProductos();
//});











