<?php
session_start();
include "conecta.php";

$tipodoc = $_POST["tipo_doc"];
$dni = $_POST["documento"];
$usuario = $_POST["usuario"];
$pass = $_POST["password"];

$sql = "SELECT * FROM usuarios 
        WHERE tipo_doc='$tipodoc'
        AND documento='$dni'
        AND usuario='$usuario'
        AND password='$pass'";

$resultado = $conn->query($sql);

if ($resultado && $resultado->num_rows > 0) {

    $fila = $resultado->fetch_assoc();

    $_SESSION["logueado"] = true;
    $_SESSION["usuario"] = $fila["usuario"];

    header("Location: resumen.php");
    exit();

} else {
    echo "Usuario o contraseña incorrectos";
}

$conn->close();
?>