document.getElementById("formularioLogin").addEventListener('submit', validarDatos);

function validarDatos(event) {
    event.preventDefault();

    let email = document.getElementById("email").value;
    let password = document.getElementById("password").value;

    if (email != "" && password != "") {
        this.submit();
    } else {
        document.querySelector("#error-msg").innerHTML = "Falta algun dato. No puede ser vacío.";
    }
}