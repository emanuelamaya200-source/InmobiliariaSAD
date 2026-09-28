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
					(Direccion, Cupo, PrecioPorDia, PorcentajeReserva, Latitud, Longitud, PropietarioId, IdTipoInmueble)
					VALUES (@direccion, @cupo, @precioPorDia, @porcentajeReserva, @latitud, @longitud, @propietarioId, @IdTipoInmueble);
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
					command.Parameters.AddWithValue("@IdTipoInmueble", entidad.IdTipoInmueble);
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
						Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId, IdTipoInmueble=@IdTipoInmueble
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
					command.Parameters.AddWithValue("@IdTipoInmueble", entidad.IdTipoInmueble);
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
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)},i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Portada)},
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}, p.{nameof(Propietario.Dni)}, t.{nameof(tipoInmueble.Descripcion)} 
					FROM Inmueble i INNER JOIN Propietario p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)} 
					INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.IdTipoInmueble
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
							IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								//Dni = reader.GetString(nameof(Propietario.Dni)),
							},
							Tipo = new tipoInmueble
							{
								idTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
								Descripcion = reader.GetString(nameof(tipoInmueble.Descripcion))
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
					SELECT COUNT(IdInmueble)
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

		public IList<Inmueble> ObtenerInformeReservas(bool sinReservas, int dias, int pagina, int tamPagina, out int total)
		{
			var lista = new List<Inmueble>();
			int offset = Math.Max(0, pagina - 1) * tamPagina;
			string filtroFecha = sinReservas
				? "r.FechaDeEntrada < DATE_ADD(CURDATE(), INTERVAL 1 DAY) AND COALESCE(r.FechaFinEfectiva, r.FechaDeSalida) > DATE_SUB(CURDATE(), INTERVAL @dias DAY)"
				: "r.FechaDeEntrada >= DATE_SUB(CURDATE(), INTERVAL @dias DAY) AND r.FechaDeEntrada < DATE_ADD(CURDATE(), INTERVAL 1 DAY)";
			string having = sinReservas ? "COUNT(r.IdReserva) = 0" : "COUNT(r.IdReserva) > 0";
			string desde = $@"FROM Inmueble i
				LEFT JOIN reserva r ON r.IdInmueble = i.IdInmueble
					AND r.Estado <> @cancelado AND ({filtroFecha})";

			using var connection = new MySqlConnection(connectionString);
			connection.Open();

			string sqlTotal = $@"SELECT COUNT(*) FROM (
				SELECT i.IdInmueble {desde}
				GROUP BY i.IdInmueble, i.Direccion
				HAVING {having}
			) informe";
			using (var totalCommand = new MySqlCommand(sqlTotal, connection))
			{
				totalCommand.Parameters.Add("@cancelado", MySqlDbType.VarChar).Value = "Cancelado";
				totalCommand.Parameters.Add("@dias", MySqlDbType.Int32).Value = dias;
				total = Convert.ToInt32(totalCommand.ExecuteScalar());
			}

			string orden = sinReservas ? "i.IdInmueble" : "CantidadReservas DESC, i.IdInmueble";
			string sql = $@"SELECT i.IdInmueble AS {nameof(Inmueble.Id)},
					i.Direccion AS {nameof(Inmueble.Direccion)},
					COUNT(r.IdReserva) AS {nameof(Inmueble.CantidadReservas)}
				{desde}
				GROUP BY i.IdInmueble, i.Direccion
				HAVING {having}
				ORDER BY {orden}
				LIMIT @limit OFFSET @offset";
			using var command = new MySqlCommand(sql, connection);
			command.Parameters.Add("@cancelado", MySqlDbType.VarChar).Value = "Cancelado";
			command.Parameters.Add("@dias", MySqlDbType.Int32).Value = dias;
			command.Parameters.Add("@limit", MySqlDbType.Int32).Value = tamPagina;
			command.Parameters.Add("@offset", MySqlDbType.Int32).Value = offset;
			using var reader = command.ExecuteReader();
			while (reader.Read())
			{
				lista.Add(new Inmueble
				{
					Id = reader.GetInt32(nameof(Inmueble.Id)),
					Direccion = reader.GetString(nameof(Inmueble.Direccion)),
					CantidadReservas = Convert.ToInt32(reader[nameof(Inmueble.CantidadReservas)])
				});
			}
			return lista;
		}

