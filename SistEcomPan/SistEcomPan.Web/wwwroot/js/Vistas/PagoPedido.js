
const MODELO_BASE = {
    idPago: 0,
    idPedido:"",
    montoDePedido: "",
    descuento: "",
    montoTotalDePago: "",
    montoDeuda: "",
    nombreCliente: "",
    fechaPago: "",
    fechaPedido:"",
    estado: ""

}

let tablaDataPago;
const itemPagina = 4; 

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

function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}


$(document).ready(function () {
    ObtenerDatosUsuario();
    fetch("/Usuario/ListaRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.nombreRol)
                    )
                })
            }
        })

    tablaDataPago = $('#tbdataPago').DataTable({
        responsive: {
            details: {
                type: 'column',        // ← Cambia de false a 'column'
                target: 'tr'
            }
        },
        columnDefs: [
            { responsivePriority: 1, targets: 5 },  // Nombre cliente: siempre visible
            { responsivePriority: 2, targets: 8 },  // Acciones: siempre visible
            { responsivePriority: 3, targets: 3 },  // Monto pago: prioritario
            { responsivePriority: 4, targets: 7 },  // Estado: prioritario
            { responsivePriority: 5, targets: 6 },  // Fecha pago
            { responsivePriority: 6, targets: 1 },  // Monto pedido
            { responsivePriority: 7, targets: 2 },  // Descuento
            { responsivePriority: 8, targets: 4 },  // Deuda
            { responsivePriority: 9, targets: 0 },  // IdPago: última prioridad
        ],
        "ajax": {
            "url": '/Pago/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPago", "searchable": false},
            { "data": "montoDePedido"},
            { "data": "descuento"},
            { "data": "montoTotalDePago"},
            { "data": "montoDeuda"},
            { "data": "nombreCliente"},
            {
                "data": "fechaPago", render: function (data) {
                    return cambiarFecha(data);
                }
            },
            {
                "data": "estado", render: function (data) {
                    if (data == "Pagado")
                        return '<span class="badge badge-info">Pagado</span>';
                    else if (data == "Existe Deuda")
                        return '<span class="badge badge-danger">Existe Deuda</span>';
                    else
                        return '<span class="badge badge-success">Sin Efectuar</span>';
                }

            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px",
                responsivePriority: 2

            }
        ],
        order: [[0, "desc"]],
        paging: true,
        pageLength: itemPagina,
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },
    });
})

function formatoFecha(fechaOriginal) {

    let fechaFormateada = "";
    if (fechaOriginal != "") {
        let fecha = new Date(fechaOriginal);
        let dia = fecha.getDate().toString().padStart(2, "0");
        let mes = (fecha.getMonth() + 1).toString().padStart(2, "0");
        let anio = fecha.getFullYear();

        fechaFormateada = dia + "/" + mes + "/" + anio;
        return fechaFormateada;
    }
    return fechaFormateada;

}

function mostrarModal3() {
    obtenerFecha();
    buscarPedidos();
    $("#modalDataPedidos").modal("show");

}

function ActivarCampos() {
    $("#txtDescuento").prop("disabled", false)
    $("#txtPagoCliente").prop("disabled", false)
}

function EstadoPedido() {
    debugger;

    let MontoTotal = document.getElementById("txtMontoPago").value;

    if (estado == "Pagado" && MontoTotal == "0.00") {
        btnEstado.disabled = true;
        Opcion1.disabled = true;
        Opcion2.disabled = true;
    }

    else if (estado == "Pagado" || estado == "Existe Deuda") {
        Opcion1.disabled = true;
        Opcion2.disabled = true;
        btnEstado.disabled = false;
        ActivarCampos();
    }
    else if (estado == "Sin Efectuar") {
        Opcion1.disabled = false;
        Opcion2.disabled = false;
        btnEstado.disabled = true;
        ActivarCampos();
    }


}


