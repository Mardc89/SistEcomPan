
function mostrarModal3() {
    buscarPedidos();
    $("#modalDataPedidos").modal("show");
}

function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}

$("#btnCodPedido").click(function () {                                                                                                                        
    mostrarModal3();
})

const itemsPerPag = 4; // Cantidad de productos por página
let currentPag = 1; // Página actual al cargar

function buscarPedidos(searchTer = '', page = 1) {
    fetch(`/Pedido/ObtenerPedidos?searchTerm=${searchTer}&page=${page}&itemsPerPage=${itemsPerPag}`)
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
            <td>${cambiarFecha(pedido.fechaPedido)}</td>
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
                    buscarPedidos(searchTer, currentPag);
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



document.addEventListener('DOMContentLoaded', function () { 
    const tablaPedidos = document.getElementById('PedidoBuscado');

    tablaPedidos.addEventListener('click', function (event) {
    const fila = event.target.closest('tr');

    if (fila && fila.parentNode.nodeName == 'TBODY') {

        const codigo = fila.cells[1].textContent;
        const nombres = fila.cells[2].textContent;
        const montoTotal = fila.cells[3].textContent;
        const estado = fila.cells[4].textContent;
        const fecha = fila.cells[5].textContent;

        document.getElementById('txtCodigo').value = codigo;
        document.getElementById('txtNombres').value = nombres;
        document.getElementById('txtMontoPedido').value = montoTotal;
        document.getElementById('txtEstado').value = estado;
        document.getElementById('txtFechaPedido').value = fecha;


        //const modalPedidos = document.getElementById('modalDataPedidos');
        //modalPedidos.style.display = 'none';


    }

});
    

});







// Función para calcular el monto total







// Función para resaltar la página actual
function resaltarPaginaActual() {
    const paginationItems = document.querySelectorAll('#pagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === currentPag.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('searchInputs').addEventListener('input', function (event) {
    const searchTer = event.target.value;
    currentPag = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    buscarPedidos(searchTer, currentPag);
});

//Llamada inicial para cargar productos al abrir la tabla modal
//$('#modalData').on('show.bs.modal', function () {
//    buscarProductos();
//});











