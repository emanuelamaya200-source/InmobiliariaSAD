
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
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino (IConfiguration configuration): base(configuration)
        {
            
        }

       public int Alta(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Inquilino
                    (Nombre, Apellido, Dni, Telefono, Email)
                    VALUES (@nombre, @apellido, @dni, @telefono, @email);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.IdInquilino = res;
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
                string sql = "DELETE FROM Inquilino WHERE IdInquilino = @id";
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

        public int Modificacion(Inquilino i)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Inquilino 
                    SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email
                    WHERE IdInquilino = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@id", i.IdInquilino);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? i = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email FROM Inquilino WHERE IdInquilino = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            i = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Telefono))) ? "" : reader.GetString(nameof(Inquilino.Telefono)),
                                Email = reader.GetString(nameof(Inquilino.Email))
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return i;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Inquilino";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inquilino> ObtenerLista(int pagina, int tamanioPagina)
        {
            var lista = new List<Inquilino>();
            int offset = (pagina - 1) * tamanioPagina;
            if (offset < 0) offset = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email 
                       FROM Inquilino 
                       ORDER BY IdInquilino 
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
                            lista.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.Telefono))) ? "" : reader.GetString(nameof(Inquilino.Telefono)),
                                Email = reader.GetString(nameof(Inquilino.Email))
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