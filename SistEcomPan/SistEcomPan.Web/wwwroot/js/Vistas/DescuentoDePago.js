﻿
function mostrarModal2() {
    buscarProductos();
    DiccionarioDescuento = {};
    DiccionarioDevolucion = {};
    $("#modalDataDescuento").modal("show");
}

$("#opcion1").click(function () {
    
    let busquedaCodPedido = $("#txtCodigoPedido").val();
    $("#searchInput").val(busquedaCodPedido);
    let monto = $("#txtMontoPedido").val();
    $("#txtImportePedido").val(monto);
    mostrarModal2();
   
})

$("#btnGuardarDescuento").click(function () {
    debugger
    let idPago = document.getElementById("txtIdPago").value;

    if (idPago>0) {
        let descuentoPedido = $("#txtDescuentoPedido").val();
        $("#txtDescuento").val(descuentoPedido);
        let monto = $("#txtImporteFinal").val();
        $("#txtMontoPago").val(monto);
        $("#txtDeuda").val(monto);
    }
    $("#modalDataDescuento").modal("hide");

})




//document.getElementById("txtPagoCliente").addEventListener("click", function () {



//    let pago = document.getElementById("txtPagoCliente").value;yo 17w7byash5aq

//    let descuento = document.getElementById("txtDescuento").value;
//    let monto = document.getElementById("txtMontoPedido").value;

//    if (!isNaN(pago) && !isNaN(descuento) && !isNaN(monto)) {
//        if (pago == "") pago = 0, descuento = 0;
//        Evaluar(pago, descuento, monto);
//    }

//    else {

//        alert("Ingrese numeros validos");
//    }



//});





const itemPaginaDes = 3; // Cantidad de productos por página
let  paginaActual= 1; // Página actual al cargar


function buscarProductos(busquedaDetalle = '',pagina = 1) {
    busquedaDetalle = document.getElementById("searchInput").value;
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
            <td id="precioProducto${i}">${detallePedido.precio}</td>
            <td id="cantidadProducto${i}">${detallePedido.cantidad}</td>
            <td>${detallePedido.total}</td>
            <td><input type="text" class="form-control form-control-sm" id="cantidadDescuento${i}" placeholder="Ingrese Unidades a Descontar"></td>
          `;
                productTable.appendChild(row);
                i++;
            });

           
            // Generar la paginación
            const totalPages = Math.ceil(totalItems / itemPaginaDes);
            const paginationDes = document.getElementById('paginacionDes');
            paginationDes.innerHTML = '';
            let Inicial = 1;
            let Final = 3;
            let Fin = 0;
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
                    let PaginaFinal = totalItems - itemPagina * (i - 1);
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
            Devoluciones(Inicial,Final,paginaActual,totalPages);
             

            
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
      
        
}

let DiccionarioDescuento = {};
let DiccionarioDevolucion = {};

function Devoluciones(inicial, final, paginaActual,totalPaginas) {
    alert("devolucion final:" + final + paginaActual);
  
    const Montofinal = document.getElementById("txtImporteFinal");
    const MontoInicial = document.getElementById("txtImportePedido");
    const Descuentos = document.getElementById("txtDescuentoPedido");
    let DescuentoGeneral = 0;
    for (let i = inicial; i <= final; i++) {
        alert("finales:" + final);
        const CantidadDescuento = document.getElementById(`cantidadDescuento${i}`);
        const valorDescuento = CantidadDescuento.textContent.trim();
        DevolucionProducto(inicial, final, paginaActual);
            CantidadDescuento.addEventListener("input", function () {
                let descuentoTotal = 0, subtotal = 0, unidadDePan = 27;
                for (let j = inicial; j <= final; j++) {
                        alert("finalmente:" + final);    
                  
                        let categorias = document.getElementById(`categoriaProducto${j}`).textContent;
                        let precio = document.getElementById(`precioProducto${j}`).textContent;
                        let cantidad = document.getElementById(`cantidadProducto${j}`).textContent;
                        let unidadDescuento = document.getElementById(`cantidadDescuento${j}`);
                        let descuento = parseFloat(unidadDescuento.value) || 0;
                        let descripcionNueva = document.getElementById(`descripcionProducto${j}`).textContent;
                        let precioUnitario;
                        alert(descuento);
                   
                        const UnidadesPan = descripcionNueva.match(/\d+/);
                        if (UnidadesPan) {
                            unidadDePan = parseInt(UnidadesPan[0]);
                            precioUnitario = precio / unidadDePan;
                        }
                        else {
                            precioUnitario = precio / unidadDePan;
                        }

                        alert("unidades:" + unidadDePan + "cantidad:" + cantidad + "precioUni:" + precioUnitario);
                        if (categorias == "Panes" && descuento > 0 && descuento <= unidadDePan * cantidad) {
                            subtotal = descuento * precioUnitario;
                            DiccionarioDescuento[`descuentos${paginaActual}${j}`] = subtotal;
                            DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = descuento;
                      
                            descuentoTotal += subtotal;
                            alert("descuentosubtotal:" + descuentoTotal + "subtotal:" + subtotal);
                        }
                        else if (categorias != "Panes" && descuento > 0 && descuento <= cantidad) {
                            subtotal = descuento * precio;
                            DiccionarioDescuento[`descuentos${paginaActual}${j}`] = subtotal;
                            DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = descuento;
                    
                            descuentoTotal += subtotal;
                            alert("descuentosubtotal:" + descuentoTotal + "subtotal:" + subtotal);
                        }
                        else if (valorDescuento !== "" || descuento < 0 || descuento > cantidad || descuento > unidadDePan * cantidad) {                        
                            unidadDescuento.value = 0;
                            DiccionarioDescuento[`descuentos${paginaActual}${j}`] = 0;
                            DiccionarioDevolucion[`devoluciones${paginaActual}${j}`] = 0;
                            alert("La Cantidad debe ser menor");
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

// Evento cuando el usuario escribe en la barra de búsqueda
//document.getElementById('searchInput').addEventListener('input', function (event) {
//    const busquedaDetallePedido = event.target.value;
//    paginaActual = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
//    buscarProductos(busquedaDetallePedido, paginaActual);
//});

//Llamada inicial para cargar productos al abrir la tabla modal
//$('#modalData').on('show.bs.modal', function () {
//    buscarProductos();
//});

























