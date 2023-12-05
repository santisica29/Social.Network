document.querySelector("#formularioRegistro").addEventListener('submit', validarDatos);

function validarDatos(event) {
    event.preventDefault();

    let nombre = document.querySelector("#nombre").value;
    let apellido = document.querySelector("#apellido").value;
    let fechaN = document.querySelector("#fechaN").value;
    let email = document.querySelector("#email").value;
    let pw = document.querySelector("#pw").value;

    if (nombre != "" && apellido != "" && fechaN != "" && email != "" && pw != "") {
        this.submit();
    }
    else {
        document.querySelector("#error-msg").innerHTML = "Falta algun dato. No puede ser vacío.";
    }
}