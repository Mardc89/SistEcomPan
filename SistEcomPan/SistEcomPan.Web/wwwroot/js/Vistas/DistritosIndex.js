

const MODELO_BASE = {
    idDistrito: 0,
    nombreDistrito: "",
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      

}

let tablaDataDistrito;
const itemPaginaDistrito = 4;

$(document).ready(function () {

    tablaDataDistrito = $('#tbdataDistrito').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Distrito/ListaDistritos',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idDistrito", "searchable": false },
            { "data": "nombreDistrito" },
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
        pageLength: itemPaginaDistrito,
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },
    });
})


function mostrarModal(modelo = MODELO_BASE) {

    $("#txtIdDistrito").val(modelo.idDistrito)
    $("#txtNombreDistrito").val(modelo.nombreDistrito)

    $("#modalDataDistrito").modal("show")
}


$("#btnNuevoDistrito").click(function () {
    mostrarModal()
})


$("#btnGuardarDistrito").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idDistrito"] = parseInt($("#txtIdDistrito").val())
    modelo["nombreDistrito"] = $("#txtNombreDistrito").val()
 


    $("#modalDataDistrito").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idDistrito == 0) {

        fetch("/Distrito/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalDataDistrito").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataDistrito.row.add(responseJson.objeto).draw(false)
                    $("#modalDataDistrito").modal("hide")
                    swal("Listo", "el producto fue creado", "success")
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
    else {

        fetch("/Distrito/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalDataDistrito").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataDistrito.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataDistrito").modal("hide")
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