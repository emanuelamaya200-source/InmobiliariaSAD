using MySql.Data.MySqlClient;
using System.Security.Claims;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioAuditoria : RepositorioBase, IRepositorioAuditoria
    {
        public RepositorioAuditoria(IConfiguration configuration) : base(configuration)
        {
        }

        public void Registrar(ClaimsPrincipal usuario, string entidad, int entidadId, string accion, string? detalle = null)
        {
            var usuarioId = int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
            Alta(new Auditoria
            {
                Entidad = entidad,
                EntidadId = entidadId,
                Accion = accion,
                UsuarioId = usuarioId > 0 ? id : null,
                UsuarioNombre = usuario.FindFirstValue("FullName") ?? usuario.Identity?.Name,
                Fecha = DateTime.Now,
                Detalle = detalle
            });
        }

        public int Alta(Auditoria p)
        {
            using var connection = new MySqlConnection(connectionString);
            const string sql = @"INSERT INTO Auditoria (Entidad, EntidadId, Accion, UsuarioId, UsuarioNombre, Fecha, Detalle)
                VALUES (@entidad, @entidadId, @accion, @usuarioId, @usuarioNombre, @fecha, @detalle);";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@entidad", p.Entidad);
            command.Parameters.AddWithValue("@entidadId", p.EntidadId);
            command.Parameters.AddWithValue("@accion", p.Accion);
            command.Parameters.AddWithValue("@usuarioId", (object?)p.UsuarioId ?? DBNull.Value);
            command.Parameters.AddWithValue("@usuarioNombre", (object?)p.UsuarioNombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@fecha", p.Fecha);
            command.Parameters.AddWithValue("@detalle", (object?)p.Detalle ?? DBNull.Value);
            connection.Open();
            command.ExecuteNonQuery();
            p.Id = (int)command.LastInsertedId;
            return p.Id;
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("DELETE FROM Auditoria WHERE Id=@id", connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public int Modificacion(Auditoria p)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(@"UPDATE Auditoria SET Entidad=@entidad, EntidadId=@entidadId, Accion=@accion,
                UsuarioId=@usuarioId, UsuarioNombre=@usuarioNombre, Fecha=@fecha, Detalle=@detalle WHERE Id=@id", connection);
            command.Parameters.AddWithValue("@entidad", p.Entidad);
            command.Parameters.AddWithValue("@entidadId", p.EntidadId);
            command.Parameters.AddWithValue("@accion", p.Accion);
            command.Parameters.AddWithValue("@usuarioId", (object?)p.UsuarioId ?? DBNull.Value);
            command.Parameters.AddWithValue("@usuarioNombre", (object?)p.UsuarioNombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@fecha", p.Fecha);
            command.Parameters.AddWithValue("@detalle", (object?)p.Detalle ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", p.Id);
            connection.Open();
            return command.ExecuteNonQuery();
        }

        public IList<Auditoria> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var lista = new List<Auditoria>();
            const string sql = @"SELECT Id, Entidad, EntidadId, Accion, UsuarioId, UsuarioNombre, Fecha, Detalle FROM Auditoria ORDER BY Fecha DESC LIMIT @offset, @limit";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
            command.Parameters.AddWithValue("@limit", tamPagina);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Auditoria
                {
                    Id = reader.GetInt32("Id"),
                    Entidad = reader.GetString("Entidad"),
                    EntidadId = reader.GetInt32("EntidadId"),
                    Accion = reader.GetString("Accion"),
                    UsuarioId = reader["UsuarioId"] is DBNull ? null : reader.GetInt32("UsuarioId"),
                    UsuarioNombre = reader["UsuarioNombre"] is DBNull ? null : reader.GetString("UsuarioNombre"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Detalle = reader["Detalle"] is DBNull ? null : reader.GetString("Detalle")
                });
            }
            return lista;
        }

        public int ObtenerCantidad()
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("SELECT COUNT(*) FROM Auditoria", connection);
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public Auditoria? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("SELECT Id, Entidad, EntidadId, Accion, UsuarioId, UsuarioNombre, Fecha, Detalle FROM Auditoria WHERE Id=@id", connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            return new Auditoria
            {
                Id = reader.GetInt32("Id"),
                Entidad = reader.GetString("Entidad"),
                EntidadId = reader.GetInt32("EntidadId"),
                Accion = reader.GetString("Accion"),
                UsuarioId = reader["UsuarioId"] is DBNull ? null : reader.GetInt32("UsuarioId"),
                UsuarioNombre = reader["UsuarioNombre"] is DBNull ? null : reader.GetString("UsuarioNombre"),
                Fecha = reader.GetDateTime("Fecha"),
                Detalle = reader["Detalle"] is DBNull ? null : reader.GetString("Detalle")
            };
        }
    }
}
