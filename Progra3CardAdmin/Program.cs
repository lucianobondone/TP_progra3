using System;
using MySql.Data.MySqlClient; 

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Port=3306;Database=mi_banco_db;Uid=usuario;Pwd=abcde5678;AllowPublicKeyRetrieval=True;SslMode=Disabled;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // =========================================================================
        // OPCIÓN 1: EMITIR NUEVA TARJETA (ALTA DE CLIENTE)
        // =========================================================================
        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- EMISIÓN DE NUEVA TARJETA ---");

            Console.Write("Ingrese el DNI/Documento del titular: ");
            string dni = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 1) Verificamos si el usuario ya existe en la tabla 'usuarios'
                    string sqlBuscaUsuario = "SELECT * FROM usuarios WHERE documento = @dni";
                    MySqlCommand cmdBusca = new MySqlCommand(sqlBuscaUsuario, conn);
                    cmdBusca.Parameters.AddWithValue("@dni", dni);

                    bool usuarioExiste = false;
                    using (MySqlDataReader reader = cmdBusca.ExecuteReader())
                    {
                        usuarioExiste = reader.HasRows;
                    }

                    // 2) Si no existe, pedimos los datos personales y lo damos de alta
                    if (!usuarioExiste)
                    {
                        Console.WriteLine("\nNo existe un cliente con ese documento. Se registrará uno nuevo.");

                        Console.Write("Tipo de documento (1=DNI, 2=PASAPORTE): ");
                        string opcionDoc = Console.ReadLine();
                        string tipoDoc = (opcionDoc == "2") ? "PASAPORTE" : "DNI";

                        Console.Write("Nombre: ");
                        string nombre = Console.ReadLine();

                        Console.Write("Apellido: ");
                        string apellido = Console.ReadLine();

                        Console.Write("Fecha de nacimiento (YYYY-MM-DD): ");
                        string fechaNac = Console.ReadLine();

                        Console.Write("Email: ");
                        string email = Console.ReadLine();

                        string sqlInsertUsuario = @"INSERT INTO usuarios 
                            (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
                            VALUES (@documento, @tipo_doc, @nombre, @apellido, @fecha_nac, @email, NULL, NULL)";

                        MySqlCommand cmdInsertUsuario = new MySqlCommand(sqlInsertUsuario, conn);
                        cmdInsertUsuario.Parameters.AddWithValue("@documento", dni);
                        cmdInsertUsuario.Parameters.AddWithValue("@tipo_doc", tipoDoc);
                        cmdInsertUsuario.Parameters.AddWithValue("@nombre", nombre);
                        cmdInsertUsuario.Parameters.AddWithValue("@apellido", apellido);
                        cmdInsertUsuario.Parameters.AddWithValue("@fecha_nac", fechaNac);
                        cmdInsertUsuario.Parameters.AddWithValue("@email", email);

                        cmdInsertUsuario.ExecuteNonQuery();
                        Console.WriteLine("Cliente registrado correctamente.");
                    }
                    else
                    {
                        Console.WriteLine("\nCliente encontrado. Se procederá a emitir la tarjeta.");
                    }

                    // 3) Datos de la tarjeta
                    Console.Write("\nNúmero de tarjeta (16 dígitos): ");
                    string numeroTarjeta = Console.ReadLine();

                    string bancoEmisor = SeleccionarBancoEmisor();

                    Console.Write("Saldo inicial (dejar vacío para 0.00): ");
                    string saldoStr = Console.ReadLine();
                    decimal saldo = string.IsNullOrWhiteSpace(saldoStr) ? 0.00m : Convert.ToDecimal(saldoStr);

                    string sqlInsertTarjeta = @"INSERT INTO tarjetas 
                        (numero_tarjeta, banco_emisor, estado, saldo, dni_titular)
                        VALUES (@numero_tarjeta, @banco_emisor, 'Activa', @saldo, @dni_titular)";

                    MySqlCommand cmdInsertTarjeta = new MySqlCommand(sqlInsertTarjeta, conn);
                    cmdInsertTarjeta.Parameters.AddWithValue("@numero_tarjeta", numeroTarjeta);
                    cmdInsertTarjeta.Parameters.AddWithValue("@banco_emisor", bancoEmisor);
                    cmdInsertTarjeta.Parameters.AddWithValue("@saldo", saldo);
                    cmdInsertTarjeta.Parameters.AddWithValue("@dni_titular", dni);

                    cmdInsertTarjeta.ExecuteNonQuery();

                    Console.WriteLine("\n✔ Tarjeta emitida correctamente para el DNI " + dni + ".");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n✖ Error al emitir la tarjeta: " + ex.Message);
                }
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static string SeleccionarBancoEmisor()
        {
            string[] bancos = {
                "Banco Nación",
                "Banco Provincia",
                "Banco Galicia",
                "Banco Santander",
                "Banco BBVA",
                "Banco Macro"
            };

            int opcion = -1;
            while (opcion < 1 || opcion > bancos.Length)
            {
                Console.WriteLine("\nSeleccione el Banco Emisor:");
                for (int i = 0; i < bancos.Length; i++)
                {
                    Console.WriteLine("{0}. {1}", i + 1, bancos[i]);
                }
                Console.Write("Opción: ");

                if (!int.TryParse(Console.ReadLine(), out opcion))
                {
                    opcion = -1;
                }

                if (opcion < 1 || opcion > bancos.Length)
                {
                    Console.WriteLine("Opción inválida, intente nuevamente.");
                }
            }

            return bancos[opcion - 1];
        }

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }
        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("--- EMISIÓN DE NUEVA LIQUIDACIÓN MENSUAL ---");

            Console.Write("Número de cuenta: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sqlVerifica = "SELECT numero_tarjeta FROM tarjetas WHERE num_cuenta = @cuenta";
                    MySqlCommand cmdVerifica = new MySqlCommand(sqlVerifica, conn);
                    cmdVerifica.Parameters.AddWithValue("@cuenta", numCuenta);

                    object resultado = cmdVerifica.ExecuteScalar();

                    if (resultado == null)
                    {
                        Console.WriteLine("\n✖ No existe ninguna tarjeta con ese número de cuenta.");
                    }
                    else
                    {
                        Console.WriteLine("Tarjeta encontrada: " + resultado.ToString());

                        Console.Write("Período (formato YYYY-MM): ");
                        string periodo = Console.ReadLine();

                        Console.Write("Fecha de vencimiento (YYYY-MM-DD): ");
                        string fechaVencimiento = Console.ReadLine();

                        Console.Write("Total a pagar: ");
                        decimal totalAPagar = Convert.ToDecimal(Console.ReadLine());

                        Console.Write("Pago mínimo: ");
                        decimal pagoMinimo = Convert.ToDecimal(Console.ReadLine());

                        string sqlInsert = @"INSERT INTO liquidaciones 
                            (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo)
                            VALUES (@num_cuenta, @periodo, @fecha_vencimiento, @total_a_pagar, @pago_minimo)";

                        MySqlCommand cmdInsert = new MySqlCommand(sqlInsert, conn);
                        cmdInsert.Parameters.AddWithValue("@num_cuenta", numCuenta);
                        cmdInsert.Parameters.AddWithValue("@periodo", periodo);
                        cmdInsert.Parameters.AddWithValue("@fecha_vencimiento", fechaVencimiento);
                        cmdInsert.Parameters.AddWithValue("@total_a_pagar", totalAPagar);
                        cmdInsert.Parameters.AddWithValue("@pago_minimo", pagoMinimo);

                        cmdInsert.ExecuteNonQuery();

                        Console.WriteLine("\n✔ Liquidación emitida correctamente para la cuenta " + numCuenta + ".");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n✖ Error al emitir la liquidación: " + ex.Message);
                }
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void ObtenerYMostrarTarjetas()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No hay tarjetas registradas en el sistema.");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}",
                                    reader["num_cuenta"],
                                    reader["numero_tarjeta"],
                                    reader["banco_emisor"],
                                    reader["dni_titular"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar las tarjetas: " + ex.Message);
                }
            }
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"SELECT t.num_cuenta, t.numero_tarjeta, t.banco_emisor, t.estado, t.saldo,
                                    u.documento, u.tipo_doc, u.nombre, u.apellido, u.email, u.fecha_nacimiento
                                    FROM tarjetas t
                                    JOIN usuarios u ON t.dni_titular = u.documento
                                    WHERE t.num_cuenta = @cuenta";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cuenta", cuenta);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine();
                            Console.WriteLine("Número de Cuenta : " + reader["num_cuenta"]);
                            Console.WriteLine("Número de Tarjeta: " + reader["numero_tarjeta"]);
                            Console.WriteLine("Banco Emisor     : " + reader["banco_emisor"]);
                            Console.WriteLine("Estado           : " + reader["estado"]);
                            Console.WriteLine("Saldo            : $ " + reader["saldo"]);
                            Console.WriteLine("--------------------------------------------------");
                            Console.WriteLine("Documento        : " + reader["documento"] + " (" + reader["tipo_doc"] + ")");
                            Console.WriteLine("Nombre Completo  : " + reader["nombre"] + " " + reader["apellido"]);
                            Console.WriteLine("Email            : " + reader["email"]);
                            Console.WriteLine("Fecha Nacimiento : " + reader["fecha_nacimiento"]);
                        }
                        else
                        {
                            Console.WriteLine("\nNo se encontró ninguna tarjeta/cliente con ese número de cuenta.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el detalle: " + ex.Message);
                }
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@cuenta", cuenta);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al eliminar la tarjeta: " + ex.Message);
                    return false;
                }
            }
        }
    }
}