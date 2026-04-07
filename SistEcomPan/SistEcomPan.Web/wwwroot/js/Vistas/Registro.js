
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


    $("#txtDni, #txtTelefono").on("input", function () {
        this.value = this.value.replace(/\D/g, '');
    });

    $("#txtNombres,#txtApellidos").on("input", function () {
        this.value = this.value.replace(/[^\p{L}\s]/gu, '');
    });
})


document.addEventListener("DOMContentLoaded", function () {
    debugger;
    const form = document.getElementById("FormRegistro");
/*    const btnRegistroCliente = document.getElementById("btnRegistroCliente");*/

    if (form) {
        form.addEventListener("submit", function (event) {
            event.preventDefault();
            validarFormulario(form);
        });
    }
    const formReset = document.getElementById("formularioReset");
    /*    const btnRegistroCliente = document.getElementById("btnRegistroCliente");*/

    if (formReset) {
        formReset.addEventListener("submit", function (event) {
            event.preventDefault();
            validarFormularioReset(formReset);
        });
    }

    const formUsuario = document.getElementById("formularioDeUsuario");

    if (formUsuario) {
        $("#btnGuardar").click(function () {
            validarFormularioUsuario();
        });
    }

});


function validarFormularioUsuario() {





}


function validarFormularioReset(formReset) {

    const nuevaClave = $("#NuevaClave").val()?.trim();
    const confirmarClave = $("#ConfirmarClave").val()?.trim();

    if (!nuevaClave || nuevaClave.length < 8) {
        swal(" ", "La contraseña debe tener al menos 8 caracteres", "error");
        $("#NuevaClave").focus();
        return;
    }

    if (nuevaClave !== confirmarClave) {
        swal(" ", "Las contraseñas no coinciden", "error");
        $("#ConfirmarClave").focus();
        return;
    }

    formReset.submit();


}

function validarFormulario(form) {
    debugger;
        const campos = [
         
            { id: "txtNombres", mensaje: "Debe Ingresar sus Nombres", longitudMinima: 4, longitudMaxima: 30, tipo: "texto" },
            { id: "txtApellidos", mensaje: "Debe Ingresar sus Apellidos", longitudMinima: 9, longitudMaxima: 30, tipo: "texto" },
            { id: "txtDni", mensaje: "Debe Completar el campo Dni", longitudMinima: 8, tipo: "numerico" },
            { id: "txtTelefono", mensaje: "Debe Ingresar su Telefono", longitudMinima: 9, tipo: "numerico" },
            { id: "cboTipoCliente", mensaje: "Debes Seleccionar un Tipo de Cliente" },
            { id: "cboDistrito", mensaje: "Debes Seleccionar un Distrito" },
            { id: "txtDireccion", mensaje: "Debe Ingresar su Direccion", longitudMinima: 10, longitudMaxima: 50 },
            { id: "txtNombreUsuario", mensaje: "Debe Ingresar Nombre de Usuario", longitudMinima: 4, longitudMaxima: 50 },
            { id: "txtPassword", mensaje: "Debe Ingresar su contraseña", longitudMinima: 10, longitudMaxima: 90 },
            { id: "txtCorreo", mensaje: "Debe Ingresar su Correo", longitudMaxima: 50 ,tipo:"correo"},
        
        ];

    for (let campo of campos) {
        let valor = $(`#${campo.id}`).val();

        // Evita undefined
        valor = valor ? valor.trim() : "";

        // 🔹 VALIDAR VACÍO
        if (campo.id.startsWith("txt") && valor === "") {
            toastr.warning("", campo.mensaje);
            $(`#${campo.id}`).focus();
            return;
        }

        if (campo.id.startsWith("cbo") && !valor) {
            toastr.warning("", campo.mensaje);
            $(`#${campo.id}`).focus();
            return;
        }

        // 🔹 VALIDAR TIPO (solo si hay valor)
        if (valor) {
            if (campo.tipo === "numerico" && !/^\d+$/.test(valor)) {
                toastr.warning("", "Solo se permiten numeros en este campo");
                $(`#${campo.id}`).focus();
                return;
            }

            if (campo.tipo === "texto" && !/^[\p{L}\s]+$/u.test(valor)) {
                toastr.warning("", "Solo se permiten letras en este campo");
                $(`#${campo.id}`).focus();
                return;
            }

            if (campo.tipo === "correo") {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(valor)) {
                    toastr.warning("", "Correo no válido");
                    $(`#${campo.id}`).focus();
                    return;
                }
            }
        }

        // 🔹 VALIDAR LONGITUD
        if (campo.longitudMinima && valor.length < campo.longitudMinima) {
            toastr.warning("", `El campo debe tener al menos ${campo.longitudMinima} caracteres`);
            $(`#${campo.id}`).focus();
            return;
        }

        if (campo.longitudMaxima && valor.length > campo.longitudMaxima) {
            toastr.warning("", `El campo no debe tener más de ${campo.longitudMaxima} caracteres`);
            $(`#${campo.id}`).focus();
            return;
        }
    }



    form.submit();

    }
    