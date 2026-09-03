
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
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration): base(configuration)
        {
            
        }

       public int Alta(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Propietario 
                    (Nombre, Apellido, Dni, Telefono, Email, Clave)
                    VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@clave", p.Clave);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPropietario = res;
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
				string sql = "DELETE FROM Propietario WHERE IdPropietario = @id";
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

        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Propietario 
                    SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave
                    WHERE IdPropietario = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@clave", p.Clave);
                    command.Parameters.AddWithValue("@id", p.IdPropietario);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Propietario? ObtenerPorId(int id)
        {
            Propietario? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave FROM Propietario WHERE IdPropietario = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            p = new Propietario
                            {
                                IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono))) ? "" : reader.GetString(nameof(Propietario.Telefono)),
                                Email = reader.GetString(nameof(Propietario.Email)),
                                Clave = reader.GetString(nameof(Propietario.Clave))
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
                string sql = "SELECT COUNT(*) FROM Propietario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Propietario> ObtenerLista(int pagina, int tamanioPagina)
        {
            var lista = new List<Propietario>();
            int offset = (pagina - 1) * tamanioPagina;
            if (offset < 0) offset = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave 
                       FROM Propietario 
                       ORDER BY IdPropietario 
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
                            lista.Add(new Propietario
                            {
                                IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni)),
                                Telefono = reader.IsDBNull(reader.GetOrdinal(nameof(Propietario.Telefono))) ? "" : reader.GetString(nameof(Propietario.Telefono)),
                                Email = reader.GetString(nameof(Propietario.Email)),
                                Clave = reader.GetString(nameof(Propietario.Clave))
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public IList<Propietario> BuscarPorNombre(string Nombre)
        {
            throw new NotImplementedException();
        }


        public Propietario? ObtenerPorEmail(string Email)
        {
            throw new NotImplementedException();
        }
    }
}