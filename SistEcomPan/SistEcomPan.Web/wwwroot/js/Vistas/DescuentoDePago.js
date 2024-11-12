
function mostrarModal2() {
    buscarProductos();
    DiccionarioDescuento = {};
    DiccionarioDevolucion = {};
    $("#modalDataDescuento").modal("show");
}

function EvaluarPago(pago, descuento, monto) {

    let pagos = parseFloat(pago);
    let montos = parseFloat(monto);
    let descuentos = descuento;

    let deuda = 0.00, cambio = 0.00;
    let estado = "";

    if (descuentos == 0) {
        let montoPedido = document.getElementById("txtMontoPedido").value;
        montos = montoPedido;
        //montofinal = montos - descuentos;
        //alert("monto final:"+montofinal)
    }
    if (pagos > montos) {
        cambio = pagos - montos;
        deuda = 0.00;
        estado = "Pagado";
    }

    else if (pagos <= montos) {
        cambio = 0.00;
        deuda = montos - pagos;
        estado = "Existe Deuda";

    }
    document.getElementById("txtMontoPago").value = montos.toFixed(2);
    document.getElementById("txtCambio").value = cambio.toFixed(2);
    document.getElementById("txtDeuda").value = deuda.toFixed(2);
    document.getElementById("txtEstado").value = estado;








}

$("#opcion1").click(function () {
    
    let busquedaCodPedido = $("#txtCodigoPedido").val();
    $("#CodPedido").val(busquedaCodPedido);
    let monto = $("#txtMontoPedido").val();
    $("#txtImportePedido").val(monto);
    mostrarModal2();
   
})

function eliminarCeros(unidadDescuento) {
    unidadDescuento.addEventListener("input", function () {
        this.value = this.value.replace(/^0+/, "");
    });
}

let DevolucionPedido;
function GuardarDevolucion() {
    debugger;
    const tablaDevoluciones = document.getElementById('tbDevoluciones');
    const filas = tablaDevoluciones.getElementsByTagName('tr');

    let detalleDevoluciones = [];

    for (let i = 1; i < filas.length; i++) {
        const fila = filas[i];
        const categoria = fila.cells[0].textContent;
        const descripcion = fila.cells[1].textContent;
        const precio = fila.cells[2].textContent;
        const cantidadPedido = fila.cells[3].textContent;
        const total = fila.cells[4].textContent;
        const cantidad = fila.cells[5].querySelector('input');
        let cantidadDevolucion = parseFloat(cantidad.value);

        const producto = {
            categoria: categoria,
            descripcion: descripcion,
            precio: precio,
            cantidadPedido: cantidadPedido,
            total: total,
            cantidadDevolucion: cantidadDevolucion
        };
        detalleDevoluciones.push(producto);
    }


    if (detalleDevoluciones.length < 1) {
        toastr.warning("", "Debes Ingresar Productos")
        return;
    }

    const vmDetalleDevolucion = detalleDevoluciones;

     DevolucionPedido = {
        codigoPedido: $("#CodPedido").val(),
        montoPedido: $("#txtImportePedido").val(),
        descuento: $("#txtDescuentoPedido").val(),
        montoAPagar: $("#txtImporteFinal").val(),
        DetalleDevolucion: vmDetalleDevolucion

    }



 /*   $("#btnGuardarDescuento").LoadingOverlay("show");*/
  
    //fetch("/Devolucion/Crear", {
    //    method: "POST",
    //    headers: { "Content-Type": "application/json;charset=utf-8" },
    //    body: JSON.stringify(Devolucion)
    //})
    //    .then(response => {
    //    /*    $("#btnGuardarDescuento").LoadingOverlay("hide");*/
    //        return response.ok ? response.json() : Promise.reject(response);
    //    })
    //    .then(responseJson => {
    //        if (responseJson.estado) {
    //            detalleDevoluciones = [];
    //          /*  $("#txtDocumentoCliente").val("")*/
    //            swal("Registrado", `Codigo de Producto:${responseJson.objeto.codigoPedido}`, "success")
    //        }
    //        else {
    //            swal("Lo sentimos", "No se pudo Registrar la Devolucion ", "error")

    //        }
    //    })


}


function EnviarDatosAModalPrincipal(datos) {
    const evento = new CustomEvent("enviarDatos", { detail: datos });
    document.dispatchEvent(evento);
}

function enviarDatos() {
    const datos = DevolucionPedido;
    EnviarDatosAModalPrincipal(datos);
}

$("#btnGuardarDescuento").click(function () {
    debugger
    let idPago = document.getElementById("txtIdPago").value;

    if (idPago>0) {
        let descuentoPedido = $("#txtDescuentoPedido").val();
        let PagoDelCliente = $("#txtPagoCliente").val();
        let Pago = parseFloat(PagoDelCliente) || 0;
        $("#txtDescuento").val(descuentoPedido);
        let descuentoFinal = $("#txtDescuento").val();
        let descuentoPago = parseFloat(descuentoFinal) || 0;
        let monto = $("#txtImporteFinal").val();
        $("#txtMontoPago").val(monto);
        $("#txtDeuda").val(monto);
        GuardarDevolucion();
        enviarDatos();
        $("#modalDataDescuento").modal("hide");
        EvaluarPago(Pago,descuentoPago, monto);
    }
    $("#modalDataDescuento").modal("hide");

})


const itemPaginaDes = 3; // Cantidad de productos por página
let  paginaActual= 1; // Página actual al cargar


