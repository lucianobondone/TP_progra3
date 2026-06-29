<?php
include "conecta.php";

$nombre = $_POST["nombre"];
$apellido = $_POST["apellido"];
$tipodoc = $_POST["tipo_doc"];
$dni = $_POST["documento"];
$fecha_nac = $_POST["fecha_nacimiento"];
$email = $_POST["email"];
$usuario = $_POST["usuario"];
$passA = $_POST["passwordA"];
$passB = $_POST["passwordB"];

if ($passA != $passB) {
    die("Las contraseñas no coinciden");
}

if ($tipodoc != "DNI" && $tipodoc != "PASAPORTE") {
    die("Tipo de documento inválido");
}

$sqlVerificaTarjeta = "SELECT num_cuenta FROM tarjetas WHERE dni_titular = '$dni'";
$resVerifica = $conn->query($sqlVerificaTarjeta);

if (!$resVerifica || $resVerifica->num_rows == 0) {
    die("No se encontró ninguna tarjeta emitida para el documento ingresado.");
}

$filaTarjeta = $resVerifica->fetch_assoc();
$numCuenta = $filaTarjeta["num_cuenta"];

$sqlUsuario = "INSERT INTO usuarios 
(documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
VALUES 
('$dni', '$tipodoc', '$nombre', '$apellido', '$fecha_nac', '$email', '$usuario', '$passA')";

if ($conn->query($sqlUsuario) === TRUE) {

    echo "Usuario registrado con éxito<br>";
    echo "<br>Tarjeta vinculada correctamente (Cuenta: $numCuenta)";

} else {
    echo "Error al registrar usuario: " . $conn->error;
}

$conn->close();
?>