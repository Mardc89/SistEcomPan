

function ObtenerDatosCliente() {
    fetch("/Home/ObtenerCliente")
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



$(document).ready(function () {
    ObtenerDatosCliente();
    debugger;
    let dni = document.getElementById("DniPersonal").textContent;
    let correo = document.getElementById("CorreoPersonal").textContent;
    $("div.container-fluid").LoadingOverlay("show");

    fetch(`/DashBoard/ObtenerResumenCliente?dni=${dni}&correo=${correo}`)
        .then(response => {
            $("div.container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                let d = responseJson.objeto
                $("#TotalDeMisPagos").text(d.totalDeMisPagos)
                $("#TotalDeMisMensajes").text(d.totalDeMisMensajes)
                $("#TotalDeMisPedidos").text(d.totalDeMisPedidos)


                let barchart_labels;
                let barchart_data;

                if (d.pagosUltimaSemana.length > 0) {
                    barchart_labels = d.pagosUltimaSemana.map((item) => { return item.fechaPago })
                    barchart_data = d.pagosUltimaSemana.map((item) => { return item.montoTotalDePago })
                }
                else {
                    barchart_labels = ["Sin resultados"]
                    barchart_data   = 0

                }

                let piechart_labels;
                let piechart_data;

                if (d.productosTopUltimaSemana.length > 0) {
                    piechart_labels = d.productosTopUltimaSemana.map((item) => { return item.producto })
                    piechart_data = d.productosTopUltimaSemana.map((item) => { return item.cantidad })
                }
                else {
                    piechart_labels = ["Sin resultados"]
                    piechart_data = 0

                }

                // Bar Chart Example
                let controlVenta = document.getElementById("chartPedidos");
                let myBarChartPedidos = new Chart(controlVenta, {
                    type: 'bar',
                    data: {
                        labels: barchart_labels,
                        datasets: [{
                            label: "Cantidad",
                            backgroundColor: "#4e73df",
                            hoverBackgroundColor: "#2e59d9",
                            borderColor: "#4e73df",
                            data: barchart_data,
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        legend: {
                            display: false
                        },
                        scales: {
                            xAxes: [{
                                gridLines: {
                                    display: false,
                                    drawBorder: false
                                },
                                maxBarThickness: 50,
                            }],
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    maxTicksLimit: 5
                                }
                            }],
                        },
                    }
                });

                // Pie Chart Example
                let controlProducto = document.getElementById("chartProducts");
                let myPieChartProduct = new Chart(controlProducto, {
                    type: 'doughnut',
                    data: {
                        labels: piechart_labels,
                        datasets: [{
                            data: piechart_data,
                            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                            hoverBorderColor: "rgba(234, 236, 244, 1)",
                        }],
                    },
                    options: {
                        maintainAspectRatio: false,
                        tooltips: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyFontColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            caretPadding: 10,
                        },
                        legend: {
                            display: true
                        },
                        cutoutPercentage: 80,
                    },
                });



    

            }

        })

})