function buscarProductos(busquedaDetalle = '', pagina = 1) {
    debugger;
    busquedaDetalle = document.getElementById("CodPedido").value;
    fetch(`/Pedido/ObtenerDetalleFinal?searchTerm=${busquedaDetalle}&page=${pagina}&itemsPerPage=${itemPaginaDes}`)
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
            <td id="precioProducto${i}">${parseFloat(detallePedido.precio).toFixed(2)}</td>
            <td id="cantidadProducto${i}">${detallePedido.cantidad}</td>
            <td>${detallePedido.total}</td>
            <td><input type="number" class="form-control form-control-sm" id="cantidadDescuento${i}" placeholder="Ingrese Unidades a Descontar"></td>
          `;
                productTable.appendChild(row);
                i++;
            });
            Paginacion(busquedaDetalle, totalItems);

            
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
      
       
}


function Paginacion(busquedaDetalle,totalItems) {

    // Generar la paginación    
    let Inicial = 1,Final = 3,Fin = 0;
    const totalPages = Math.ceil(totalItems / itemPaginaDes);
    const paginationDes = document.getElementById('paginacionDes');
    paginationDes.innerHTML = '';

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
        if (paginaActual === totalPages && li.classList.contains("active")) {
            let PaginaFinal = totalItems - itemPaginaDes * (i - 1);
            Fin = PaginaFinal;
            Final = Fin;

        }

        link.addEventListener('click', () => {
            paginaActual = i;
            buscarProductos(busquedaDetalle, paginaActual);
            resaltarPaginaActuales();

        });

        paginationDes.appendChild(li);
    }
    resaltarPaginaActuales();
    Devoluciones(Inicial, Final, paginaActual);


}

let DiccionarioDescuento = {};
let DiccionarioDevolucion = {};



function Devoluciones(inicial, final, paginaActual) {
    debugger;
    CantidadInicial(inicial, final);
    const Montofinal = document.getElementById("txtImporteFinal");
    const MontoInicial = document.getElementById("txtImportePedido");
    const Descuentos = document.getElementById("txtDescuentoPedido");
    let DescuentoGeneral = 0;
    for (let i = inicial; i <= final; i++) {
        const CantidadDescuento = document.getElementById(`cantidadDescuento${i}`);
        const valorDescuento = CantidadDescuento.textContent.trim();
        DevolucionProducto(inicial, final, paginaActual);
        CantidadDescuento.addEventListener("input", function () {
            debugger;
                let descuentoTotal = 0, subtotal = 0, unidadDePan = 27;
                for (let j = inicial; j <= final; j++) {
                  
                        let categorias = document.getElementById(`categoriaProducto${j}`).textContent;
                        let precio = document.getElementById(`precioProducto${j}`).textContent;
                        let cantidad = document.getElementById(`cantidadProducto${j}`).textContent;
                        let unidadDescuento = document.getElementById(`cantidadDescuento${j}`);
                        eliminarCeros(unidadDescuento);
                        let descuento = parseFloat(unidadDescuento.value) || 0;
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
                    if (categorias == "Panes" && descuento >= 0 && descuento <= unidadDePan * cantidad && !isNaN(unidadDescuento.value)) {
                        subtotal = descuento * precioUnitario;
                        DiccionarioDescuento[`descuentos${paginaActual}${j}`] = subtotal;
                        DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = descuento;

                        descuentoTotal += subtotal;
                    }
                    else if (categorias != "Panes" && descuento >= 0 && descuento <= cantidad && !isNaN(unidadDescuento.value)) {
                        subtotal = descuento * precio;
                        DiccionarioDescuento[`descuentos${paginaActual}${j}`] = subtotal;
                        DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = descuento;
                        descuentoTotal += subtotal;
                    }
                    else if (valorDescuento !== "" || descuento < 0 || descuento > cantidad || descuento > unidadDePan * cantidad) {
                        unidadDescuento.value = 0;
                        DiccionarioDescuento[`descuentos${paginaActual}${j}`] = 0;
                        DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = 0;
                        alert("La Cantidad debe ser menor");
                    }
                    else if (isNaN(unidadDescuento.value)) {
                        unidadDescuento.value = 0;
                        DiccionarioDescuento[`descuentos${paginaActual}${j}`] = 0;
                        DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = 0;
                        alert("Ingrese valores correctos");
                    }

               
                    
                }
                DescuentoGeneral += descuentoTotal;
                alert("Descuentos finales");
                Object.keys(DiccionarioDescuento).forEach(clave => {
                    alert(`Clave:${clave},Valor:${DiccionarioDescuento[clave]}`)
                });

                Object.keys(DiccionarioDevolucion).forEach(clave => {
                    alert(`Clave:${clave},Valor:${DiccionarioDevolucion[clave]}`)
                });

                let sumaDiccionario = SumarDescuento();
                alert("suma final:" + sumaDiccionario);
                Descuentos.value = sumaDiccionario.toFixed(2);
                Montofinal.value = (MontoInicial.value - sumaDiccionario).toFixed(2);
            });

        
    }

}

function DevolucionProducto(inicial, final, paginaActual) {

    for (let j = inicial; j <= final; j++) { 
        if (DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] !== undefined) {
            let unidadDescuento = document.getElementById(`cantidadDescuento${j}`);
            unidadDescuento.value = DiccionarioDevolucion[`devoluciones${paginaActual}${j}`];
        }
    }

}

function CantidadInicial(inicial, final) {

    for (let j = inicial; j <= final; j++) { 
        document.getElementById(`cantidadDescuento${j}`).value = "0";
    }

}

function SumarDescuento() {

    let suma = 0;

    for(let clave in DiccionarioDescuento) {

        suma += DiccionarioDescuento[clave];
    }
    return suma;
    
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
function resaltarPaginaActuales() {
    const paginationItem = document.querySelectorAll('#paginacionDes .page-item');
    paginationItem.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === paginaActual.toString()) {
            item.classList.add('active');
        }
    });
}



























