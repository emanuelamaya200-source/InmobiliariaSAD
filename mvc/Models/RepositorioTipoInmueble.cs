

using System.Data;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_.Net_Core.Models

{
    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(tipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO tipoInmueble 
                    (Descripcion)
                    VALUES (@descripcion);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@descripcion", t.Descripcion);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    t.idTipoInmueble = res;
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
                string sql = "DELETE FROM tipoInmueble WHERE IdTipoInmueble = @id";
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

        public int Modificacion(tipoInmueble t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE tipoInmueble 
                    SET  Descripcion=@descripcion
                    WHERE IdTipoInmueble = @id";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", t.idTipoInmueble);
                    command.Parameters.AddWithValue("@descripcion", t.Descripcion);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            throw new NotImplementedException();
        }

        public IList<tipoInmueble> ObtenerLista(int pagina, int tamanioPagina)
        {
            var lista = new List<tipoInmueble>();
            int offset = (pagina - 1) * tamanioPagina;
            if (offset < 0) offset = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT IdTipoInmueble, Descripcion 
                       FROM tipoInmueble 
                       ORDER BY IdTipoInmueble
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
                            lista.Add(new tipoInmueble
                            {
                                idTipoInmueble = reader.GetInt32(nameof(tipoInmueble.idTipoInmueble)),
                                Descripcion = reader.GetString(nameof(tipoInmueble.Descripcion)),
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return lista;
        }

        public tipoInmueble? ObtenerPorId(int id)
        {
            tipoInmueble? t = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT IdTipoInmueble, Descripcion FROM tipoInmueble WHERE IdTipoInmueble = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            t = new tipoInmueble
                            {
                                idTipoInmueble = reader.GetInt32(nameof(tipoInmueble.idTipoInmueble)),
                                Descripcion = reader.GetString(nameof(tipoInmueble.Descripcion)),
                            };
                        }
                    }
                    connection.Close();
                }
            }
            return t;
        }

    }
}