function VerificarEstadoPedido() {
    debugger;

    let montoPedido = document.getElementById("txtMontoPedido").value;
    let montoAPagar = document.getElementById("txtMontoPago").value;

    let idPago = document.getElementById("txtIdPago").value;
    let estado = document.getElementById("txtEstado").value;
    let btnEstado = document.getElementById("btnGuardarPago");
    let Opcion1 = document.getElementById("opcion1");
    let Opcion2 = document.getElementById("opcion2");
    if (estado === "Pagado" && idPago > 0) {
        btnEstado.disabled = true;
    }
    else {
        btnEstado.disabled = false;
    }

    if ((estado === "Existe Deuda" || estado === "Sin Efectuar") && montoPedido === montoAPagar) {
        Opcion1.disabled = false;
        Opcion2.disabled = false;
        ActivarCampos();
    }
    else if (estado === "Existe Deuda" && montoPedido !== montoAPagar){
        Opcion1.disabled = true;
        Opcion2.disabled = true;
        ActivarCampos();
    }


}

function mostrarModal(modelo = MODELO_BASE) {
    debugger;

    $("#txtIdPago").val(modelo.idPago)
    $("#txtIdPedido").val(modelo.idPedido)
    $("#txtPagoAPagar").val(modelo.montoDeuda)
    $("#txtMontoPedido").val(modelo.montoDePedido)
    $("#txtNombres").val(modelo.nombreCliente)
    $("#txtFechaPedido").val(formatoFecha(modelo.fechaPedido))
    $("#txtFechaPago").val(formatoFecha(modelo.fechaPago))
    $("#txtDescuento").val("0.00")
    $("#txtDeuda").val(modelo.montoDeuda)
    $("#txtMontoPago").val(modelo.montoDeuda)
    $("#txtPagoCliente").val("0.00")
    $("#txtCambio").val("0.00")
    $("#txtEstado").val(modelo.estado)
    $("#txtCodigoPedido").val(modelo.codigoPedido)


    $("#modalDataPago").modal("show")
    DesacticarCamposPago();
    VerificarEstadoPedido();
}

function DesacticarCamposPago() {
    $("#txtMontoPedido").prop("disabled",true)
    $("#txtNombres").prop("disabled", true)
    $("#txtFechaPedido").prop("disabled", true)
    $("#txtFechaPago").prop("disabled", true)
    $("#txtDeuda").prop("disabled", true)
    $("#txtMontoPago").prop("disabled", true)
    $("#txtCambio").prop("disabled", true)
    $("#txtEstado").prop("disabled", true)
    $("#txtCodigoPedido").prop("disabled", true)
    $("#txtDescuento").prop("disabled", true)
    $("#txtPagoCliente").prop("disabled", true)

}


    $("#btnNuevoPago").click(function () {
    mostrarModal()
    })

$("#btnClosePago").click(function () {
    $("#txtPagoDelCliente").val("")
})

