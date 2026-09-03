using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioReserva : RepositorioBase, IRepositorioReserva
	{
		public RepositorioReserva(IConfiguration configuration) : base(configuration)
		{
		}

		public int Alta(Reserva p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Reservas (IdInmueble, IdInquilino, FechaDeEntrada, FechaDeSalida) 
                               VALUES (@IdInmueble, @IdInquilino, @FechaDeEntrada, @FechaDeSalida);
                               SELECT LAST_INSERT_ID();";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
					command.Parameters.AddWithValue("@IdInquilino", p.IdInquilino);
					command.Parameters.AddWithValue("@FechaDeEntrada", p.FechaDeEntrada);
					command.Parameters.AddWithValue("@FechaDeSalida", p.FechaDeSalida);

					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.IdReserva = res;
				}
			}
			return res;
		}

		public int Baja(int id)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"DELETE FROM Reservas WHERE IdReserva = @id";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
				}
			}
			return res;
		}

		public int Modificacion(Reserva p)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"UPDATE Reservas 
                               SET IdInmueble = @IdInmueble, 
                                   IdInquilino = @IdInquilino, 
                                   FechaDeEntrada = @FechaDeEntrada, 
                                   FechaDeSalida = @FechaDeSalida 
                               WHERE IdReserva = @IdReserva";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
					command.Parameters.AddWithValue("@IdInquilino", p.IdInquilino);
					command.Parameters.AddWithValue("@FechaDeEntrada", p.FechaDeEntrada);
					command.Parameters.AddWithValue("@FechaDeSalida", p.FechaDeSalida);
					command.Parameters.AddWithValue("@IdReserva", p.IdReserva);

					connection.Open();
					res = command.ExecuteNonQuery();
				}
			}
			return res;
		}

		public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Reserva> res = new List<Reserva>();
			int offset = (paginaNro - 1) * tamPagina;

			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT IdReserva, IdInmueble, IdInquilino, FechaDeEntrada, FechaDeSalida 
                               FROM Reservas 
                               LIMIT @tamPagina OFFSET @offset";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@tamPagina", tamPagina);
					command.Parameters.AddWithValue("@offset", offset);

					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Reserva
							{
								IdReserva = reader.GetInt32(nameof(Reserva.IdReserva)),
								IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
								IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
								FechaDeEntrada = reader.GetDateTime(nameof(Reserva.FechaDeEntrada)),
								FechaDeSalida = reader.GetDateTime(nameof(Reserva.FechaDeSalida))
							});
						}
					}
				}
			}
			return res;
		}

		public int ObtenerCantidad()
		{
			int res = 0;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT COUNT(*) FROM Reservas";
				using (var command = new MySqlCommand(sql, connection))
				{
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
				}
			}
			return res;
		}

		public Reserva? ObtenerPorId(int id)
		{
			Reserva? res = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT IdReserva, IdInmueble, IdInquilino, FechaDeEntrada, FechaDeSalida 
                               FROM Reservas 
                               WHERE IdReserva = @id";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							res = new Reserva
							{
								IdReserva = reader.GetInt32(nameof(Reserva.IdReserva)),
								IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
								IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
								FechaDeEntrada = reader.GetDateTime(nameof(Reserva.FechaDeEntrada)),
								FechaDeSalida = reader.GetDateTime(nameof(Reserva.FechaDeSalida))
							};
						}
					}
				}
			}
			return res;
		}

		public IList<Inmueble> VerificarDisponibilidad(DateTime inicioFecha, DateTime finFecha)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT i.Id, i.Direccion, i.Cupo, i.PrecioPorDia, i.PorcentajeReserva, i.PropietarioId, i.Habilitado
                               FROM Inmuebles i
                               WHERE i.Habilitado = 1 
                               AND i.Id NOT IN (
                                   SELECT r.IdInmueble 
                                   FROM Reservas r 
                                   WHERE r.FechaDeEntrada < @finFecha AND r.FechaDeSalida > @inicioFecha
                               )";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@inicioFecha", inicioFecha);
					command.Parameters.AddWithValue("@finFecha", finFecha);

					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(new Inmueble
							{
								Id = reader.GetInt32("Id"),
								Direccion = reader.GetString("Direccion"),
								Cupo = reader.GetInt32("Cupo"),
								PrecioPorDia = reader.GetDecimal("PrecioPorDia"),
								PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
								PropietarioId = reader.GetInt32("PropietarioId"),
								Habilitado = reader.GetBoolean("Habilitado")
							});
						}
					}
				}
			}
			return res;
		}

		public decimal CalcularMulta(int idReserva, DateTime finFecha)
		{
			var reserva = ObtenerPorId(idReserva);
			if (reserva == null)
			{
				throw new Exception("Reserva no encontrada");
			}

			int diasTotales = (reserva.FechaDeSalida - reserva.FechaDeEntrada).Days;
			int diasTranscurridos = (finFecha - reserva.FechaDeEntrada).Days;
			int diasRestantes = (reserva.FechaDeSalida - finFecha).Days;

			if (diasRestantes <= 0)
			{
				return 0;
			}

			decimal porcentajeMulta = (diasTranscurridos < (diasTotales / 2.0)) ? 0.50m : 0.25m;

			string sqlPrecio = "SELECT PrecioPorDia FROM Inmuebles WHERE Id = @IdInmueble";
			decimal precioPorDia = 0;

			using (var connection = new MySqlConnection(connectionString))
			{
				using (var command = new MySqlCommand(sqlPrecio, connection))
				{
					command.Parameters.AddWithValue("@IdInmueble", reserva.IdInmueble);
					connection.Open();
					precioPorDia = Convert.ToDecimal(command.ExecuteScalar());
				}
			}

			return diasRestantes * precioPorDia * porcentajeMulta;
		}

		public bool TerminarReservaAnticipada(int idReserva, DateTime nuevoFinFecha)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"UPDATE Reservas 
                               SET FechaDeSalida = @nuevoFinFecha 
                               WHERE IdReserva = @idReserva";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@nuevoFinFecha", nuevoFinFecha);
					command.Parameters.AddWithValue("@idReserva", idReserva);

					connection.Open();
					res = command.ExecuteNonQuery();
				}
			}
			return res > 0;
		}

		public Reserva RenovarReserva(int idReserva, DateTime nuevoFinFecha, decimal nuevoPrecio)
		{
			var reservaOriginal = ObtenerPorId(idReserva);
			if (reservaOriginal == null)
			{
				throw new Exception("Reserva original no encontrada");
			}

			Reserva nuevaReserva = new Reserva
			{
				IdInmueble = reservaOriginal.IdInmueble,
				IdInquilino = reservaOriginal.IdInquilino,
				FechaDeEntrada = reservaOriginal.FechaDeSalida,
				FechaDeSalida = nuevoFinFecha
			};

			Alta(nuevaReserva);
			return nuevaReserva;
		}
	}
}