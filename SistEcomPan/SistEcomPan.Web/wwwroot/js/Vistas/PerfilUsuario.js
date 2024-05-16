$(document).ready(function () {

    fetch("/Usuario/ListaRoles")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#txtIdRol").append(
                        $("<option>").val(item.idRol).text(item.nombreRol)
                    )
                })
            }
        })

    $(".container-fluid").LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario")
        .then(response => {
            $(".container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#ImgFoto").attr("src", `/Imagenes/${d.nombreFoto}`)
                $("#txtDni").val(d.dni)
                $("#txtIdRol").val(d.idRol)
                $("#txtNombres").val(d.nombres)
                $("#txtApellidos").val(d.apellidos)
                $("#txtCorreo").val(d.correo)
                $("#txtNombreUsuario").val(d.nombreUsuario)
                $("#txtClave").val(d.clave)
                $("#txtRol").val(d.nombreRol)
           
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
        return
    };



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
                    dni: $("#txtDni").val().trim(),
                    nombres: $("#txtNombres").val().trim(),
                    apellidos: $("#txtApellidos").val().trim(),
                    idRol: $("#txtIdRol").val().trim(),
                    nombreUsuario: $("#txtNombreUsuario").val().trim(),
                    correo: $("#txtCorreo").val().trim(),
                    clave: $("#txtClave").val().trim(),
          
                }


                const inputFoto = document.getElementById("txtFoto");

                const formData = new FormData();

                formData.append("foto", inputFoto.files[0])
                formData.append("modelo", JSON.stringify(modelo))

                fetch("/Home/GuardarPerfil", {
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
