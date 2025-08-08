
const RemitenteMensaje = {
    idMensaje: 0,
    asunto: "",
    cuerpo: "",
    nombreRemitente: "",
    nombreDestinatario: "",
    idRespuestaMensaje:0,
    correoRemitente: ""
}

const DestinatarioMensaje = {
    idMensaje: 0,
    correoDestinatario:""
}



let tablaDataMisMensajes;

const itemPagMisMensajes = 4; // Cantidad de productos por página



function cambiarFecha(fecha) {

    const fechaOriginal = new Date(fecha);
    const dia = fechaOriginal.getDate();
    const mes = fechaOriginal.getMonth() + 1;
    const año = fechaOriginal.getFullYear();
    const horas = fechaOriginal.getHours();
    const minutos = fechaOriginal.getMinutes();

    const fechaFormateada = `${dia}/${mes}/${año} ${horas}${minutos}`;

    return fechaFormateada;

}

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


let nombreDelUsuario, NombreDelDestinatario;

$(document).ready(function () {
    ObtenerDatosCliente();
    debugger;
    let busquedaMensaje = "";
    let busquedaDetalleMensaje = document.getElementById("DniPersonal").textContent;
    tablaDataMisMensajes = $('#tbDataMisMensajes').DataTable({
        responsive: {
            details: false
        },
        "ajax": {
            "url": `/Mensaje/ObtenerMisMensajes?searchTerm=${busquedaDetalleMensaje}&busqueda=${busquedaMensaje}`,
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "idMensaje", "searchable": false, responsivePriority: 100 },
            { "data": "asunto", responsivePriority: 100 },
            { "data": "cuerpo", responsivePriority: 100 },
            { "data": "nombreRemitente", responsivePriority: 1 },
            {
                "data": "nombreDestinatario", render: function (data) {
                    if (data != null)
                        NombreDelDestinatario = data;
                    return NombreDelDestinatario;
                }
            },
            { "data": "correoRemitente", "visible": false, responsivePriority: 100 },
            { "data": "correoDestinatario", "visible": false, responsivePriority: 100 },
            { "data": "idRespuestaMensaje", "visible": false, responsivePriority: 100 },
            {
                "data": "fechaDeMensaje", responsivePriority: 100 , render: function (data) {
                    return cambiarFecha(data);
                }
            },
            {
                "defaultContent":
                '<div class="d-flex justify-content-start">'+
                    '<button class= "btn btn-success btn-sm btnResponder"><i class= "fas fa-reply"></i></button>' +
                    '<button class="btn btn-primary btn-editar btn-sm mx-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class= "btn btn-danger btn-eliminar btn-sm"><i class= "fas fa-trash-alt"></i></button>' +
                    '</div>',

                "orderable": false,
                "searchable": true,
                "width": "80px",
                responsivePriority: 1
            }
        ],
        "createdRow": function (row, data, dataIndex) {

            if (data.nombreDestinatario !== nombreDelUsuario) {

                $(row).find(".btnResponder").prop("disabled", true);
            }
            if (data.nombreRemitente !== nombreDelUsuario) {

                $(row).find(".btn-editar").prop("disabled", true);
            }
            if (data.nombreRemitente !== nombreDelUsuario) {

                $(row).find(".btn-eliminar").prop("disabled", true);
            }

        },
        order: [[0, "desc"]],
        paging: true,
        pageLength: itemPagMisMensajes,
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },

    });
})


let socket;

function connectWebSocket() {
    socket = new WebSocket("wss://localhost:7078/ws");

    socket.onmessage = function (event) {
        const message = JSON.parse(event.data);
        actualizarMensajes(message.remitenteMensaje.asunto);
    };

    socket.onclose = function (event) {
        console.log("WebSocket is closed now.");
        // Reconectar si es necesario
        setTimeout(connectWebSocket, 1000);
    };

    socket.onerror = function (error) {
        console.error("WebSocket error:", error);
    };
}

