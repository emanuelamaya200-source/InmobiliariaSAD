using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_.Net_Core.Models
{
    public class RepositorioReserva : RepositorioBase, IRepositorioReserva
    {
        public RepositorioReserva(IConfiguration configuration) : base(configuration) { }

        private static Reserva Map(MySqlDataReader reader)
        {
            var entrada = reader.GetDateTime("FechaDeEntrada");
            var salida = reader.GetDateTime("FechaDeSalida");
            var montoDiario = reader.GetDecimal("MontoDiario");

            int cantidadDias = Math.Max(
                1,
                (salida.Date - entrada.Date).Days
            );

            return new Reserva
            {
                IdReserva = reader.GetInt32("IdReserva"),
                IdInmueble = reader.GetInt32("IdInmueble"),
                IdInquilino = reader.GetInt32("IdInquilino"),

                FechaDeEntrada = entrada,
                FechaDeSalida = salida,

                Estado = reader.GetString("Estado"),

                MontoDiario = montoDiario,

                MontoTotal = (int)(cantidadDias * montoDiario),

                FechaFinEfectiva = reader["FechaFinEfectiva"] is DBNull
                    ? null
                    : reader.GetDateTime("FechaFinEfectiva"),

                UsuarioCreacionId = reader["UsuarioCreacionId"] is DBNull
                    ? null
                    : reader.GetInt32("UsuarioCreacionId"),

                UsuarioFinalizacionId = reader["UsuarioFinalizacionId"] is DBNull
                    ? null
                    : reader.GetInt32("UsuarioFinalizacionId")
            };
        }

        public int Alta(Reserva p)
        {
            const string sql = @"INSERT INTO reserva (IdInmueble, IdInquilino, FechaDeEntrada, FechaDeSalida, Estado, MontoDiario, UsuarioCreacionId)
                VALUES (@inmueble, @inquilino, @entrada, @salida, 'Activo', @monto, @usuario);";
            using var connection = new MySqlConnection(connectionString); using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@inmueble", p.IdInmueble); command.Parameters.AddWithValue("@inquilino", p.IdInquilino); command.Parameters.AddWithValue("@entrada", p.FechaDeEntrada); command.Parameters.AddWithValue("@salida", p.FechaDeSalida); command.Parameters.AddWithValue("@monto", p.MontoDiario); command.Parameters.AddWithValue("@usuario", (object?)p.UsuarioCreacionId ?? DBNull.Value);
            connection.Open(); command.ExecuteNonQuery(); p.IdReserva = (int)command.LastInsertedId; return p.IdReserva;
        }

        public int Baja(int id) { using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand("UPDATE reserva SET Estado='Cancelado' WHERE IdReserva=@id", c); q.Parameters.AddWithValue("@id", id); c.Open(); return q.ExecuteNonQuery(); }

        public int Cancelar(int id, int usuarioId) { using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand("UPDATE reserva SET Estado='Cancelado', UsuarioFinalizacionId=@usuario WHERE IdReserva=@id", c); q.Parameters.AddWithValue("@id", id); q.Parameters.AddWithValue("@usuario", usuarioId); c.Open(); return q.ExecuteNonQuery(); }

        public int Modificacion(Reserva p)
        {
            const string sql = "UPDATE reserva SET IdInmueble=@inmueble, IdInquilino=@inquilino, FechaDeEntrada=@entrada, FechaDeSalida=@salida WHERE IdReserva=@id";
            using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand(sql, c); q.Parameters.AddWithValue("@inmueble", p.IdInmueble); q.Parameters.AddWithValue("@inquilino", p.IdInquilino); q.Parameters.AddWithValue("@entrada", p.FechaDeEntrada); q.Parameters.AddWithValue("@salida", p.FechaDeSalida); q.Parameters.AddWithValue("@id", p.IdReserva); c.Open(); return q.ExecuteNonQuery();
        }

        public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            var reservas = ObtenerPorRango(null, null, null);
            return reservas.Skip(Math.Max(0, paginaNro - 1) * tamPagina).Take(tamPagina).ToList();
        }

        public IList<Reserva> ObtenerPorRango(DateTime? inicio, DateTime? fin, int? cupo)
        {
            var result = new List<Reserva>();
                            const string sql = @"SELECT r.*,
                                i.Direccion AS NombreInmueble,
                                CONCAT(inqui.Nombre, ' ', inqui.Apellido) AS NombreInquilino
                FROM reserva r
                INNER JOIN inmueble i ON r.IdInmueble = i.IdInmueble
                                INNER JOIN inquilino inqui ON r.IdInquilino = inqui.IdInquilino";
            using var c = new MySqlConnection(connectionString);
            using var q = new MySqlCommand(sql, c);
            c.Open();
            using var reader = q.ExecuteReader();
            while (reader.Read())
            {
                var reserva = Map(reader);
                reserva.NombreInmueble = reader.GetString("NombreInmueble");
                reserva.NombreInquilino = reader.GetString("NombreInquilino");
                result.Add(reserva);
            }
            return result;
        }

        public int ObtenerCantidad() { using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand("SELECT COUNT(*) FROM reserva", c); c.Open(); return Convert.ToInt32(q.ExecuteScalar()); }

        public Reserva? ObtenerPorId(int id)
        {
            using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand("SELECT IdReserva, IdInmueble, IdInquilino, FechaDeEntrada, FechaDeSalida, Estado, MontoDiario, FechaFinEfectiva, UsuarioCreacionId, UsuarioFinalizacionId FROM reserva WHERE IdReserva=@id", c); q.Parameters.AddWithValue("@id", id); c.Open(); using var reader = q.ExecuteReader(); return reader.Read() ? Map(reader) : null;
        }

        public IList<Inmueble> VerificarDisponibilidad(DateTime inicioFecha, DateTime finFecha, int cupo = 0)
        {
            var result = new List<Inmueble>();
            const string sql = @"SELECT i.IdInmueble AS Id, i.Direccion, i.Cupo, i.PrecioPorDia, i.PorcentajeReserva, i.PropietarioId, i.Portada FROM inmueble i WHERE i.Disponible=1 AND i.Cupo>=@cupo AND NOT EXISTS (SELECT 1 FROM reserva r WHERE r.IdInmueble=i.IdInmueble AND r.Estado='Activo' AND r.FechaDeEntrada<@fin AND COALESCE(r.FechaFinEfectiva,r.FechaDeSalida)>@inicio)";
            using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand(sql, c); q.Parameters.AddWithValue("@inicio", inicioFecha); q.Parameters.AddWithValue("@fin", finFecha); q.Parameters.AddWithValue("@cupo", cupo); c.Open(); using var reader = q.ExecuteReader(); while (reader.Read()) result.Add(new Inmueble { Id = reader.GetInt32("Id"), Direccion = reader.GetString("Direccion"), Cupo = reader.GetInt32("Cupo"), PrecioPorDia = reader.GetDecimal("PrecioPorDia"), PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"), PropietarioId = reader.GetInt32("PropietarioId"), Portada = reader["Portada"] as string }); return result;
        }

        public bool ExisteSolapamiento(int idInmueble, DateTime entrada, DateTime salida, int? idReservaExcluir = null)
        {
            const string sql = "SELECT COUNT(*) FROM reserva WHERE IdInmueble=@inmueble AND Estado='Activo' AND (@excluir IS NULL OR IdReserva<>@excluir) AND FechaDeEntrada<@salida AND COALESCE(FechaFinEfectiva,FechaDeSalida)>@entrada";
            using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand(sql, c); q.Parameters.AddWithValue("@inmueble", idInmueble); q.Parameters.AddWithValue("@entrada", entrada); q.Parameters.AddWithValue("@salida", salida); q.Parameters.AddWithValue("@excluir", (object?)idReservaExcluir ?? DBNull.Value); c.Open(); return Convert.ToInt32(q.ExecuteScalar()) > 0;
        }

        public decimal CalcularMulta(int idReserva, DateTime finFecha)
        {
            var reserva = ObtenerPorId(idReserva);

            if (reserva == null)
                throw new InvalidOperationException("La reserva no existe.");

            if (finFecha <= reserva.FechaDeEntrada)
                throw new InvalidOperationException("La fecha de finalización no es válida.");

            if (finFecha >= reserva.FechaDeSalida)
                return 0;


            decimal diasTotales =
                (decimal)(reserva.FechaDeSalida.Date - reserva.FechaDeEntrada.Date).TotalDays;

            decimal diasTranscurridos =
                (decimal)(finFecha.Date - reserva.FechaDeEntrada.Date).TotalDays;


            decimal diasRestantes = diasTotales - diasTranscurridos;

            if (diasRestantes <= 0)
                return 0;


            decimal porcentajeMulta;

            if (diasTranscurridos < diasTotales / 2m)
            {
                porcentajeMulta = 0.50m;
            }
            else
            {
                porcentajeMulta = 0.25m;
            }

            decimal alquilerRestante = diasRestantes * reserva.MontoDiario;

            return alquilerRestante * porcentajeMulta;
        }

        public bool TerminarReservaAnticipada(
     int idReserva,
     DateTime nuevoFinFecha,
     int usuarioFinalizacionId)
        {
            const string sql = @"
        UPDATE reserva
        SET FechaFinEfectiva = @fin,
            UsuarioFinalizacionId = @usuario,
            Estado = 'Finalizada'
        WHERE IdReserva = @id
          AND Estado = 'Activo'
          AND FechaDeEntrada < @fin
          AND @fin < FechaDeSalida";

            using var c = new MySqlConnection(connectionString);
            using var q = new MySqlCommand(sql, c);

            q.Parameters.AddWithValue("@fin", nuevoFinFecha);
            q.Parameters.AddWithValue("@usuario", usuarioFinalizacionId);
            q.Parameters.AddWithValue("@id", idReserva);

            c.Open();

            return q.ExecuteNonQuery() > 0;
        }


        public bool TerminarReservaAnticipada(int idReserva, DateTime nuevoFinFecha)
        {
            using var c = new MySqlConnection(connectionString); using var q = new MySqlCommand("UPDATE reserva SET FechaFinEfectiva=@fin, Estado='Finalizada' WHERE IdReserva=@id", c); q.Parameters.AddWithValue("@fin", nuevoFinFecha); q.Parameters.AddWithValue("@id", idReserva); c.Open(); return q.ExecuteNonQuery() > 0;
        }

        public Reserva RenovarReserva(int idReserva, DateTime nuevoFinFecha, decimal nuevoPrecio)
        {
            var original = ObtenerPorId(idReserva) ?? throw new InvalidOperationException("Reserva original no encontrada.");
            if (nuevoFinFecha <= original.FechaDeSalida || ExisteSolapamiento(original.IdInmueble, original.FechaDeSalida, nuevoFinFecha)) throw new InvalidOperationException("Las fechas seleccionadas no son válidas o están ocupadas.");
            var nueva = new Reserva { IdInmueble = original.IdInmueble, IdInquilino = original.IdInquilino, FechaDeEntrada = original.FechaDeSalida, FechaDeSalida = nuevoFinFecha, MontoDiario = nuevoPrecio > 0 ? nuevoPrecio : original.MontoDiario, UsuarioCreacionId = original.UsuarioCreacionId }; Alta(nueva); return nueva;
        }

    }


}
