
if (userRol === "Administrador") {

function ObtenerDatosUsuario() {
    fetch("/Home/ObtenerUsuario")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto
                $("#userDropdown img.img-profile").attr("src", `/ImagenesPerfil/${d.nombreFoto}`);

            }
            else {
                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })
}
}
else if (userRol === "Cliente") {
    function ObtenerDatosCliente() {
        fetch("/Home/ObtenerCliente")
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    const d = responseJson.objeto
                    $("#userDropdown img.img-profile").attr("src", `/ImagenesPerfil/${d.nombreFoto}`);
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }

}







$(document).ready(function () {
    /*  obtenerFecha();*/

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaDeEntrega").datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear:true
    })

    if (!$("#txtFechaEntrega").val()) {
        $("#txtFechaEntrega").val($.datepicker.formatDate('dd/mm/yy', new Date()));
    }


    ObtenerDatosUsuario();
    fetch("/Pedido/ListaNombres")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboNombreCliente").append(
                        $("<option>").val(item.idCliente).text(item.nombreCompleto)
                    )
                })
            }
        })

})

$("#btnCliente").click(function () {
    mostrarClientes();
})


function mostrarClientes() {
/*    obtenerFecha();*/
    buscarClientes();
    $("#modalDataPedidoCliente").modal("show");
}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById('ClienteBuscado').addEventListener('click', function (event) {


        if (event.target.tagName == 'TD') {

            const fila = event.target.parentNode;
            const nombreCompleto = fila.cells[2].textContent;

            document.getElementById('txtNombreCliente').value = nombreCompleto;

            $("#modalDataPedidoCliente").modal("hide");
        }

    });
});


const ClientesPorPagina = 4; // Cantidad de productos por página
let PagClienteInicial = 1; // Página actual al cargar


