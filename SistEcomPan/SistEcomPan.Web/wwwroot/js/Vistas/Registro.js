
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