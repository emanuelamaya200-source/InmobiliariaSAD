
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_.Net_Core.Models

{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Pago
                    (IdReserva, Monto, Concepto, Estado, Fecha, UsuarioCreacionId)
                    VALUES (@idreserva, @monto, @concepto, @estado, @fecha, @usuario);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idreserva", p.IdReserva);
                    command.Parameters.AddWithValue("@monto", p.Monto);
                    command.Parameters.AddWithValue("@concepto", p.Concepto);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    command.Parameters.AddWithValue("@fecha", p.Fecha.ToDateTime(TimeOnly.MinValue));
                    command.Parameters.AddWithValue("@usuario", (object?)p.UsuarioCreacionId ?? DBNull.Value);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPago = res;
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Pago SET Estado = 'Inactivo' WHERE IdPago = @id AND Estado <> 'Inactivo'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Reactivar(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Pago SET Estado = 'Activo' WHERE IdPago = @id AND Estado = 'Inactivo'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Pago 
                    SET Concepto=@concepto
                    WHERE IdPago = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@concepto", p.Concepto);
                    command.Parameters.AddWithValue("@id", p.IdPago);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT p.IdPago, p.IdReserva, p.Monto, p.Concepto, p.Estado, p.Fecha,
                    io.Dni AS DniInquilino,
                    CONCAT(io.Apellido, ', ', io.Nombre) AS NombreInquilino,
                    ie.IdInmueble, ie.Direccion
                    FROM Pago p
                    INNER JOIN Reserva r ON p.IdReserva = r.IdReserva
                    INNER JOIN Inmueble ie ON r.IdInmueble = ie.IdInmueble
                    INNER JOIN Inquilino io ON r.IdInquilino = io.IdInquilino
                    WHERE p.IdPago = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Pago
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                IdReserva = reader.GetInt32(nameof(Pago.IdReserva)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                Concepto = reader.GetString(nameof(Pago.Concepto)),
                                Estado = reader.GetString(nameof(Pago.Estado)),
                                Fecha = DateOnly.FromDateTime(reader.GetDateTime(nameof(Pago.Fecha))),
                                IdInmueble = reader.GetInt32(nameof(Pago.IdInmueble)),
                                Direccion = reader.GetString(nameof(Pago.Direccion)),
                                DniInquilino = reader.GetString(nameof(Pago.DniInquilino)),
                                NombreInquilino = reader.GetString(nameof(Pago.NombreInquilino))
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public Pago? ObtenerPorReserva(int idReserva)
        {
            Pago? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT IdPago, IdReserva, Monto, Concepto, Estado, Fecha
                    FROM Pago
                    WHERE IdReserva = @idReserva
                    ORDER BY IdPago DESC
                    LIMIT 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Pago
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                IdReserva = reader.GetInt32(nameof(Pago.IdReserva)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                Concepto = reader.GetString(nameof(Pago.Concepto)),
                                Estado = reader.GetString(nameof(Pago.Estado)),
                                Fecha = DateOnly.FromDateTime(reader.GetDateTime(nameof(Pago.Fecha)))
                            };
                        }
                    }
                }
            }
            return p;
        }

        public IList<Pago> ObtenerListaPorReserva(int idReserva)
        {
            var result = new List<Pago>();
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("SELECT IdPago, IdReserva, Monto, Concepto, Estado, Fecha, UsuarioCreacionId, UsuarioAnulacionId FROM Pago WHERE IdReserva=@id ORDER BY Fecha DESC, IdPago DESC", connection);
            command.Parameters.AddWithValue("@id", idReserva); connection.Open(); using var reader = command.ExecuteReader();
            while (reader.Read()) result.Add(new Pago { IdPago = reader.GetInt32("IdPago"), IdReserva = reader.GetInt32("IdReserva"), Monto = reader.GetDecimal("Monto"), Concepto = reader.GetString("Concepto"), Estado = reader.GetString("Estado"), Fecha = DateOnly.FromDateTime(reader.GetDateTime("Fecha")), UsuarioCreacionId = reader["UsuarioCreacionId"] is DBNull ? null : reader.GetInt32("UsuarioCreacionId"), UsuarioAnulacionId = reader["UsuarioAnulacionId"] is DBNull ? null : reader.GetInt32("UsuarioAnulacionId") });
            return result;
        }

        public int ObtenerCantidad()
        {
            return ObtenerCantidad(false);
        }

        public int ObtenerCantidad(bool incluirInactivos)
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = incluirInactivos ? "SELECT COUNT(*) FROM Pago" : "SELECT COUNT(*) FROM Pago WHERE Estado <> 'Inactivo'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }


        public IList<Pago> ObtenerLista(int pagina, int tamanioPagina)
        {
            return ObtenerLista(pagina, tamanioPagina, false);
        }

        public IList<Pago> ObtenerLista(int pagina, int tamanioPagina, bool incluirInactivos)
        {
            var lista = new List<Pago>();
            int offset = (pagina - 1) * tamanioPagina;
            if (offset < 0) offset = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string filtroEstado = incluirInactivos ? "" : "WHERE p.Estado <> 'Inactivo'";
                string sql = $@"SELECT p.IdPago, p.IdReserva, p.Monto, p.Concepto, p.Estado, p.Fecha,
                CONCAT(io.Apellido, ', ', io.Nombre) AS NombreInquilino,
                io.Dni AS DniInquilino, ie.Direccion, ie.IdInmueble
                    FROM Pago p
                    INNER JOIN Reserva r ON p.IdReserva = r.IdReserva
                    INNER JOIN Inmueble ie ON r.IdInmueble = ie.IdInmueble
                    INNER JOIN Inquilino io ON r.IdInquilino = io.IdInquilino
                    {filtroEstado}
                    ORDER BY p.IdPago
                    LIMIT @limit OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@limit", tamanioPagina);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pago
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                IdReserva = reader.GetInt32(nameof(Pago.IdReserva)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                Concepto = reader.GetString(nameof(Pago.Concepto)),
                                Estado = reader.GetString(nameof(Pago.Estado)),
                                Fecha = DateOnly.FromDateTime(reader.GetDateTime(nameof(Pago.Fecha))),
                                IdInmueble = reader.GetInt32(nameof(Pago.IdInmueble)),
                                Direccion = reader.GetString(nameof(Pago.Direccion)),
                                DniInquilino = reader.GetString(nameof(Pago.DniInquilino)),
                                NombreInquilino = reader.GetString(nameof(Pago.NombreInquilino))
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }
    }
}