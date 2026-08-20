
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


    }
}