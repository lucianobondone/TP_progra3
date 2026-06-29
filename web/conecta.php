<?php
$server = "db";
$usuario = "root";
$password = "abcde1234";
$baseDatos = "mi_banco_db";

$conn = new mysqli(
    $server,
    $usuario,
    $password,
    $baseDatos
);

if ($conn->connect_error) {
    die("Error de conexión: " . $conn->connect_error);
}

//echo "Conexión exitosa";
//$conn->close();
?>
