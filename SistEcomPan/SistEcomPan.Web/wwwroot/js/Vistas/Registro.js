
const MODELO_BASE = {
    idDistrito: 0,
    nombreDistrito: "",

}



$(document).ready(function () {
   
    fetch("/Cliente/ListaDistritos")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboDistrito").append(
                        $("<option>").val(item.idDistrito).text(item.nombreDistrito)
                    )
                })
          
            }
          
        })
})


document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
/*    const btnRegistroCliente = document.getElementById("btnRegistroCliente");*/

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        validarFormulario(form);
    })


});

    function validarFormulario(form) {

        const campos = [
            { id: "txtDni", mensaje: "Debe Completar el campo Dni" ,longitudMaxima:8,tipo:"numerico"},
            { id: "txtNombres", mensaje: "Debe Ingresar sus Nombres", longitudMinima: 20, longitudMaxima: 50, tipo: "texto" },
            { id: "txtApellidos", mensaje: "Debe Ingresar sus Apellidos", longitudMinima: 20, longitudMaxima: 50, tipo: "texto" },
            { id: "txtTelefono", mensaje: "Debe Ingresar su Telefono", longitudMaxima: 9, tipo: "numerico" },
            { id: "cboTipoCliente", mensaje: "Debes Seleccionar un Tipo de Cliente"},
            { id: "txtDireccion", mensaje: "Debe Ingresar su Direccion", longitudMinima: 10, longitudMaxima: 50 },
            { id: "cboDistrito", mensaje: "Debes Seleccionar un Distrito"},
            { id: "txtCorreo", mensaje: "Debe Ingresar su Correo", longitudMaxima: 50 },
            { id: "txtNombreUsuario", mensaje: "Debe Ingresar Nombre de Usuario", longitudMinima: 4, longitudMaxima: 50 },
            { id: "txtPassword", mensaje: "Debe Ingresar su contraseña",longitudMinima:10, longitudMaxima: 90 },
        ];

        for (let campo of campos) {
            const valor = $(`#${campo.id}`).val()?.trim();
            if (campo.id.startsWith("txt") && valor==="") {          
                toastr.warning("", campo.mensaje);
                $(`#${campo.id}`).focus();
                return;         
            }

            if (campo.longitudMinina && valor.length < campo.longitudMinima) {
                toastr.warning("", `El campo debe tener al menos${campo.longitudMinima} caracteres`);
                $(`#${campo.id}`).focus();
                return;  

            }
            if (campo.tipo === "numerico" && !/^\d+$/.test(valor)) {
                toastr.warning("","Solo se permiten numeros en este campo");
                $(`#${campo.id}`).focus();
                return;

            }

            if (campo.tipo === "texto" && !/^[a-zA-Z\s]+$/.test(valor)) {
                toastr.warning("","Solo se permiten letras en este campo");
                $(`#${campo.id}`).focus();
                return;

            }

            if (campo.longitudMaxima && valor.length > campo.longitudMaxima) {
                toastr.warning("", `El campo no debe tener mas de ${campo.longitudMaxima} caracteres`);
                $(`#${campo.id}`).focus();
                return;

            }

            if (campo.id.startsWith("cbo") && !valor) {
                toastr.warning("", campo.mensaje);
                $(`#${campo.id}`).focus();
                return;
            }

        }

        const nuevaClave = $("#NuevaClave").val()?.trim();
        const confirmarClave = $("#ConfirmarClave").val()?.trim();

        if (!nuevaClave || nuevaClave.length < 8) {
            toastr.warning("", "La contraseña debe tener al menos 8 caracteres");
            $("#NuevaClave").focus();
            return;
        }

        if (nuevaClave !== confirmarClave) {
            toastr.warning("", "Las contraseñas no coinciden");
            $("#ConfirmarClave").focus();
            return;
        }

        form.submit();

    }
    