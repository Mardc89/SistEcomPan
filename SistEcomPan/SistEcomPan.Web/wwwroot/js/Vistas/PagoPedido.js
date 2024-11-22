
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
        responsive: true,
        "ajax": {
            "url": '/Pago/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPago", "searchable": false },
            { "data": "montoDePedido" },
            { "data": "descuento" },
            { "data": "montoTotalDePago" },
            { "data": "montoDeuda" },
            { "data": "nombreCliente" },
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
                "width": "80px"

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

function VerificarEstado() {
    debugger;
    let idPago = document.getElementById("txtIdPago").value;
    let estado = document.getElementById("txtEstado").value;
    let btnEstado = document.getElementById("btnGuardarPago");
    let Opcion1 = document.getElementById("opcion1");
    let Opcion2 = document.getElementById("opcion2");
    if (estado == "Pagado" && idPago > 0) {
        btnEstado.disabled = true;
    }
    else {
        btnEstado.disabled = false;
    }

    if (estado == "Pagado" || estado == "Existe Deuda" || estado=="") {
        Opcion1.disabled = true;
        Opcion2.disabled = true;
    }
    else {
        Opcion1.disabled = false;
        Opcion2.disabled = false;
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
    $("#txtDescuento").val(modelo.descuento)
    $("#txtDeuda").val(modelo.montoDeuda)
    $("#txtMontoPago").val(modelo.montoDeuda)
    $("#txtPagoCliente").val("0.00")
    $("#txtCambio").val("0.00")
    $("#txtEstado").val(modelo.estado)
    $("#txtCodigoPedido").val(modelo.codigoPedido)


    $("#modalDataPago").modal("show")
    DesacticarCamposPago();
    VerificarEstado();
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

}


    $("#btnNuevoPago").click(function () {
    mostrarModal()
    })

$("#btnClosePago").click(function () {
    $("#txtPagoDelCliente").val("")
})

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

        const detalle = {
            montoAPagar: montoAPagar,
            pagoDelCliente: pagoDelCliente,
            deudaDelCliente: deudaDelCliente,
            cambioDelCliente: cambioDelCliente
        };
        detallePagos.push(detalle);
    


    if (detallePagos.length < 1) {
        toastr.warning("", "Debes Ingresar Productos")
        return;
    }

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