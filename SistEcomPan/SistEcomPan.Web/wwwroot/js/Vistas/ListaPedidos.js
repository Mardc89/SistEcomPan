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

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById('PedidoBuscado').addEventListener('click', function (event) {


        if (event.target.tagName == 'TD') {
            debugger
            const fila = event.target.parentNode;
            const idPedido = fila.cells[0].textContent;
            const codigo = fila.cells[1].textContent;
            const nombres = fila.cells[2].textContent;
            const montoTotal = fila.cells[3].textContent;
            const estado = fila.cells[4].textContent;
            const fecha = fila.cells[5].textContent;

            fetch(`/Pago/ObtenerPagoPedido?searchTerm=${idPedido}`)
                .then(response => response.json())
                .then(data => {
                    const pagos = data.data; // Array de productos obtenidos
                    if (pagos && pagos.length > 0) {
                        const Pagos = pagos[0];
                        /* const totalItems = data.totalItems; */
                        /*  const nombre = data.nombresCompletos;*/
                        // Actualizar la tabla modal con los productos obtenidos
                        document.getElementById('txtDescuento').value = Pagos.descuento;
                        document.getElementById('txtMontoPago').value = Pagos.montoDeuda;
                        document.getElementById('txtDeuda').value = Pagos.montoDeuda;
                        document.getElementById('txtIdPago').value = Pagos.idPago;
                    } else {
                        console.error("No se encontraron Pagos");
                    }
                })
                .catch(error => {
                    console.error('Error al buscar Pagos:', error);
                });
      
            document.getElementById('txtIdPedido').value = idPedido;
            document.getElementById('txtCodigoPedido').value = codigo;
            document.getElementById('txtNombres').value = nombres;
            document.getElementById('txtMontoPedido').value = montoTotal;
            document.getElementById('txtEstado').value = estado;
            document.getElementById('txtFechaPedido').value = fecha;
      /*      document.getElementById('txtMontoPago').value = montoTotal;*/

            $("#modalDataPedidos").modal("hide");
        }



    });
});


const ProductosPorPagina = 4; // Cantidad de productos por página
let PaginaInicial = 1; // Página actual al cargar


function buscarPedidos(searchTer = '', page = 1) {
    fetch(`/Pedido/ObtenerPedidos?searchTerm=${searchTer}&page=${page}&itemsPerPage=${ProductosPorPagina}`)
        .then(response => response.json())
        .then(data => {
            const pedidos = data.pedidos; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
          /*  const nombre = data.nombresCompletos;*/
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('PedidoBuscado');
            productTable.innerHTML = '';

            pedidos.forEach(pedido => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${pedido.idPedido}</td>
            <td>${pedido.codigo}</td>
            <td>${pedido.nombresCompletos}</td>
            <td>${pedido.montoTotal}</td>
            <td>${pedido.estado}</td>
            <td>${cambiarFecha(pedido.fechaPedido)}</td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPagina);
            const pagination = document.getElementById('paginacion');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === PaginaInicial) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    PaginaInicial = i;
                    buscarPedidos(searchTer, PaginaInicial);
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



const ListProductPagina = 4; // Cantidad de productos por página
let ListPaginaInicial = 1; // Página actual al cargar


function buscarListPedidos(searchTer = '', page = 1) {
    fetch(`/Pedido/ObtenerPedidos?searchTerm=${searchTer}&page=${page}&itemsPerPage=${ListProductPagina}`)
        .then(response => response.json())
        .then(data => {
            const pedidos = data.pedidos; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            /*  const nombre = data.nombresCompletos;*/
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('PedidoBuscado');
            productTable.innerHTML = '';

            pedidos.forEach(pedido => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${pedido.idPedido}</td>
            <td>${pedido.codigo}</td>
            <td>${pedido.nombresCompletos}</td>
            <td>${pedido.montoTotal}</td>
            <td>${pedido.estado}</td>
            <td>${cambiarFecha(pedido.fechaPedido)}</td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ListProductPorPagina);
            const pagination = document.getElementById('paginacion');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === ListPaginaInicial) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    PaginaInicial = i;
                    buscarListPedidos(searchTer, ListPaginaInicial);
                    resaltarListPagActual();
                });

                pagination.appendChild(li);
            }

            resaltarListPagActual();
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}
    


