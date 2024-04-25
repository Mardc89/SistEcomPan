﻿

const MODELO_BASE = {
    idProducto: 0,
    descripcion: "",
    idCategoria: 0,
    precio: "",
    nombreCategoria:"",
    estado: 1,
    stock: "",
    urlImagen:"",

}

let tablaDataProducto;

$(document).ready(function () {

    fetch("/Producto/ListaCategorias")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoria).text(item.tipoDeCategoria)
                    )
                })
            }
        })

    tablaData = $('#tbdataProducto').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Producto/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    let ruta = data;
                    let rutaRelativa = ruta.replace('C:\\Proyects\\SistEcomPan\\SistEcomPan\\SistEcomPan.Web\\wwwroot\\Imagenes\\', 'Imagenes/');
                    return `<img style="height:60px" src=${rutaRelativa} class="rounded mx-auto d-block"/>`;
                }

            },
            { "data": "descripcion" },
            { "data": "nombreCategoria" },
            { "data": "precio" },
            { "data": "stock" },
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
        dom: "Bfrtip",
        buttons: [
            {
                text: "Exportar Excel",
                extend: "excelHtml5",
                title: "",
                filename: "Reporte Usuarios",
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plugin-ins/1.11.5/i18n/es-Es.json"
        },
    });
})


function mostrarModal(modelo = MODELO_BASE) {
    debugger;
    let rutaCompleta = modelo.urlImagen;
    let rutaRelativa = rutaCompleta.replace('C:\\Proyects\\SistEcomPan\\SistEcomPan\\SistEcomPan.Web\\wwwroot\\Imagenes\\', 'Imagenes/');
    $("#txtIdProducto").val(modelo.idProducto)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#txtStock").val(modelo.stock)
    $("#txtPrecio").val(modelo.precio)
    $("#cboCategoria").val(modelo.idCategoria == 0 ? $("#cboCategoria option:first").val() : modelo.idCategoria)
    $("#cboEstado").val(modelo.estado)
    $("#txtImagen").val("")
    $("#imgProducto").attr("src", rutaRelativa)

    $("#modalDataProducto").modal("show")
}


$("#btnNuevoProducto").click(function () {
    mostrarModal()
})


$("#btnGuardarProducto").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo:"${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje)
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus()
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idProducto"] = parseInt($("#txtIdProducto").val())
    modelo["descripcion"] = $("#txtDescripcion").val()
    modelo["precio"] = $("#txtPrecio").val()
    modelo["stock"] = $("#txtStock").val()
    modelo["idCategoria"] = $("#cboCategoria").val()
    modelo["estado"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtImagen")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalDataProducto").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idUsuario == 0) {

        fetch("/Producto/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalDataProducto").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataProducto.row.add(responseJson.objeto).draw(false)
                    $("#modalDataProducto").modal("hide")
                    swal("Listo", "el producto fue creado", "success")
                }
                else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })
    }
    else {

        fetch("/Producto/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalDataProducto").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {

                if (responseJson.estado) {

                    tablaDataProducto.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataProducto").modal("hide")
                    swal("Listo", "el producto fue modificado", "success")
                } else {
                    swal("Lo sentimos", responseJson.mensaje, "error")
                }
            })





    }
})

let filaSeleccionada;

$("#tbdataProducto tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaDataProducto.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#tbdataProducto tbody").on("click", ".btn-eliminar", function () {

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
                fetch(`/Producto/Eliminar?IdProducto=${data.idProducto}`, {
                    method: "DELETE",

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            tablaDataProducto.row(fila).remove().draw();
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