$("#btnGuardarDescuento").click(function () {
    //let pago = document.getElementById("txtPagoCliente").value;
    //let idPago = document.getElementById("txtIdPago");
    //let idPagos = parseFloat(idPago.value) || 0
    ///*      let monto = document.getElementById("txtMontoPedido");*/
    //let descuento = document.getElementById("txtDescuento");
    //let deuda = document.getElementById("txtDeuda");
    //let descuentofinal = parseFloat(descuento.value) || 0
    //let monto = document.getElementById("txtMontoPago");
    //let estado = document.getElementById("txtEstado");
    //// if (idPagos > 0) {
    ////    monto = document.getElementById("txtMontoPago");

    ////}
    //let pagoInput = parseFloat(pago) || 0;

    ////if (!/^-?\d*\.?\d*$/.test(pago)) {
    ////event.target.value = pago.slice(0, -1);

    ////}

    //if (pagoInput === 0 && descuentofinal === 0) {
    //    /*   pago = 0;*/

    //    document.getElementById("txtDeuda").value = monto.value;
    //    document.getElementById("txtCambio").value = "0.00";
    //    document.getElementById("txtEstado").value = "0.00";
    //    estado.value = "Sin Efectuar";
    //    VerificarEstado();
    //    return;
    //}


    //let montofinal = parseFloat(monto.value) || 0

    //if (!isNaN(pago) && !isNaN(descuentofinal) && !isNaN(montofinal) && pago >= 0) {

    //    EvaluarDescuento(pago, descuentofinal, montofinal);
    //}

    debugger;
    //let descuentoPago = event.target.value;
    //if (!/^-?\d*\.?\d*$/.test(descuentoPago)) {
    //    event.target.value = descuentoPago.slice(0, -1);

    //}
    //if (!/^\d*\.?\d{0,2}$/.test(descuentoPago)) {
    //    event.target.value = descuento.slice(0, -1);
    //    return;
    //}

    let estadoInput = document.getElementById("txtEstado");
    let idPago = document.getElementById("txtIdPago").value;
    let monto = document.getElementById("txtMontoPedido");
    let descuentoFin = document.getElementById("txtDescuentoPedido").value;

    let MontoAPagar = document.getElementById("txtMontoPago");
    let MontoDePago = parseFloat(MontoAPagar.value);
    if (idPago > 0) {
        monto = document.getElementById("txtMontoPago");
    }
    let pago = document.getElementById("txtPagoCliente");
    let pagofinal = parseFloat(pago.value) || 0
    let montofinal = parseFloat(monto.value) || 0
    let descuento = parseFloat(descuentoFin) || 0;

    if (descuento === 0 && pagofinal === 0) {
        document.getElementById("txtMontoPago").value = MontoDePago.toFixed(2);
        document.getElementById("txtDeuda").value = MontoDePago.toFixed(2);
        document.getElementById("txtCambio").value = "0.00";
        estadoInput.value = "Sin Efectuar";
        descuento = 0.00;
        return;

    }
    else if (parseFloat(descuento) > montofinal) {
        descuento = 0.00;
        document.getElementById("txtDescuento").value = "0.00";
        document.getElementById("txtMontoPago").value = montofinal.toFixed(2);
        document.getElementById("txtDeuda").value = montofinal.toFixed(2);
        document.getElementById("txtPagoCliente").value = "0.00";
        document.getElementById("txtCambio").value = "0.00";
        /*       let estado = montofinal < 0 ? "Error" : montofinal > 0 ? "Existe Deuda" : "Pagado";*/
        document.getElementById("txtEstado").value = "Sin Efectuar";
        return;
    }

    if (!isNaN(pagofinal) && !isNaN(descuento) && !isNaN(montofinal)) {
        EvaluarDescuento(pagofinal, parseFloat(descuento), montofinal);
    }

    else {
        alert("Ingrese numeros validos");
        document.getElementById("txtDescuento").value = "0.00";
    }


})



function EvaluarDescuento(pago, descuento, monto) {
    debugger;
    let pagos = parseFloat(pago) || 0;
    let montos = parseFloat(monto);
    let descuentos = descuento;
    let cambio = 0.00;
    let estado = "";
    let deuda = document.getElementById("txtDeuda").value;
    let deudaFinal = parseFloat(deuda);
    let montofinal = montos.toFixed(2) - descuento.toFixed(2);

    let montoPago = document.getElementById("txtPagoAPagar").value;

    if (pagos === 0) {
        cambio = 0.00;
        deuda = montofinal;
        estado = "Sin Efectuar";
    }


    else if (pagos >= montos && pagos >= deudaFinal) {
        cambio = pagos - montofinal;
        deuda = 0.00;
        estado = "Pagado";
    }
    else {
        deuda = montofinal.toFixed(2) - pagos;
        cambio = deuda < 0 ? -1 * (deuda) : 0.00;
        deuda = deuda > 0 ? deuda : 0.00;
        estado = (deuda == 0 && pagos == 0 && cambio == 0) ? "Sin Efectuar" : (deuda > 0 ? "Existe Deuda" : "Pagado");
    }

    document.getElementById("txtCambio").value = cambio.toFixed(2);
    document.getElementById("txtDeuda").value = deuda.toFixed(2);
    document.getElementById("txtEstado").value = estado;
    document.getElementById("txtMontoPago").value = montos.toFixed(2);

}




document.addEventListener("enviarDatos", (event) => {
    const datos = event.detail;
    actualizarModalPrincipal(datos);
});

let Devolucion;

debugger;
function actualizarModalPrincipal(datos) {
    Devolucion = datos;
}