connectWebSocket();



/*let intervaloActualizacion;*/
function actualizarMensajes(asunto) {
    fetch(`/Mensaje/ObtenerMensajeDeAsunto?asunto=${asunto}`)
        .then(response => response.json())
        .then(data => {
            const mensajes = data.data;
            const area = document.getElementById('DetalleAsuntoMensaje');
            area.innerHTML = "";
            mensajes.forEach(mensaje => {
                const nombreRemitente = mensaje.nombreRemitente;
                const fecha = cambiarFecha(mensaje.fechaDeMensaje);

               
               const mensajeElemento = document.createElement('p');
               mensajeElemento.innerHTML = `<strong>${nombreRemitente}</strong><span style="float:right;">${fecha}</span><br>${mensaje.cuerpo}`;
               area.appendChild(mensajeElemento);
                
            });
        })
        .catch(error => {
            console.error('Error al buscar mensajes:', error);
        });
}

let AsuntoRemitente = "";


function mostrarModalRespuesta(remitenteMensaje = RemitenteMensaje) {
    // Llamada inicial para cargar los mensajes
    AsuntoRemitente = remitenteMensaje.asunto,
    actualizarMensajes(AsuntoRemitente);

    // Iniciar el intervalo de actualización
   /* iniciarActualizacion(AsuntoRemitente);*/

    $("#txtResptIdMensaje").val(remitenteMensaje.idMensaje);
    $("#txtResptAsunto").val(remitenteMensaje.asunto);
    $("#txtResptCorreoRemitente").val(remitenteMensaje.correoRemitente);
    $("#txtResptCorreoDestinatario").val(remitenteMensaje.correoDestinatario);
    $("#modalDataMensajeRespuesta").modal("show");
}

// Detener el intervalo cuando el modal se cierra
//$('#modalDataMensajeRespuesta').on('hidden.bs.modal', function () {
//    clearInterval(intervaloActualizacion);
//});

let filaSeleccionada;

$("#tbDataMisMensajes tbody").on("click", ".btnResponder", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMisMensajes.row(filaSeleccionada).data();
    mostrarModalRespuesta(data);
});


//$("#tbDataMisMensajes tbody").on("click", ".btnResponder", function () {
//    if ($(this).closest("tr").hasClass("child")) {
//        filaSeleccionada = $(this).closest("tr").prev();
//    } else {
//        filaSeleccionada = $(this).closest("tr");
//    }
//    const data = tablaDataMisMensajes.row(filaSeleccionada).data();
//    mostrarModalRespuesta(data);
//})







function mostrarModal(remitenteMensaje = RemitenteMensaje, destinatarioMensaje = DestinatarioMensaje) {

    let correo = document.getElementById("CorreoPersonal").textContent;

    let correoDestino = remitenteMensaje.correoDestinatario;

    if (correoDestino == "" || correo == null || correoDestino === undefined) {
        $("#txtCorreoRemitente").val(correo);
    }
    else {
        $("#txtCorreoRemitente").val(remitenteMensaje.correoRemitente)
    }

    $("#txtIdMensaje").val(remitenteMensaje.idMensaje)
    $("#txtAsunto").val(remitenteMensaje.asunto)
    $("#txtCuerpo").val(remitenteMensaje.cuerpo)
/*    $("#txtCorreoRemitente").val(remitenteMensaje.correoRemitente)*/
    $("#txtCorreoDestinatario").val(remitenteMensaje.correoDestinatario)
    $("#modalDataMensaje").modal("show")
}


$("#btnNuevoMensaje").click(function () {
    mostrarModal()
})




//document.getElementById("txtMensajeRespuesta").addEventListener("input", function (event) {
//    debugger;
//    let textoDelEscritor = event.target.innerText;
//    clearTimeout(intervaloActualizacion);
//    intervaloActualizacion = setTimeout(() => {
//        if (textoDelEscritor.length > 0) {
//            detenerActualizacion();
//        }else {
//            iniciarActualizacion(AsuntoRemitente);
//        }

