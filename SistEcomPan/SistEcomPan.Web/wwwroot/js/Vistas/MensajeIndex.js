
const RemitenteMensaje = {
    idMensaje: 0,
    asunto: "",
    cuerpo: "",
    nombreRemitente: "",
    nombreDestinatario: "",
    idRespuestaMensaje:0,
    correoRemitente:""
}

const DestinatarioMensaje = {
    idMensaje:0,
    correoDestinatario: ""
}

let tablaDataMensaje;
const itemPaginaMensaje = 4;

function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();

    const fechaFormateada = `${dia}/${mes}/${año}`;

    return fechaFormateada;

}


let IdRespuesta,NombreDelDestinatario,nombresDelUsuario;

$(document).ready(function () {

    tablaDataMensaje = $('#tbdataMensaje').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Mensaje/ListaMensajes',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idMensaje", "searchable": false },
            { "data": "asunto"},
            { "data": "cuerpo" },
            { "data": "nombreRemitente" },
            {
                "data": "nombreDestinatario", render: function (data) {
                    if (data!=null) 
                        NombreDelDestinatario = data;
                        return NombreDelDestinatario;
                }
            },
            { "data": "correoRemitente", "visible": false },
            { "data": "correoDestinatario", "visible": false },
            { "data": "idRespuestaMensaje", "visible": false },

            //{
            //    "data": "IdRespuesta", render: function (data) {
            //        if (data == NombreDelDestinatario) {

            //            return NombreDelDestinatario;
            //        }
            //        else {

            //            return NombreDelDestinatario;

            //        }


            //    }
            //},
            {
                "data": "fechaDeMensaje", render: function (data) {
                    return cambiarFecha(data);
                }
            },
            {
                "defaultContent":
                    '<div class="d-flex justify-content-start">' +
                    '<button class="btn btn-success btn-sm btnResponder"><i class="fas fa-reply"></i></button>' +
                    '<button class="btn btn-primary btn-editar btn-sm mx-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>' +
                    '</div>',

                "orderable": false,
                "searchable": false,
                "width": "80px"

            }
        ],
        "createdRow": function (row, data, dataIndex) {
            if (data.nombreDestinatario !== nombresDelUsuario) {
                debugger;
                $(row).find(".btnResponder").prop("disabled", true);
            }

        },
        order: [[0, "desc"]],
        responsive: true,
        paging: true,
        pageLength: itemPaginaMensaje,
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },
    });
})


function mostrarModal(remitenteMensaje = RemitenteMensaje, destinatarioMensaje = DestinatarioMensaje) {

    $("#txtIdMensaje").val(remitenteMensaje.idMensaje)
    $("#txtAsunto").val(remitenteMensaje.asunto)
    $("#txtCuerpo").val(remitenteMensaje.cuerpo)
    $("#txtCorreoRemitente").val(remitenteMensaje.correoRemitente)
    $("#txtCorreoDestinatario").val(remitenteMensaje.correoDestinatario)

    $("#modalDataMensaje").modal("show")
}


$("#btnNuevoMensaje").click(function () {
    mostrarModal()
})


//let RolUsuario = "",dniDelCliente="";

if (userRol === "Administrador") {
    nombresDelUsuario = document.getElementById("NombreCompleto").textContent
    //if (NombreDelDestinatario === nombres && IdRespuesta==null) {
    //    document.getElementById("btnResponder").disabled = false;
    //}

}
else if (userRol === "Cliente") {

    nombresDelUsuario = document.getElementById("NombreCompleto").textContent
    //if (NombreDelDestinatario === nombres && IdRespuesta == null) {
    //    document.getElementById("btnResponder").disabled = false;
    //}
    
}


$("#btnGuardarMensaje").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }


    debugger;
    const remitenteMensaje = structuredClone(RemitenteMensaje);
    remitenteMensaje["idMensaje"] = parseInt($("#txtIdMensaje").val())
    remitenteMensaje["asunto"] = $("#txtAsunto").val()
    remitenteMensaje["cuerpo"] = $("#txtCuerpo").val()
    remitenteMensaje["correoRemitente"] = $("#txtCorreoRemitente").val()
    remitenteMensaje["idRespuestaMensaje"] = null

    const destinatarioMensaje = structuredClone(DestinatarioMensaje);
    destinatarioMensaje["idMensaje"] = parseInt($("#txtIdMensaje").val())
    destinatarioMensaje["correoDestinatario"] = $("#txtCorreoDestinatario").val()

    //modelo["remitente"] = RolUsuario
    //modelo["destinatario"] = RolUsuario
    //modelo["idRemitente"]=dniDelCliente

    const modelo = { remitenteMensaje,destinatarioMensaje };


    $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("show");

    if (remitenteMensaje.idMensaje == 0) {

        fetch("/Mensaje/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataMensaje.row.add(responseJson.objeto).draw(false)
                    $("#modalDataMensaje").modal("hide")
                    swal("Listo", "el mensaje fue creado", "success")
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
    else {

        fetch("/Mensaje/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(datosDelMensaje)
        })
            .then(response => {
                $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataDistrito.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataMensaje").modal("hide")
                    swal("Listo", "el producto fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })





    }
})

let filaSeleccionada;

$("#tbdataDistrito tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataDistrito.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdataDistrito tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataDistrito.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar  "${data.tipoDeCategoria}"`,
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
                fetch(`/Distrito/Eliminar?IdDistrito=${data.idDistrito}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataDistrito.row(fila).remove().draw();
                            swal("Listo", "el producto fue eliminado", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})