$("#btnGuardarPago").click(function () {

        debugger
        let detallePagos = [];

        const montoAPagar = document.getElementById("txtMontoPago").value;
        const pagoDelCliente = document.getElementById("txtPagoCliente").value;
        const deudaDelCliente = document.getElementById("txtDeuda").value;
        const cambioDelCliente = document.getElementById("txtCambio").value;

        const Descuento = document.getElementById("txtDescuento").value;

        const detalle = {
            montoAPagar: montoAPagar,
            pagoDelCliente: pagoDelCliente,
            deudaDelCliente: deudaDelCliente,
            cambioDelCliente: cambioDelCliente
        };
        detallePagos.push(detalle);

        if (Descuento == "") {
            toastr.warning("", "Debes Ingresar Descuento")
            return;
        }
        if (pagoDelCliente == "") {
            toastr.warning("", "Debes Ingresar Pago")
            return;
        }
    


        //if (detallePagos.length < 1) {
        //    toastr.warning("", "Debes Ingresar Productos")
        //    return;
        //}

        const vmDetallePago = detallePagos;

        const pago = {
            idPago: $("#txtIdPago").val(),
            idPedido: $("#txtIdPedido").val(),
            montoDePedido: $("#txtMontoPedido").val(),
            nombreCliente: $("#txtNombres").val(),
            descuento: $("#txtDescuento").val(),
            montoTotalDePago: $("#txtMontoPago").val(),
            montoDeuda: $("#txtDeuda").val(),
            estado: $("#txtEstado").val(),
            montoAPagar: $("#txtPagoAPagar").val(),
            DetallePago: vmDetallePago

        }

        const modelo = structuredClone(MODELO_BASE);
        modelo["idPago"] = parseInt($("#txtIdPago").val())
        modelo["idPedido"] = parseInt($("#txIdPedido").val())
        modelo["montoDePedido"] = $("#txtMontoPedido").val()
        modelo["montoTotalDePago"] = $("#txtMontoPago").val()
        modelo["nombreCliente"] = $("#txtNombres").val()
        modelo["fechaPago"] = $("#txtFechaPago").val()
        modelo["fechaPedido"] = $("#txtFechaPedido").val()
        modelo["descuento"] = $("#txtDescuento").val()
        modelo["montoDeuda"] = $("#txtDeuda").val()
        modelo["estado"] = $("#txtEstado").val()
    

    

  /*  $("#btnGuardarPago").LoadingOverlay("show");*/
    $("#modalDataPago").find("div.modal-content").LoadingOverlay("show");
    
    if (modelo.idPago == 0) {
       
        fetch("/Pago/Guardar", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(pago)
        })
            .then(response => {
                $("#modalDataPago").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    detallePagos = [];
                    /*$("#txtDocumentoCliente").val("")*/
                    tablaDataPago.row.add(responseJson.objeto).draw(false)
                    $("#modalDataPago").modal("hide");
                    swal("Pago Registrado", `Codigo de Pago:${responseJson.objeto.idPedido}`, "success")
                }
                else {
                    swal("Lo sentimos", "No se pudo Registrar el pago", "error")

                }
            })




    }
    else {
        fetch("/Pago/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(pago)
        })
            .then(response => {
                $("#modalDataPago").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    if (Devolucion != null) {
                        RegistrarDevoluciones();
                    }
                    tablaDataPago.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataPago").modal("hide")
                    swal("Listo", "el pago fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })







    }
})


function RegistrarDevoluciones() {

    fetch("/Devolucion/Crear", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(Devolucion)
    })
        .then(response => {
            /*    $("#btnGuardarDescuento").LoadingOverlay("hide");*/
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                Devolucion = [];
                /*  $("#txtDocumentoCliente").val("")*/
                swal("Registrado", `Codigo de Producto:${responseJson.objeto.codigoPedido}`, "success")
            }
            else {
                swal("Lo sentimos", "No se pudo Registrar la Devolucion ", "error")

            }
        }) 

}


let filaSeleccionada;

$("#tbdataPago tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataPago.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdataPago tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataPago.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al usuario "${data.idPago}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si,eliminar",
        cancelButtonText: "No,cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {

            if (respuesta) {

                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/Pago/Eliminar?IdPago=${data.idPago}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataPago.row(fila).remove().draw();
                            swal("Listo", "el pago fue eliminado", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})