function buscarClientes(searchTer = '', page = 1) {
    fetch(`/Cliente/ObtenerClientes?searchTerm=${searchTer}&page=${page}&itemsPerPage=${ClientesPorPagina}`)
        .then(response => response.json())
        .then(data => {
            const clientes = data.clientes; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            /*  const nombre = data.nombresCompletos;*/
            // Actualizar la tabla modal con los productos obtenidos 
            const clientTable = document.getElementById('ClienteBuscado');
            clientTable.innerHTML = '';

            clientes.forEach(cliente => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td class="text-nowrap d-none d-sm-table-cell">${cliente.idCliente}</td>
            <td>${cliente.dni}</td>
            <td class="nombres">${cliente.nombreCompleto}</td>
            <td class="text-nowrap d-none d-sm-table-cell">${cliente.correo}</td>
            <td class="text-nowrap d-none d-sm-table-cell">${cliente.direccion}</td>
            <td>${cliente.telefono}</td>
          `;
                clientTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ClientesPorPagina);
            const pagination = document.getElementById('paginacionCliente');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === PagClienteInicial) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    PagClienteInicial = i;
                    buscarClientes(searchTer, PagClienteInicial);
                    resaltarPagCliente();
                });

                pagination.appendChild(li);
            }

            resaltarPagCliente();
        })
        .catch(error => {
            console.error('Error al buscar clientes:', error);
        });
}


function resaltarPagCliente() {
    const paginationItems = document.querySelectorAll('#PaginacionCliente .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PagClienteInicial.toString()) {
            item.classList.add('active');
        }
    });
}


document.getElementById('BuscarNombreCliente').addEventListener('input', function (event) {
    const Busqueda = event.target.value;
    PagClienteInicial = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    buscarClientes(Busqueda, PagClienteInicial);
});





function obtenerFecha() {
    let fechaActual = new Date();
    let fechaTexto = document.getElementById("txtFecha");
    let dia = fechaActual.getDate();
    let mes = fechaActual.getMonth() + 1;
    let anio = fechaActual.getFullYear();

    if (dia < 10) {
        dia = '0' + dia;
    }
    if (mes<10) {
        mes = '0' + mes;
    }
    let fechaformateada = dia + '/' + mes + '/' + anio;
    fechaTexto.value = fechaformateada;
}


if (userRol==="Administrador") {
    /* let DniCliente = document.getElementById("DniPersonal").textContent;*/
    document.getElementById("btnCliente").disabled = false;
    $("#ClienteBuscado").on('click', 'tr', function () {
        var nombreCliente = $(this).find('.nombres').text();
        $("#ClienteBuscado").removeClass('selected');
        $(this).addClass('selected');
        fetch(`/Pedido/ListaNumeroDocumento?nombreCompleto=${nombreCliente}`)
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.length > 0) {
                    responseJson.forEach((item) => {
                        $("#txtDocumentoCliente").val(item.dni)
                        $("#txtDireccionCliente").val(item.direccion)
                        $("#txtTelefonoCliente").val(item.telefono)
                    })
                }
            })

    })

    $("#txtDocumentoCliente").click(function () {
        if ($("#txtDocumentoCliente").val().length == 8) {
            var numeroDocumento = $("#txtDocumentoCliente").val();
            fetch(`/Pedido/ListaClientes?numeroDocumento=${numeroDocumento}`)
                .then(response => {
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.length > 0) {
                        responseJson.forEach((item) => {
                            $("#txtNombreCliente").val(item.nombreCompleto);
                            $("#txtDireccionCliente").val(item.direccion);
                            $("#txtTelefonoCliente").val(item.telefono);
                        })
                    }
                })
        }

    })


    

}
else if (userRol === "Cliente") {
    document.getElementById("btnCliente").disabled = true;
    let DniCliente = document.getElementById("DniPersonal").textContent;
    let nombresCompletos = document.getElementById("NombreCompleto").textContent;
    document.getElementById("txtDocumentoCliente").value=DniCliente;
    document.getElementById("txtNombreCliente").value = nombresCompletos;

    fetch(`/Pedido/ListaClientes?numeroDocumento=${DniCliente}`)
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#txtDireccionCliente").val(item.direccion);
                    $("#txtTelefonoCliente").val(item.telefono);
                })
            }
        })
   
    
}
function ModalPedidos() {
    MostrarProduct();
  
    $("#modalData").modal("show");
   
}

$("#btnGuardar").click(function () {
    ModalPedidos();
})

const ElementosDePagina = 3; // Cantidad de productos por página
let actualDePagina = 1; // Página actual al cargar
let indice =actualDePagina-1;
function MostrarProduct(TerminoBusqueda = '', pagina = 1) {
    fetch(`/Pedido/ObtenerProductos?searchTerm=${TerminoBusqueda}&page=${pagina}&itemsPerPage=${ElementosDePagina}`)
        .then(response => response.json())
        .then(data => {
            const productos = data.productos; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            const categoria = data.categoria;
            let nombreCarpeta = /ImagenesProducto/;
      
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('ProductoBuscado');
            productTable.innerHTML = '';
      
            productos.forEach(producto => {
                const row =  document.createElement('tr');
                row.innerHTML = `
            <td class="d-none d-md-table-cell">${producto.idProducto}</td>
            <td>${producto.descripcion}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
            <td class="d-none d-md-table-cell">${categoria[indice]} </td>
            <td class="d-none d-md-table-cell">${producto.stock} </td>
            <td>${producto.precio.toFixed(2)}</td>
            <td><input type="text" class="form-control form-control-sm txtCantidad" id="txtCantidad" placeholder="Ingrese Cantidad"></td>
            <td><img style = "height:60px" src = ${nombreCarpeta}${producto.nombreImagen} class="rounded mx-auto d-block" /></td>
            <td class="d-flex">
            <button onclick="agregarProducto(this)" class="btn btn-danger btn-sm mr-2" style="display:inline-block;"><i class="fa fa-plus"></i></button>

            </td>
          `;
                indice++;
                productTable.appendChild(row);
                

         });
            SeleccionProductos();
            Paginacion(TerminoBusqueda, totalItems);
                

        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}

document.addEventListener("input", function (event) {
    if (event.target.classList.contains("txtCantidad")) { 
    let cantidad = event.target.value;
     event.target.value = cantidad.replace(/\D/g,'');

    }
});


function Paginacion(TerminoBusqueda,totalItems) {

    // Generar la paginación
    const totalPag = Math.ceil(totalItems / ElementosDePagina);
    const PaginaPag = document.getElementById('Paginations');
    PaginaPag.innerHTML = '';

    for (let i = 1; i <= totalPag; i++) {
        const liA = document.createElement('li');
        liA.classList.add('page-item');
        const linkA = document.createElement('a');
        linkA.classList.add('page-link');
        linkA.href = '#';
        linkA.textContent = i;
        liA.appendChild(linkA);

        if (i === actualDePagina) {

            liA.classList.add('active');
        }

        linkA.addEventListener('click', () => {
            actualDePagina = i;
            indice = actualDePagina * ElementosDePagina - ElementosDePagina;
            MostrarProduct(TerminoBusqueda, actualDePagina);
            resaltarPagActual();

        });

        PaginaPag.appendChild(liA);
    }

    resaltarPagActual();

}

function SeleccionProductos() {
    const filas = document.querySelectorAll("#ProductoBuscado tbody tr");
    filas.forEach(fila => {
        fila.addEventListener("click", function () {
            alert("filas filas");

            const stock = parseFloat(fila.cells[3].textContent);
            const cantidadIngresada = parseFloat(fila.cells[5].querySelector('input').value);
            const imputElement = fila.cells[5].querySelector('input');

            if (isNaN(cantidadIngresada) || cantidadIngresada <= 0) {
                imputElement.value = "0";
                alert("Ingrese una cantidad valida");
            }
            else if (cantidadIngresada > stock) {
                imputElement.value = "0";
                alert("La cantidad supera al stock");
            }

        });
    });
}

//document.addEventListener("DOMContentLoaded", function () {

//    ModalPedidos();
//});


function agregarProducto(button) {
    debugger;
    const row = button.parentNode.parentNode;
    const IdProducto = row.cells[0].textContent;
    const descripcion = row.cells[1].textContent;
    const categoria = row.cells[2].textContent;
    const stock = parseFloat(row.cells[3].textContent);
    const precio = parseFloat(row.cells[4].textContent).toFixed(2);
    const cantidadIngresada = row.cells[5].querySelector('input');

    let cantidad = parseFloat(cantidadIngresada.value)||0;

    if (isNaN(cantidad) || cantidad <= 0) {
        alert("Ingrese una cantidad valida");
    }
    else if (cantidad > stock) {
        alert("La cantidad supera al stock");
    }
    else if (cantidad <= stock) {

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
                if (nuevaCantidad <= stock) {
                    fila.cells[3].textContent = nuevaCantidad;
                    cantidadTotal = nuevaCantidad;
                    cantidad = cantidadTotal;
                    total = precio * cantidad;
                    fila.cells[5].textContent = total.toFixed(2);
                    calcularTotal();
                    return;
                }
                return;
            }


        }
        total = precio * cantidad;
        const nuevaFila = `
      <tr>
        <td class="d-none d-sm-table-cell">${IdProducto}</td>
        <td>${descripcion}</td>
        <td class="d-none d-sm-table-cell">${categoria}</td>
        <td>${cantidad}</td>
        <td>${precio}</td>
        <td>${total.toFixed(2)}</td>
        <td><button class="btn btn-primary btn-sm" onclick="eliminarProducto(this)"><i class="fas fa-trash-alt" aria-hidden="true"></i></button></td>
      </tr>
    `;

        document.getElementsByTagName('tbody')[0].insertAdjacentHTML('beforeend', nuevaFila);

        calcularTotal();
    }
}

function ajustarColspan() {
    const anchoPantalla = window.innerWidth;
    const tdTotal = document.getElementById("textoTotal");

    if (anchoPantalla < 576) {
        tdTotal.colSpan = 3;
    } else {
        tdTotal.colSpan = 5;
    }
}

window.addEventListener("resize", ajustarColspan);
window.addEventListener("DOMContentLoaded", ajustarColspan);

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
function resaltarPagActual() {
    const paginationItems = document.querySelectorAll('#Paginations .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === actualDePagina.toString()) {
            item.classList.add('active');
        }
    });
}

// Evento cuando el usuario escribe en la barra de búsqueda
document.getElementById('BusquedaPedidos').addEventListener('input', function (event) {
    const TerminoBusqueda = event.target.value;
    actualDePagina = 1; // Reiniciar a la primera página al realizar una nueva búsqueda
    indice = 0;
    MostrarProduct(TerminoBusqueda, actualDePagina);
});

 //Llamada inicial para cargar productos al abrir la tabla modal
//$('#modalData').on('show.bs.modal', function () {
//    buscarProductos();
//});

function formatoFecha() {
    const fechaFormateada = $("#txtFechaDeEntrega").val();
    const partes = fechaFormateada.split("/");
    const fechaFormato = `${partes[2]}-${partes[1]}-${partes[0]}`;
    return fechaFormato;
}

$("#btnEnviarPedido").click(function () {
    debugger;

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const tablaProductos = document.getElementById('tbProductosSeleccionados');
    const filas = tablaProductos.getElementsByTagName('tr');

    let productosPedidos = [];

    for (let i = 1; i < filas.length - 1; i++) {
        const fila = filas[i];
        const idProducto = fila.cells[0].textContent;
        const cantidad = fila.cells[3].textContent;
        const total = fila.cells[5].textContent;

        const producto = {
            idProducto: idProducto,
            cantidad: cantidad,
            total: total
        };
        productosPedidos.push(producto);
    }


    if (productosPedidos.length < 1) {
        toastr.warning("", "Debes Ingresar Productos")
        return;
    }

    const vmDetallePedido = productosPedidos;

    const pedido = {
        dni: $("#txtDocumentoCliente").val(),
        montoTotal: $("#montoTotal").text(),
        estado: "Nuevo",
        fechaDeEntrega: formatoFecha(),
        DetallePedido: vmDetallePedido

    }



    $("#btnEnviarPedido").LoadingOverlay("show");
    debugger;
    fetch("/Pedido/Crear", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(pedido)
    })
        .then(response => {
            $("#btnEnviarPedido").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                productosPedidos = [];
                $("#txtDocumentoCliente").val("")
                swal("Registrado", `Nuevo Pedido:${responseJson.objeto.codigo}`, "success")
            }
            else {
                swal("Lo sentimos", "No se pudo Registrar el Pedido", "error")

            }
        })
})


























