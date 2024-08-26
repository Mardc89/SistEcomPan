

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

let nombreDelUsuario;

$(document).ready(function () {
    debugger;
    let busquedaMensaje = "";
    let busquedaDetalleMensaje = document.getElementById("DniPersonal").textContent;
    tablaDataMisMensajes = $('#tbDataMisMensajes').DataTable({
        responsive: true,
        "ajax": {
            "url": `/Mensaje/ObtenerMisMensajes?searchTerm=${busquedaDetalleMensaje}&busqueda=${busquedaMensaje}`,
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "idMensaje", "searchable": false },
            { "data": "asunto" },
            { "data": "cuerpo" },
            { "data": "nombreRemitente" },
            { "data": "nombreDestinatario", },
            { "data": "correoRemitente","visible": false },
            { "data": "correoDestinatario", "visible": false },
            { "data": "idRespuestaMensaje", "visible": false },
            {
                "data": "fechaDeMensaje", render: function (data) {
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
                "width": "80px"

            }
        ],
        "createdRow": function (row, data, dataIndex) {

            if (data.nombreDestinatario !== nombreDelUsuario) {

                $(row).find(".btnResponder").prop("disabled", true);
            }

        },
        order: [[0, "desc"]],
        paging: true,
        pageLength: itemPagMisMensajes,

    });
})


const ProductosPorPag = 4; // Cantidad de productos por página
let PaginaInicialPago = 1; // Página actual al cargar


function buscarDetallePago(idPago, page = 1) {
    fetch(`/Pago/ObtenerMiDetallePago?idPago=${idPago}&page=${page}&itemsPerPage=${ProductosPorPag}`)
        .then(response => response.json())
        .then(data => {
            const DetallePagos = data.detallePago; // Array de productos obtenidos
            const totalItems = data.totalItems; // Total de productos encontrados
            // Actualizar la tabla modal con los productos obtenidos 
            const productTable = document.getElementById('DetallePagoBuscado');
            productTable.innerHTML = '';

            DetallePagos.forEach(pago => {
                const row = document.createElement('tr');
                row.innerHTML = `  
            <td>${pago.idPago}</td>
            <td>${pago.idDetallePago}</td>
            <td>${pago.montoAPagar}</td>
            <td>${pago.pagoDelCliente}</td>
            <td>${pago.deudaDelCliente}</td>
            <td>${pago.cambioDelCliente}</td>
          `;
                productTable.appendChild(row);
            });

            // Generar la paginación
            const totalPages = Math.ceil(totalItems / ProductosPorPag);
            const pagination = document.getElementById('DetallePagoPagination');
            pagination.innerHTML = '';

            for (let i = 1; i <= totalPages; i++) {
                const li = document.createElement('li');
                li.classList.add('page-item');
                const link = document.createElement('a');
                link.classList.add('page-link');
                link.href = '#';
                link.textContent = i;
                li.appendChild(link);

                if (i === PaginaInicialPago) {
                    li.classList.add('active');
                }

                link.addEventListener('click', () => {
                    PaginaInicialPago = i;
                    buscarDetallePago(idPago, PaginaInicialPago);
                    resaltarPaginaPagoActual();
                });

                pagination.appendChild(li);
            }

            resaltarPaginaPagoActual();
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
        });
}


// Función para resaltar la página actual
function resaltarPaginaPagoActual() {
    const paginationItems = document.querySelectorAll('#DetallePagoPagination .page-item');
    paginationItems.forEach(item => {
        item.classList.remove('active');
        const link = item.querySelector('.page-link');
        if (link.textContent === PaginaInicialPago.toString()) {
            item.classList.add('active');
        }
    });
}



function mostrarModalDetallePago(modelo = MODELO_BASE) {

    let idPagos = modelo.idPago;
    buscarDetallePago(idPagos)

    $("#modalDataMiDetallePago").modal("show")
}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
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

//document.getElementById("txtResptCuerpo").addEventListener("input", function (event) {
//    let textoDelEscritor = event.target.value;
//    alert(textoDelEscritor);
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


    debugger;
    const remitenteMensaje = structuredClone(RemitenteMensaje);
    remitenteMensaje["idMensaje"] = parseInt($("#txtResptIdMensaje").val())
    remitenteMensaje["asunto"] = "Respuesta:" + " " + $("#txtResptAsunto").val()
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


    $("#modalDataMensaje").find("div.modal-content").LoadingOverlay("show");

    if (remitenteMensaje.idMensaje!= 0) {

        fetch("/Mensaje/EnviarMensajeRespuesta", {
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
                    swal("Listo", "el mensaje fue enviado", "success")
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }

})


function mostrarModalRespuesta(remitenteMensaje = RemitenteMensaje,destinatarioMensaje = DestinatarioMensaje) {
    fetch(`/Mensaje/ObtenerMensajeDeAsunto?asunto=${remitenteMensaje.asunto}`)
        .then(response => response.json())
        .then(data => {
            const mensajes = data.data;
            const area = document.getElementById('DetalleAsuntoMensaje');
            area.innerHTML = '';

            mensajes.forEach(mensaje => {
                const nombreRemitente = mensaje.nombreRemitente;
                const fecha = cambiarFecha(mensaje.fechaDeMensaje);
                const mensajeElemento = document.createElement('p');
                mensajeElemento.innerHTML = `<strong>${nombreRemitente}</strong><span style="float:right;">${fecha}</span><br>${mensaje.cuerpo}`;
/*                area.value += `${mensaje.nombreRemitente}\t\t\t\t ${cambiarFecha(mensaje.fechaDeMensaje)}\n ${mensaje.cuerpo}\n`;*/
/*                area.value += `${mensaje.cuerpo}\n`;*/
                /*   area.value += `Destinatario:${mensaje.nombreDestinatario}\n`;*/
                area.appendChild(mensajeElemento);
            });

        })
        .catch(error => {
            console.error('Error al buscar mensajes:', error);        });



    $("#txtResptIdMensaje").val(remitenteMensaje.idMensaje)
    $("#txtResptAsunto").val(remitenteMensaje.asunto)
    $("#txtResptCorreoRemitente").val(remitenteMensaje.correoRemitente)
    $("#txtResptCorreoDestinatario").val(remitenteMensaje.correoDestinatario)
    $("#modalDataMensajeRespuesta").modal("show")
}

let filaSeleccionada;
$("#tbDataMisMensajes tbody").on("click", ".btnResponder", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataMisMensajes.row(filaSeleccionada).data();
    mostrarModalRespuesta(data);
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
    const data = tablaDataProducto.row(fila).data();

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
                fetch(`/Mensaje/Eliminar?IdMensaje=${data.idMensaje}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataProducto.row(fila).remove().draw();
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