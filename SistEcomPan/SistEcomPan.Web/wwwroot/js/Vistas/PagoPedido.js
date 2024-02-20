
const MODELO_BASE = {
    idUsuario: 0,
    dni: "",
    nombres: "",
    apellidos: "",
    correo: "",
    nombreUsuario: "",
    clave: "",
    idRol: 0,
    urlFoto: "",
    esActivo: 1,
    nombreRol: "",

}


$(document).ready(function () {

    //fetch("/Usuario/ListaRoles")
    //    .then(response => {
    //        return response.ok ? response.json() : Promise.reject(response);
    //    })
    //    .then(responseJson => {
    //        if (responseJson.length > 0) {
    //            responseJson.forEach((item) => {
    //                $("#cboRol").append(
    //                    $("<option>").val(item.idRol).text(item.nombreRol)
    //                )
    //            })
    //        }
    //    })

    tablaData = $('#tbdataPedido').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Pago/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPedido", "searchable": false },
            { "data": "codigo" },
            { "data": "dni" },
            { "data": "nombresCompletos" },
            { "data": "montoTotal" },
            { "data": "estado" },
            { "data": "fechaPedido" },
            /*            { "data": "nombreRol" },*/
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"

            }
        ],


    });
})


function mostrarModal(modelo = MODELO_BASE) {
    debugger;
    let rutaCompleta = modelo.urlFoto;
    let rutaRelativa = rutaCompleta.replace('C:\\Proyects\\SistEcomPan\\SistEcomPan\\SistEcomPan.Web\\wwwroot\\Imagenes\\', 'Imagenes/');
    $("#txtId").val(modelo.idUsuario)
    $("#txtDni").val(modelo.dni)
    $("#txtNombres").val(modelo.nombres)
    $("#txtApellidos").val(modelo.apellidos)
    $("#txtNombreUsuario").val(modelo.nombreUsuario)
    $("#txtCorreo").val(modelo.correo)
    $("#txtClave").val(modelo.clave)
    $("#cboRol").val(modelo.idRol == 0 ? $("#cboRol option:first").val() : modelo.idRol)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", rutaRelativa)

    $("#modalDataPago").modal("show")
}


$("#btnNuevoPago").click(function () {
    mostrarModal()
})