public Inmueble? ObtenerPorId(int id)
		{
			Inmueble? entidad = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT i.IdInmueble AS {nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
					i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)},
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)},i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Portada)},
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}, t.{nameof(tipoInmueble.Descripcion)} 
					FROM Inmueble i JOIN Propietario p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)}
					INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.IdTipoInmueble
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
							IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								//Dni = reader.GetString(nameof(Propietario.Dni)),
							},
							Tipo = new tipoInmueble
							{
								idTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
								Descripcion = reader.GetString(nameof(tipoInmueble.Descripcion))
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
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)},i.{nameof(Inmueble.IdTipoInmueble)},
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}
					FROM Inmueble i JOIN Propietario p ON i.PropietarioId = p.IdPropietario
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
							IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
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

        public decimal CalcularPrecioEstadia(int id, DateTime inicioFecha, DateTime finFecha)
        {
            var inmueble = ObtenerPorId(id);
            if (inmueble is null)
            {
                throw new Exception("Inmueble No Encontrado");
            }

            int cantidadDeDias = (finFecha.Date - inicioFecha.Date).Days;
            if (cantidadDeDias <= 0)
            {
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            return cantidadDeDias * inmueble.PrecioPorDia;
		}

		public IList<Inmueble> BuscarPorTipo(string busqueda)
		{
			var lista = new List<Inmueble>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $@"SELECT i.IdInmueble AS {nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
                i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)},
                i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Portada)},
                p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}, t.{nameof(tipoInmueble.Descripcion)} 
                FROM Inmueble i 
                INNER JOIN Propietario p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)} 
                INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.IdTipoInmueble
                WHERE i.Direccion LIKE @busqueda OR t.Descripcion LIKE @busqueda
                ORDER BY i.IdInmueble";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							lista.Add(new Inmueble
							{
								Id = reader.GetInt32(nameof(Inmueble.Id)),
								Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Direccion)),
								Portada = reader[nameof(Inmueble.Portada)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Portada)),
								Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
								PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
								PorcentajeReserva = reader.GetDecimal(nameof(Inmueble.PorcentajeReserva)),
								Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
								Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
								PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
								Duenio = new Propietario
								{
									IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
									Nombre = reader.GetString(nameof(Propietario.Nombre)),
									Apellido = reader.GetString(nameof(Propietario.Apellido))
								},
								Tipo = new tipoInmueble
								{
									idTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
									Descripcion = reader.GetString(nameof(tipoInmueble.Descripcion))
								}
							});
						}
					}
					connection.Close();
				}
			}
			return lista;
		}

        public IList<Inmueble> BuscarPorTipo(string tipo, int pagina, int tamPagina)
        {
            throw new NotImplementedException();
        }

        public int ObtenerCantidadFiltrada(string? busqueda, int? disponibilidad)
		{
			int res = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT COUNT(i.IdInmueble)
					FROM Inmueble i 
					INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario
					INNER JOIN TipoInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
					WHERE 1=1";
				if (!string.IsNullOrWhiteSpace(busqueda))
					sql += " AND (i.Direccion LIKE @busqueda OR t.Descripcion LIKE @busqueda)";
				if (disponibilidad.HasValue)
					sql += " AND i.Disponible = @disponibilidad";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					if (!string.IsNullOrWhiteSpace(busqueda))
						command.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
					if (disponibilidad.HasValue)
						command.Parameters.AddWithValue("@disponibilidad", disponibilidad.Value);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					connection.Close();
				}
			}
			return res;
		}
		public IList<Inmueble> ObtenerListaFiltrada(string? busqueda, int? disponibilidad, int pagina, int tamPagina)
		{
			var lista = new List<Inmueble>();
			int offset = (pagina - 1) * tamPagina;
			if (offset < 0) offset = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $@"SELECT i.IdInmueble AS {nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Cupo)},
					i.{nameof(Inmueble.PrecioPorDia)}, i.{nameof(Inmueble.PorcentajeReserva)},
					i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)}, i.{nameof(Inmueble.IdTipoInmueble)}, i.{nameof(Inmueble.Portada)},
					i.Disponible AS Habilitado,
					p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}, t.{nameof(tipoInmueble.Descripcion)} 
					FROM Inmueble i 
					INNER JOIN Propietario p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)} 
					INNER JOIN TipoInmueble t ON i.{nameof(Inmueble.IdTipoInmueble)} = t.IdTipoInmueble
					WHERE 1=1";
				if (!string.IsNullOrWhiteSpace(busqueda))
					sql += " AND (i.Direccion LIKE @busqueda OR t.Descripcion LIKE @busqueda)";
				if (disponibilidad.HasValue)
					sql += " AND i.Disponible = @disponibilidad";
				sql += " ORDER BY i.IdInmueble LIMIT @limit OFFSET @offset";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					if (!string.IsNullOrWhiteSpace(busqueda))
						command.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
					if (disponibilidad.HasValue)
						command.Parameters.AddWithValue("@disponibilidad", disponibilidad.Value);
					command.Parameters.AddWithValue("@limit", tamPagina);
					command.Parameters.AddWithValue("@offset", offset);
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							lista.Add(new Inmueble
							{
								Id = reader.GetInt32(nameof(Inmueble.Id)),
								Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Direccion)),
								Portada = reader[nameof(Inmueble.Portada)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Portada)),
								Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
								PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
								PorcentajeReserva = reader.GetDecimal(nameof(Inmueble.PorcentajeReserva)),
								Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
								Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
								PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
								IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
								Habilitado = reader.GetBoolean("Habilitado"),
								Duenio = new Propietario
								{
									IdPropietario = reader.GetInt32(nameof(Inmueble.PropietarioId)),
									Nombre = reader.GetString(nameof(Propietario.Nombre)),
									Apellido = reader.GetString(nameof(Propietario.Apellido))
								},
								Tipo = new tipoInmueble
								{
									idTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
									Descripcion = reader.GetString(nameof(tipoInmueble.Descripcion))
								}
							});
						}
					}
					connection.Close();
				}
			}
			return lista;
		}

	public IList<Inmueble> MasReservados(int dias)
		{
		IList<Inmueble> model = new List<Inmueble>();
		//     
		using (var connection = new MySqlConnection(connectionString))
		{
			if(!(dias==0))
				{
				//@ permite saltos en linea
				string sql = $@"SELECT i.IdInmueble AS {nameof(Inmueble.Id)} , i.Direccion, Count(*) AS CantidadReservas
				From Inmueble i
				JOIN reserva r ON r.IdInmueble = i.IdInmueble
				Where r.FechaDeEntrada >= DATE_SUB()
				";	
				}
				else
				{
					
				}
				// si la persona introduce una cantidad de dias se muestra esa cantidad
		}
		return model;                  
              
		}
	}
}