//    }, 60000);

//});
//$("#btnRespuestaDelMensaje").click(function () {
//    alert("Que tal amigos");
//})


$("#btnRespuestaDelMensaje").click(function () {
  
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    let asunto = $("#txtResptAsunto").val();
    if (!asunto.startsWith("Respuesta:")) {
        asunto = "Respuesta:" + asunto;
    }


    debugger;
    const remitenteMensaje = structuredClone(RemitenteMensaje);
    remitenteMensaje["idMensaje"] = parseInt($("#txtResptIdMensaje").val())
    remitenteMensaje["asunto"] = asunto
    remitenteMensaje["cuerpo"] = $("#txtMensajeRespuesta").text()
    remitenteMensaje["correoRemitente"] = $("#txtResptCorreoDestinatario").val()
    remitenteMensaje["idRespuestaMensaje"] = remitenteMensaje["idMensaje"]; 

    const destinatarioMensaje = structuredClone(DestinatarioMensaje);
    destinatarioMensaje["idMensaje"] = parseInt($("#txtResptIdMensaje").val())
    destinatarioMensaje["correoDestinatario"] =  $("#txtResptCorreoRemitente").val()
       

    //modelo["remitente"] = RolUsuario
    //modelo["destinatario"] = RolUsuario
    //modelo["idRemitente"]=dniDelCliente

    const modelo = { remitenteMensaje, destinatarioMensaje };  


  /*  $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("show");*/
   /* detenerActualizacion();*/
    if (remitenteMensaje.idMensaje != 0) {
        
        try {
         
            fetch("/Mensaje/EnviarMensajeRespuesta", {
                method: "POST",
                headers: { "Content-Type": "application/json;charset=utf-8" },
                body: JSON.stringify(modelo)
            })
                .then(response => {
                    /*  $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("hide");*/
                 /*   alert("bien");*/
                    if (!response.ok) {
                        throw new Error("HTTP error,status=" + response.status);
                    }
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                     /*   tablaDataMisMensajes.row.add(responseJson.objeto).draw(false)*/
                        //$("#modalDataMensaje").modal("hide")
                        //swal("Listo", "el mensaje fue enviado", "success")
                        /* actualizarMensajes(AsuntoRemitente);*/
                        socket.send(JSON.stringify(modelo));
                    }
                    else {
                        swal("Lo sentimos", responseJson.mensaje, "error")
                    }
                }).catch(error => {

                    alert("Error en la solicitud:" + error);
                    console.error("Error en la solicitud", error);
                })
        } catch (error) {

            alert("Error en la solicitud:" + error);
            console.error("Error en la solicitud", error);
        }

    }

    $("#txtMensajeRespuesta").text("");
    /*  iniciarActualizacion(AsuntoRemitente);*/
  

})




if (userRol === "Administrador") {
    nombreDelUsuario = document.getElementById("NombreCompleto").textContent
    //if (NombreDelDestinatario === nombres && IdRespuesta==null) {
    //    document.getElementById("btnResponder").disabled = false;
    //}

}
else if (userRol === "Cliente") {

    nombreDelUsuario = document.getElementById("NombreCompleto").textContent
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

    const modelo = { remitenteMensaje, destinatarioMensaje };


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
                    tablaDataMisMensajes.row.add(responseJson.objeto).draw(false)
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
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataMisMensajes.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataMensaje").modal("hide")
                    swal("Listo", "el mensaje fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })





    }
})



$("#tbDataMisMensajes tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMisMensajes.row(filaSeleccionada).data();
    mostrarModal(data);
})
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  

$("#tbDataMisMensajes tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataMisMensajes.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar al mensaje: "${data.asunto}"`,
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
                fetch(`/Mensaje/Eliminar?IdMensaje=${data.idMensaje}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataMisMensajes.row(fila).remove().draw();
                            swal("Listo", "el mensaje fue eliminado", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})