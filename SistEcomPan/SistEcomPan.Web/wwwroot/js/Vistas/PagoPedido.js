
    function mostrarModal() {
        $("#modalDataPago").modal("show")
    }


    $("#btnNuevoPago").click(function () {
    mostrarModal()
    })


    $("#btnGuardarPago").click(function () {
        
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
        idPedido: $("#txtIdPedido").val(),
        montoDePedido: $("#txtMontoPedido").val(),
        descuento: $("#txtDescuento").val(),
        montoTotalDePago: $("#txtMontoPago").val(),
        montoDeuda: $("#txtMontoDeuda").val(),
        estado: $("#txtEstado").val(),
        DetallePago: vmDetallePago

    }



    $("#btnGuardarPago").LoadingOverlay("show");
    debugger;
    fetch("/Pago/Guardar", {
        method: "POST",
        headers: { "Content-Type": "application/json;charset=utf-8" },
        body: JSON.stringify(pago)
    })
        .then(response => {
            $("#btnGuardarPago").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                detallePagos= [];
                /*$("#txtDocumentoCliente").val("")*/
                swal("Pago Registrado", `Codigo de Pago:${responseJson.objeto.idPedido}`, "success")
            }
            else {
                swal("Lo sentimos", "No se pudo Registrar el pago", "error")

            }
        })
})