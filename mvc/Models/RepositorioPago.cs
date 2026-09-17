
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
                    (IdReserva, Monto)
                    VALUES (@idreserva, @monto);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idreserva", p.IdReserva);
                    command.Parameters.AddWithValue("@monto", p.Monto);
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
                string sql = "DELETE FROM Pago WHERE IdPago = @id";
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

        public int Modificacion(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Pago 
                    SET IdReserva=@idreserva, Monto=@monto
                    WHERE IdPago = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idreserva", p.IdReserva);
                    command.Parameters.AddWithValue("@monto", p.Monto);
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
                string sql = "SELECT IdPago, IdReserva, Monto FROM Pago WHERE IdPago = @id";
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
                                Monto = reader.GetDecimal(nameof(Pago.Monto))
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Pago";
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
            var lista = new List<Pago>();
            int offset = (pagina - 1) * tamanioPagina;
            if (offset < 0) offset = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT IdPago, IdReserva, Monto 
                       FROM Pago 
                       ORDER BY IdPago 
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
                                Monto = reader.GetDecimal(nameof(Pago.Monto))
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }
    }}