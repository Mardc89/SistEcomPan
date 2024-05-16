
$(document).ready(function () {

    fetch("/Cliente/ListaDistritos")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#txtIdDistrito").append(
                        $("<option>").val(item.idDistrito).text(item.nombreDistrito)
                    )
                })
            }
        })

    $(".container-fluid").LoadingOverlay("show");

    fetch("/Home/ObtenerCliente")
        .then(response => {
            $(".container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#ImgFoto").attr("src", `/ImagenesPerfil/${d.nombreFoto}`)
/*                $("#txtFoto").val(d.urlFoto)*/
                $("#txtDni").val(d.dni)
                $("#txtIdDistrito").val(d.idDistrito)
                $("#txtTipoCliente").val(d.tipoCliente)
                $("#txtNombres").val(d.nombres)
                $("#txtApellidos").val(d.apellidos)
                $("#txtCorreo").val(d.correo)
                $("#txtDireccion").val(d.direccion)
                $("#txtTelefono").val(d.telefono)
                $("#txtDistrito").val(d.nombreDistrito)
                $("#txtNombreUsuario").val(d.nombreUsuario)
                $("#txtClave").val(d.clave)

            }
            else {

                swal("Lo sentimos", responseJson.mensaje, "error")
            }
        })

})


$("#btnGuardarCambios").click(function () {

    if ($("#txtCorreo").val().trim() == "") {
        toastr.warning("", "Debe completar el campo correo")
        $("#txtCorreo").focus()
        return;

    }

    if ($("#txtClave").val().trim() == "") {
        toastr.warning("", "Debe Ingresar una contraseña")
        $("#txtClave").focus()
        return};



    swal({
        title: "¿Deseas Guardar los cambios?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {

            if (respuesta) {

                $(".showSweetAlert").LoadingOverlay("show");

                let modelo = {
                    dni:$("#txtDni").val().trim(),
                    tipoCliente:$("#txtTipoCliente").val().trim(),
                    nombres:$("#txtNombres").val().trim(),
                    apellidos:$("#txtApellidos").val().trim(),
                    direccion:$("#txtDireccion").val().trim(),
                    idDistrito:$("#txtIdDistrito").val().trim(),
                    nombreUsuario:$("#txtNombreUsuario").val().trim(),
                    correo:$("#txtCorreo").val().trim(),
                    clave:$("#txtClave").val().trim(),
                    telefono:$("#txtTelefono").val().trim()

                }


                const inputFoto = document.getElementById("txtFoto");

                const formData = new FormData();

                formData.append("foto", inputFoto.files[0])
                formData.append("modelo", JSON.stringify(modelo))

                fetch("/Home/GuardarPerfilCliente", {
                    method: "POST",
                    body: formData

                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            swal("Listo", "Los Cambios fueron Guardados", "success")
                        }
                        else {
                            swal("Lo sentimos", responseJson.mensaje, "error")
                        }
                    })

            }


        }




    )


})

