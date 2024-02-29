﻿
function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}

function obtenerFecha() {
    var fechaActual = new Date();
    var fechaTexto = document.getElementById("txtFechaPago");
    var dia = fechaActual.getDate();
    var mes = fechaActual.getMonth() + 1;
    var anio = fechaActual.getFullYear();

    if (dia < 10) {
        dia = '0' + dia;
    }
    if (mes < 10) {
        mes = '0' + mes;
    }
    var fechaformateada = dia + '/' + mes + '/' + anio;
    fechaTexto.value = fechaformateada;
}

$("#btnCodPedido").click(function () {                                                                                                                        
    mostrarModal3();
})

function mostrarModal3() { 
    obtenerFecha();
    buscarPedidos();
    $("#modalDataPedidos").modal("show");
}

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
        document.getElementById('txtDescuento').value = 0.00;
        document.getElementById('txtMontoPago').value = montoTotal;





        //const modalPedidos = document.getElementById('modalDataPedidos');
        //modalPedidos.style.display = 'none';


    }

});
    

});


document.getElementById("txtPagoCliente").addEventListener("input", function () {

   
 
    let pago = document.getElementById("txtPagoCliente").value;
    let descuento = document.getElementById("txtDescuento").value;
    let monto = document.getElementById("txtMontoPedido").value;

    if (!isNaN(pago) && !isNaN(descuento) && !isNaN(monto)) {
        if (pago == "") pago = 0;
        Evaluar(pago, descuento, monto);
    }

    else {

        alert("Ingrese numeros validos");
    }



});


function Evaluar(pago, descuento, monto) {
   
    let deuda = 0.00,cambio=0.00;
    let estado = "";

    let montofinal = parseFloat(monto) - parseFloat(descuento);

    if (pago > montofinal) {
        cambio = parseFloat(pago) - parseFloat(montofinal);
        deuda = 0.00;
        estado = "Pagado";
    }

    else if (pago < montofinal) {
        cambio = 0.00;
        deuda = parseFloat(montofinal) - parseFloat(pago);
        estado = "Pendiente";

    }
    else {
        cambio = 0.00;
        deuda = 0.00;
        estado = "Pagado";

    }


    document.getElementById("txtCambio").value = cambio;
    document.getElementById("txtDeuda").value = deuda;
    document.getElementById("txtEstado").value = estado;




   


}






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










