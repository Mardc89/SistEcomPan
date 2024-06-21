


const MODELO_BASE = {
    idRol: 0,
    nombreRol: "",
    estado: 1

}

let tablaDataRol;
const itemPagina = 4;

$(document).ready(function () {
    tablaDataRol = $('#tbdataRol').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Rol/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idRol", "searchable": false },
            { "data": "nombreRol" },
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

    $("#txtIdRol").val(modelo.idRol)
    $("#txtNombreRol").val(modelo.nombreRol)
    $("#cboEstado").val(modelo.estado)
    $("#modalDataRol").modal("show")
}


$("#btnNuevoRol").click(function () {
    mostrarModal()
})


$("#btnGuardarRol").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idRol"] = parseInt($("#txtIdRol").val())
    modelo["nombreRol"] = $("#txtNombreRol").val()
    modelo["estado"] = $("#cboEstado").val()
  


    $("#modalDataRol").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idRol == 0) {

        fetch("/Rol/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
            
        })
            .then(response => {
                $("#modalDataRol").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataRol.row.add(responseJson.objeto).draw(false)
                    $("#modalDataRol").modal("hide")
                    swal("Listo", "el Rol fue creado", "success")
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
    else {

        fetch("/Rol/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(modelo)
        })
            .then(response => {
                $("#modalDataRol").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataRol.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataRol").modal("hide")
                    swal("Listo", "el Rol fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })





    }
})

let filaSeleccionada;

$("#tbdataRol tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataRol.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdataRol tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaDataRol.row(fila).data();

    swal({
        title: "¿Estas Seguro?",
        text: `Eliminar  "${data.nombreRol}"`,
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
                fetch(`/Rol/Eliminar?IdRol=${data.idRol}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataRol.row(fila).remove().draw();
                            swal("Listo", "el Rol fue eliminado", "success")

                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )

})