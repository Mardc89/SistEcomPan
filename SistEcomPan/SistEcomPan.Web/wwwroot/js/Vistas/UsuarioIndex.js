﻿

const MODELO_BASE={
    idUsuario: 0,
    dni:"",
    nombres: "",
    apellidos:"",
    correo: "",
    nombreUsuario: "",
    clave:"",
    idRol: 0,
    urlFoto: "",
    esActivo: 1,
    nombreRol: "",
    nombreFoto:"",
   
}

let tablaData;
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

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Usuario/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idUsuario","searchable": false},
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
            { "data": "dni"},
            { "data": "nombres" },
            { "data": "apellidos" },
            { "data": "correo" },
            { "data": "nombreUsuario" },
            { "data": "nombreRol" },
            {
                "data": "esActivo", render: function (data) {
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
    ObtenerDatosUsuario();

    $("#modalData").modal("show")
}


$("#btnNuevo").click(function () {
    mostrarModal()
})

function validarDni() {
    document.getElementById("txtDni").addEventListener("input", function (e) {
        e.target.value = e.target.value.replace(/\D/g, '');

    });
}


$("#btnGuardar").click(function () {
    debugger;
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }
   

    const modelo = structuredClone(MODELO_BASE);
    modelo["idUsuario"] = parseInt($("#txtId").val())
    modelo["dni"] = $("#txtDni").val()
    modelo["nombres"] = $("#txtNombres").val()
    modelo["apellidos"] = $("#txtApellidos").val()
    modelo["nombreUsuario"] = $("#txtNombreUsuario").val()
    modelo["correo"] = $("#txtCorreo").val()
    modelo["clave"] = $("#txtClave").val()
    modelo["idRol"] = $("#cboRol").val()
    modelo["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtFoto")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idUsuario == 0) {

        fetch("/Usuario/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo", "el usuario fue creado", "success");
                  
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
    else {
        
        fetch("/Usuario/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    const datos = tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    let rutaRelativa = datos.nombreFoto;
                    $("#imgUsuario").attr("src", rutaRelativa);
                    filaSeleccionada = null;
                   
                    $("#modalData").modal("hide")
                   
                    swal("Listo", "el usuario fue modificado", "success");
                 

                }else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })





    }
})

let filaSeleccionada;

$("#tbdata tbody").on("click", ".btn-editar", function () {
   
    if ($(this).closest("tr").hasClass("child")){
    filaSeleccionada = $(this).closest("tr").prev();
    }else {
    filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al usuario "${data.nombre}"`,
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
                fetch(`/Usuario/Eliminar?IdUsuario=${data.idUsuario}`, {
                    method: "DELETE",
                    
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaData.row(fila).remove().draw();
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