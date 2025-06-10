
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

function ActivarCampos() {
    $("#txtDescuento").prop("disabled", false)
    $("#txtPagoCliente").prop("disabled",false)
}

function VerificarEstado() {
    debugger;
    let idPago = document.getElementById("txtIdPago").value;
    let estado = document.getElementById("txtEstado").value;
    let MontoTotal = document.getElementById("txtMontoPago").value;
    let btnEstado = document.getElementById("btnGuardarPago");
    let Opcion1 = document.getElementById("opcion1");
    let Opcion2 = document.getElementById("opcion2");
    if (estado == "Pagado" && MontoTotal=="0.00") {
        btnEstado.disabled = true;
        Opcion1.disabled = true;
        Opcion2.disabled = true;
    }
    //else if (estado == "Sin Efectuar" && idPago > 0) {
    //    btnEstado.disabled = true;
    //}
    //else {
    //    btnEstado.disabled = false;
    //}

    else if (estado == "Pagado" || estado == "Existe Deuda") {
        Opcion1.disabled = true;
        Opcion2.disabled = true;
        btnEstado.disabled = false;
        ActivarCampos();
    }
    else if (estado == "") {
        Opcion1.disabled = true;
        Opcion2.disabled = true;
        btnEstado.disabled = true;
    }
    else if (estado == "Sin Efectuar") {
        Opcion1.disabled = false;
        Opcion2.disabled = false;
        btnEstado.disabled = true;
        ActivarCampos();
    }


}

function Estado() {
    debugger;
/*    let idPago = document.getElementById("txtIdPago").value;*/
    let estado = document.getElementById("txtEstado").value;
    let btnEstado = document.getElementById("btnGuardarPago");

    if (estado == "Sin Efectuar" || estado=="") {
        btnEstado.disabled = true;
    }
    else {
        btnEstado.disabled = false;
    }
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
            let estado = fila.cells[4].textContent;
            const fecha = fila.cells[5].textContent;
            if (estado.toString() == "Nuevo") { estado = "Sin Efectuar" };

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
                        document.getElementById('txtPagoAPagar').value = Pagos.montoDeuda;
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
            document.getElementById('txtMontoPago').value = montoTotal;
            document.getElementById('txtDeuda').value = montoTotal;
            ActivarCampos();
            VerificarEstado();
        
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
            <td>${pedido.fechaPedido}</td>
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
        VerificarEstado();
     
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
    


document.getElementById("txtPagoCliente").addEventListener("input", function (event) {

        debugger;
        let pago = event.target.value;
        let idPago = document.getElementById("txtIdPago");
        let idPagos = parseFloat(idPago.value) || 0
/*      let monto = document.getElementById("txtMontoPedido");*/
        let descuento = document.getElementById("txtDescuento");
        let deuda = document.getElementById("txtDeuda");
        let descuentofinal = parseFloat(descuento.value) || 0
        let monto = document.getElementById("txtMontoPago");
        let Estado = document.getElementById("txtEstado");
        // if (idPagos > 0) {
        //    monto = document.getElementById("txtMontoPago");
           
        //}   

        if (!/^-?\d*\.?\d*$/.test(pago)) {
        event.target.value = pago.slice(0, -1);

        }
        else if (pago === "" ||  pago==0) {
            pago = 0;

            document.getElementById("txtDeuda").value = monto.value;
            document.getElementById("txtCambio").value = "0.00";
            document.getElementById("txtEstado").value = "0.00";
        }

    
        let montofinal = parseFloat(monto.value) || 0
          
        if (!isNaN(pago) && !isNaN(descuentofinal) && !isNaN(montofinal) && pago>=0) {

            Evaluar(pago, descuentofinal, montofinal);
        }

        else {
            pago.value = "0.00";
            alert("Ingrese numeros validos");
        }
});



document.getElementById("txtDescuento").addEventListener("input", function (event) {
    debugger;
    let descuento = event.target.value;
    if (!/^-?\d*\.?\d*$/.test(descuento)) {
        event.target.value = descuento.slice(0, -1);

    }

    let idPago = document.getElementById("txtIdPago").value;
    let monto = document.getElementById("txtMontoPedido");
    let DescuentoPago = document.getElementById("txtDescuento").value;

    if (idPago > 0) {
        monto = document.getElementById("txtMontoPago");
    }
    let pago = document.getElementById("txtPagoCliente");
    let pagofinal = parseFloat(pago.value) || 0
    let montofinal = parseFloat(monto.value) || 0

    if (descuento == "" || DescuentoPago == 0) {
        let MontoAPagar = document.getElementById("txtPagoAPagar");
        let MontoDePago = parseFloat(MontoAPagar.value);
        document.getElementById("txtMontoPago").value = MontoDePago;
        document.getElementById("txtDeuda").value = MontoDePago;
        document.getElementById("txtCambio").value = "0.00";
        descuento = 0.00;

    }
    else if (parseFloat(descuento)>montofinal) {
        descuento= 0.00;
        document.getElementById("txtDescuento").value = "0.00";
        document.getElementById("txtMontoPago").value = montofinal;
        document.getElementById("txtDeuda").value = montofinal;
        document.getElementById("txtPagoCliente").value = "0.00";
        document.getElementById("txtCambio").value = "0.00";
        let estado = montofinal < 0 ? "Error" : montofinal > 0 ? "Existe Deuda" : "Pagado";
        document.getElementById("txtEstado").value = estado;
        return;
    }

    if (!isNaN(pagofinal) && !isNaN(descuento) && !isNaN(montofinal)) {
        Evaluar(pagofinal, parseFloat(descuento), montofinal);
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
    let cambio=0.00;
    let estado = "";
    let deuda = document.getElementById("txtDeuda").value;
    let deudaFinal = parseFloat(deuda);
    let montofinal = montos - descuento;

    if (descuentos == 0) {
        let montoPago = document.getElementById("txtPagoAPagar").value;
        montos = parseFloat(montoPago);
        document.getElementById("txtMontoPago").value = montos;
    }
    else if (descuento=="") {
        document.getElementById("txtDescuento").value ="0.00";

    }

    else {
        document.getElementById("txtMontoPago").value = monto.toFixed(2);
    }

      
    if (pagos >= montos && pagos >= deudaFinal) {
            cambio = pagos - montofinal;
            deuda = 0.00;
            estado = "Pagado";
    }
    else {
            deuda = montofinal.toFixed(2) - pagos;
            cambio = deuda < 0 ? -1 * (deuda) : 0.00;
            deuda =  deuda > 0 ? deuda : 0.00;
            estado = (deuda == 0 && pagos == 0 && cambio == 0) ? "Sin Efectuar" :(deuda > 0 ? "Existe Deuda" : "Pagado");
    }

    document.getElementById("txtCambio").value = cambio.toFixed(2);
    document.getElementById("txtDeuda").value = deuda.toFixed(2);
    document.getElementById("txtEstado").value = estado;
    VerificarEstado();
 
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













