<?php
session_start();
include "conecta.php";

if (!isset($_SESSION["logueado"]) || $_SESSION["logueado"] !== true) {
    header("Location: ingreso.html");
    exit();
}

$usuario = $_SESSION["usuario"];

$sqlCuenta = "SELECT t.num_cuenta FROM usuarios u JOIN tarjetas t ON u.documento = t.dni_titular WHERE u.usuario = '$usuario' LIMIT 1";

$resCuenta = $conn->query($sqlCuenta);

if (!$resCuenta || $resCuenta->num_rows == 0) {
    die("No se encontró tarjeta para el usuario");
}

$cuenta = $resCuenta->fetch_assoc()["num_cuenta"];
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Panel del Cliente</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>

<body class="bg-gray-100 font-sans">

<header class="bg-[#004691] text-white p-4 text-center">
    <h1 class="text-xl font-bold">Panel del Cliente</h1>
</header>

<main class="max-w-5xl mx-auto p-6">

<h2 class="text-lg font-semibold mb-6">
    Bienvenido, <?php echo $usuario; ?>
</h2>

<?php
$sqlActual = "SELECT * FROM liquidaciones WHERE num_cuenta = $cuenta ORDER BY periodo DESC LIMIT 1";

$resActual = $conn->query($sqlActual);

echo "<div class='bg-white p-4 rounded shadow mb-6'>";
echo "<h3 class='text-lg font-bold mb-2'>Liquidación Actual</h3>";

if ($resActual && $resActual->num_rows > 0) {
    $fila = $resActual->fetch_assoc();

    echo "<p><b>Período:</b> {$fila['periodo']}</p>";
    echo "<p><b>Vencimiento:</b> {$fila['fecha_vencimiento']}</p>";
    echo "<p><b>Total a pagar:</b> $ {$fila['total_a_pagar']}</p>";
    echo "<p><b>Pago mínimo:</b> $ {$fila['pago_minimo']}</p>";
} else {
    echo "<p>No hay liquidaciones disponibles.</p>";
}

echo "</div>";

$sqlHistorial = "SELECT * FROM liquidaciones WHERE num_cuenta = $cuenta ORDER BY periodo DESC";

$resHistorial = $conn->query($sqlHistorial);

echo "<div class='bg-white p-4 rounded shadow'>";
echo "<h3 class='text-lg font-bold mb-3'>Historial de Liquidaciones</h3>";

if ($resHistorial && $resHistorial->num_rows > 0) {

    echo "<table class='w-full text-sm border'>";
    echo "<tr class='bg-gray-200'>
            <th class='p-2'>Período</th>
            <th class='p-2'>Vencimiento</th>
            <th class='p-2'>Total</th>
            <th class='p-2'>Pago mínimo</th>
            </tr>";

    while ($fila = $resHistorial->fetch_assoc()) {
        echo "<tr class='border-t'>
                <td class='p-2'>{$fila['periodo']}</td>
                <td class='p-2'>{$fila['fecha_vencimiento']}</td>
                <td class='p-2'>$ {$fila['total_a_pagar']}</td>
                <td class='p-2'>$ {$fila['pago_minimo']}</td>
                </tr>";
    }

    echo "</table>";

} else {
    echo "<p>No hay historial disponible.</p>";
}

echo "</div>";

$conn->close();
?>

</main>
</body>
</html>