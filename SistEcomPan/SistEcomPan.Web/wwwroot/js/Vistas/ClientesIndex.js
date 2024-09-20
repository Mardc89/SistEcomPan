

const MODELO_BASE = {
    idCliente: 0,
    tipoCliente:"",
    dni: "",
    nombres: "",
    apellidos:"",
    nombreCompleto:"",
    correo: "",
    nombreUsuario: "",
    clave: "",
    direccion: "",
    telefono:"",
    urlFoto: "",
    idDistrito:0,
    estado: 1,
    nombreFoto: "",
    nombreDistrito:""

}

let tablaDataCliente;
const itemPagina = 4;
$(document).ready(function () {

    fetch("/Cliente/ListaDistritos")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboDistrito").append(
                        $("<option>").val(item.idDistrito).text(item.nombreDistrito)
                    )
                })

            }

        })

    tablaDataCliente = $('#tbdataCliente').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Cliente/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCliente", "searchable": false },
            {
                "data": "nombreFoto", render: function (data) {
                    let ruta = data;
                    let nombreCarpeta = /ImagenesPerfil/;
                    let rutaRelativa = `${nombreCarpeta}${ruta}`;
                    if (data == "" || data == null)
                        rutaRelativa = '/ImagenDefault/ImgUser.png';
                    return `<img style="height:60px" src=${rutaRelativa} class="rounded mx-auto d-block"/>`;
                }

            },
            { "data": "dni" },
            { "data": "apellidos" },
            { "data": "nombres" },
            { "data": "correo" },
            { "data": "nombreUsuario" },

            {
                "data": "estado", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">No Activo</span>';


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
        responsive: true,
        paging: true,
        pageLength: itemPagina,
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },
    });
})


function mostrarModal(modelo = MODELO_BASE) {
    const rutaBase = '/ImagenesPerfil/';
    //let rutaCompleta = modelo.urlFoto;
    //let rutaRelativa = rutaCompleta.replace('C:\\Proyects\\SistEcomPan\\SistEcomPan\\SistEcomPan.Web\\wwwroot\\Imagenes\\', '/Imagenes/');
    let rutaRelativa = rutaBase + modelo.nombreFoto;
    if (modelo.nombreFoto == "") {
        rutaRelativa = '/ImagenDefault/ImgUser.png';
    }
    $("#txtIdCliente").val(modelo.idCliente)
    $("#txtDni").val(modelo.dni)
    $("#txtNombres").val(modelo.nombres)
    $("#txtApellidos").val(modelo.apellidos)
    $("#txtNombreUsuario").val(modelo.nombreUsuario)
    $("#txtCorreo").val(modelo.correo)
    $("#txtTipoCliente").val(modelo.tipoCliente)
    $("#txtTelefono").val(modelo.telefono)
    $("#txtDireccion").val(modelo.direccion)
    $("#txtClave").val(modelo.clave)
    $("#cboEstado").val(modelo.estado)
    $("#cboDistrito").val(modelo.idDistrito == 0 ? $("#cboRol option:first").val() : modelo.idDistrito)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", rutaRelativa)

    $("#modalDataCliente").modal("show")
}


$("#btnNuevoCliente").click(function () {
    mostrarModal()
})


$("#btnGuardarCliente").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idCliente"] = parseInt($("#txtIdCliente").val())
    modelo["dni"] = $("#txtDni").val()
    modelo["nombres"] = $("#txtNombres").val()
    modelo["apellidos"] = $("#txtApellidos").val()
    modelo["nombreUsuario"] = $("#txtNombreUsuario").val()
    modelo["correo"] = $("#txtCorreo").val()
    modelo["clave"] = $("#txtClave").val()
    modelo["telefono"] = $("#txtTelefono").val()
    modelo["direccion"] = $("#txtDireccion").val()
    modelo["tipoCliente"] = $("#cboTipoCliente").val()
    modelo["idDistrito"] = $("#cboDistrito").val()
    modelo["estado"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtFoto")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalDataCliente").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCliente == 0) {

        fetch("/Cliente/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalDataCliente").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataCliente.row.add(responseJson.objeto).draw(false)
                    $("#modalDataCliente").modal("hide")
                    swal("Listo", "el usuario fue creado", "success")
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
    else {

        fetch("/Cliente/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalDataCliente").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataCliente.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataCliente").modal("hide")
                  
                    swal("Listo", "el usuario fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })





    }
})

let filaSeleccionada;

$("#tbdataCliente tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataCliente.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdataCliente tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataCliente.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al cliente "${data.nombres}"`,
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
                fetch(`/Cliente/Eliminar?IdCliente=${data.idCliente}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataCliente.row(fila).remove().draw();
                            swal("Listo", "el usuario fue eliminado", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})