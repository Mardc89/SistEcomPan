

const VISTA_BUSQUEDA = {

    busquedaFecha: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtCodigo").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()

    }, busquedaVenta: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtCodigo").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }
}

$(document).ready(function () {

    VISTA_BUSQUEDA["busquedaFecha"]()
    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })


})

$("#cboBuscarPor").change(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    }
    else {
        VISTA_BUSQUEDA["busquedaVenta"]()

    }
})


$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debes Ingresar Fecha Inicio y Fin")
            return;
        }

    }
    else {
        if ($("#txtCodigo").val().trim() == "") {
            toastr.warning("", "Debes Ingresar Codigo de Pedido")
            return;
        }

    }

    let codigo = $("#txtCodigo").val()
    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()


    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Pedido/Historial?codigo=${codigo}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
        .then(response => {
            $(".card-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            $("#tbPedido tbody").html("");

            if (responseJson.length > 0) {

                responseJson.forEach((pedido) => {

                    $("#tbPedido tbody").append(
                        $("<tr>").append(                       
                            $("<td>").text(pedido.codigo),
                            $("<td>").text(pedido.dni),
                            $("<td>").text(pedido.nombresCompletos),
                            $("<td>").text(pedido.montoTotal),
                            $("<td>").text(pedido.fechaPedido),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-eye")
                                ).data("pedido", pedido)
                            )
                        )
                    )

                })
            }

        })
})



$("#tbPedido tbody").on("click", ".btn-info", function () {

    let d = $(this).data("pedido")

    $("#txtFechaRegistro").val(d.fechaRegistro)
    $("#txtCodigo").val(d.codigo)
    $("#txtUsuarioRegistro").val(d.usuario)
    $("#txtDocumentoCliente").val(d.documentoCliente)
    $("#txtNombreCliente").val(d.nombreCliente)
    $("#txtTotal").val(d.total)

    $("#tbProductos tbody").html("");

    d.detallePedido.forEach((item) => {

        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )

    })

    $("#linkImprimir").attr("href", `/Pedido/MostrarPDFVenta?codigo=${d.codigo}`)

    $("#modalData").modal("show");



})