document.getElementById("txtPagoCliente").addEventListener("input", function () {

    debugger;
        let idPago = document.getElementById("txtIdPago");
        let idPagos = parseFloat(idPago.value) || 0
        let monto = document.getElementById("txtMontoPedido");
        let descuento = document.getElementById("txtDescuento");
        let descuentofinal = parseFloat(descuento.value) || 0
        if (idPagos > 0) {
            monto = document.getElementById("txtMontoPago");
           
        }
        if (descuento.value=="") {
            document.getElementById("txtDescuento").value="0.00";
        }
        let montofinal = parseFloat(monto.value) || 0
  /*      alert(montofinal);*/

        let pago = document.getElementById("txtPagoCliente");
   

      
        let pagofinal = parseFloat(pago.value) || 0
        if (!isNaN(pagofinal) && !isNaN(descuentofinal) && !isNaN(montofinal) && pago.value>=0) {

       /*     alert("soy desecuento:"+descuentofinal);*/
            Evaluar(pagofinal, descuentofinal, montofinal);
        }

        else {
            document.getElementById("txtPagoCliente").value = "0.00";
            alert("Ingrese numeros validos");
        }

    

});

document.getElementById("txtDescuento").addEventListener("input", function () {
    debugger;
    let idPago = document.getElementById("txtIdPago").value;
    let monto = document.getElementById("txtMontoPedido");
    let deuda = document.getElementById("txtDeuda");
    let descuento = document.getElementById("txtDescuento");
    //if (idPago > 0) {
    //    monto = document.getElementById("txtMontoPago");
    //}
    let pago = document.getElementById("txtPagoCliente");
    let pagofinal = parseFloat(pago.value) || 0
 
    let descuentofinal = parseFloat(descuento.value)||0
    let deudafinal = parseFloat(deuda.value) || 0
    let montofinal = parseFloat(monto.value) || 0
    let montoPedido = montofinal;

    if (pago.value == "") {
        document.getElementById("txtPagoCliente").value = "0.00";
    }

    if (!isNaN(pagofinal) && !isNaN(descuentofinal) && !isNaN(montofinal) && descuento.value>=0) {
        if (descuentofinal > montofinal || descuentofinal > deudafinal && pagofinal > 0 || descuento.value <0 || pago.value <0)
        {
         /*   alert("deuda:" + deudafinal + "montofinal:" + montofinal);*/
            descuentofinal = 0.00;
            document.getElementById("txtDescuento").value ="0.00";
     /*       document.getElementById("txtPagoCliente").value = "0.00";*/
        }
        if (descuentofinal % 1 !== 0 || descuentofinal == 0 || descuentofinal % 1 == 0) {
            montofinal = montofinal - descuentofinal;
            Evaluar(pagofinal, descuentofinal, montofinal);
            montofinal = montoPedido;
        }
    }

    else {
        alert("Ingrese numeros validos");
        document.getElementById("txtDescuento").value = "0.00";
    }



});


function Evaluar(pago, descuento, monto) {
    debugger;
    let pagos = parseFloat(pago);
    let montos = parseFloat(monto);
    let descuentos =descuento; 
    let deuda = 0.00,cambio=0.00;
    let estado = "";

    if (descuentos==0) {
        let montoPedido = document.getElementById("txtMontoPedido").value;
        montos = montoPedido;
        //montofinal = montos - descuentos;
        //alert("monto final:"+montofinal)
    }
    if (pagos > montos) {
        cambio = pagos- montos;
        deuda = 0.00;
        estado = "Pagado";
    }

    else if (pagos <= montos) {
        cambio = 0.00;
        deuda = montos - pagos;
        estado = "Existe Deuda";

    }


    document.getElementById("txtMontoPago").value = monto.toFixed(2);
    document.getElementById("txtCambio").value = cambio.toFixed(2);
    document.getElementById("txtDeuda").value = deuda.toFixed(2);
    document.getElementById("txtEstado").value = estado;
 




   


}

// Función para resaltar la página actual
function resaltarPaginaActual() {
    const paginationItems = document.querySelectorAll('#paginacion .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PaginaInicial.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('searchInputs').addEventListener('input', function (event) {
    const searchTer = event.target.value;
    PaginaInicial = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    buscarPedidos(searchTer, PaginaInicial);
});













