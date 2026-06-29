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

$sqlUsuario = "INSERT INTO usuarios 
(documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
VALUES 
('$dni', '$tipodoc', '$nombre', '$apellido', '$fecha_nac', '$email', '$usuario', '$passA')";

if ($conn->query($sqlUsuario) === TRUE) {

    echo "Usuario registrado con éxito<br>";

    $sqlTarjeta = "SELECT num_cuenta FROM tarjetas LIMIT 1";

    $result = $conn->query($sqlTarjeta);

    if ($result && $row = $result->fetch_assoc()) {

        $numCuenta = $row["num_cuenta"];

        $sqlUpdate = "UPDATE tarjetas SET dni_titular = '$dni' WHERE num_cuenta = $numCuenta";

        if ($conn->query($sqlUpdate) === TRUE) {
            echo "<br>Tarjeta asignada correctamente (Cuenta: $numCuenta)";
        } else {
            echo "<br>Error al asignar tarjeta: " . $conn->error;
        }

    } else {
        echo "<br>Error: no se pudo obtener tarjeta";
    }

} else {
    echo "Error al registrar usuario: " . $conn->error;
}

$conn->close();
?>