
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
        public RepositorioPago (IConfiguration configuration): base(configuration)
        {
            
        }

       public int Alta(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Pago
                    (IdReserva, Monto, Concepto, Estado, Fecha)
                    VALUES (@idreserva, @monto, @concepto, @estado, @fecha);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idreserva", p.IdReserva);
                    command.Parameters.AddWithValue("@monto", p.Monto);
                    command.Parameters.AddWithValue("@concepto", p.Concepto);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    command.Parameters.AddWithValue("@fecha", p.Fecha.ToDateTime(TimeOnly.MinValue));
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
                string sql = "SELECT IdPago, IdReserva, Monto, Concepto, Estado, Fecha FROM Pago WHERE IdPago = @id";
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
                                Fecha = DateOnly.FromDateTime(reader.GetDateTime(nameof(Pago.Fecha)))
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

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Pago WHERE Estado <> 'Inactivo'";
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
                string sql = $@"SELECT p.IdPago, p.IdReserva, p.Monto, p.Concepto, p.Estado, p.Fecha
                    FROM Pago p
                    INNER JOIN Reserva r ON p.IdReserva = r.IdReserva
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
                                Fecha = DateOnly.FromDateTime(reader.GetDateTime(nameof(Pago.Fecha)))
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }
    }}