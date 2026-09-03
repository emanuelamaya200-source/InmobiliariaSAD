using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
	{
		public RepositorioInmueble(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Inmueble entidad)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Inmueble
					(Direccion, Cupo, PrecioPorDia, PorcentajeReserva, Latitud, Longitud, PropietarioId)
					VALUES (@direccion, @cupo, @precioPorDia, @porcentajeReserva, @latitud, @longitud, @propietarioId);
					SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", entidad.Direccion == null? DBNull.Value : entidad.Direccion);
					command.Parameters.AddWithValue("@cupo", entidad.Cupo);
					command.Parameters.AddWithValue("@precioPorDia", entidad.PrecioPorDia);
					command.Parameters.AddWithValue("@porcentajeReserva", entidad.PorcentajeReserva);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.Id = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"DELETE FROM Inmueble WHERE IdInmueble = @id";
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
		public int Modificacion(Inmueble entidad)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"
					UPDATE Inmueble SET
						Direccion=@direccion, Cupo=@cupo, PrecioPorDia=@precioPorDia, PorcentajeReserva=@porcentajeReserva, 
						Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId
					WHERE IdInmueble = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@cupo", entidad.Cupo);
					command.Parameters.AddWithValue("@precioPorDia", entidad.PrecioPorDia);
					command.Parameters.AddWithValue("@porcentajeReserva", entidad.PorcentajeReserva);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					command.Parameters.AddWithValue("@id", entidad.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
        }
        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = $@"SELECT i.IdInmueble AS {nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
					i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)},
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)}, i.{nameof(Inmueble.Portada)},
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}, p.{nameof(Propietario.Dni)}
					FROM Inmueble i INNER JOIN Propietario p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)}
					ORDER BY i.IdInmueble
					LIMIT {(paginaNro - 1) * tamPagina}, {tamPagina}
				";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value? "" : reader.GetString(nameof(Inmueble.Direccion)),
							Portada = reader[nameof(Inmueble.Portada)] == DBNull.Value? null : reader.GetString(nameof(Inmueble.Portada)),
							Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
							PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
							PorcentajeReserva = reader.GetDecimal(nameof(Inmueble.PorcentajeReserva)),
							Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
							Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
							PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								//Dni = reader.GetString(nameof(Propietario.Dni)),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public int ObtenerCantidad()
		{
			int res = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT COUNT(Id)
					FROM Inmueble
				";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					connection.Close();
				}
			}
			return res;
		}
public Inmueble? ObtenerPorId(int id)
		{
			Inmueble? entidad = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT i.IdInmueble AS {nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
					i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)},
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)}, i.{nameof(Inmueble.Portada)},
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}
					FROM Inmueble i JOIN Propietario p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)}
					WHERE i.IdInmueble = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value? "" : reader.GetString(nameof(Inmueble.Direccion)),
							Portada = reader[nameof(Inmueble.Portada)] == DBNull.Value? null : reader.GetString(nameof(Inmueble.Portada)),
							Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
							PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
							PorcentajeReserva = reader.GetDecimal(nameof(Inmueble.PorcentajeReserva)),
							Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
							Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
							PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								//Dni = reader.GetString(nameof(Propietario.Dni)),
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}

		public IList<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT i.IdInmueble AS {nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
					i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)},
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)},
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}
					FROM Inmueble i JOIN Propietario p ON i.IdInmueble = p.IdPropietario
					WHERE i.IdInmueble = @idPropietario";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", MySqlDbType.Int32).Value = idPropietario;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						var entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value? "" : reader.GetString(nameof(Inmueble.Direccion)),
							Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
							PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
							PorcentajeReserva = reader.GetDecimal(nameof(Inmueble.PorcentajeReserva)),
							Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
							Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
							PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
	
    	}

    public int ModificarPortada(int id, string url)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"
					UPDATE Inmueble SET
					Portada=@portada
					WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@portada", String.IsNullOrEmpty(url) ? DBNull.Value : url);
					command.Parameters.AddWithValue("@id", id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

	}
}
