﻿
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


let IdRespuesta,NombreDelDestinatario,nombresDelUsuario;

$(document).ready(function () {
    ObtenerDatosUsuario();
    tablaDataMensaje = $('#tbDataMensajes').DataTable({
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
            if (data.nombreRemitente !== nombresDelUsuario) {
                debugger;
                $(row).find(".btn-editar").prop("disabled", true);
            }
            if (data.nombreRemitente !== nombresDelUsuario) {
                debugger;
                $(row).find(".btn-eliminar").prop("disabled", true);
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


$("#btnMensajeCliente").click(function () {
    mostrarMensajesClientes();
})


function mostrarMensajesClientes() {
    /*    obtenerFecha();*/
    buscarClientes();

    $("#modalDataMensajeCliente").modal("show");
}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById('ClienteMensaje').addEventListener('click', function (event) {


        if (event.target.tagName == 'TD') {

            const fila = event.target.parentNode;
            const correoDestino = fila.cells[3].textContent;

            document.getElementById('txtCorreoDestinatario').value = correoDestino;

            $("#modalDataMensajeCliente").modal("hide");
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
            const clientTable = document.getElementById('ClienteMensaje');
            clientTable.innerHTML = '';

            clientes.forEach(cliente => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${cliente.idCliente}</td>
            <td>${cliente.dni}</td>
            <td class="nombres">${cliente.nombreCompleto}</td>
            <td>${cliente.correo}</td>
            <td>${cliente.direccion}</td>
            <td>${cliente.telefono}</td>
          `;
                clientTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ClientesPorPagina);
            const pagination = document.getElementById('paginacionMensajeCliente');
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



let filaSeleccionada;

$("#tbDataMensajes tbody").on("click", ".btnResponder", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMensaje.row(filaSeleccionada).data();
    mostrarModalRespuesta(data);
});




function mostrarModal(remitenteMensaje = RemitenteMensaje, destinatarioMensaje = DestinatarioMensaje) {
    debugger
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
    destinatarioMensaje["correoDestinatario"] = $("#txtResptCorreoRemitente").val()


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
                        /*  actualizarMensajes(AsuntoRemitente);*/
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
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataMensaje.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataMensaje").modal("hide")
                    swal("Listo", "el mensaje fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })

    }
})



$("#tbDataMensajes tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMensaje.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbDataMensajes tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataMensaje.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar  "${data.asunto}"`,
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

                            tablaDataMensaje.row(fila).remove().draw();
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