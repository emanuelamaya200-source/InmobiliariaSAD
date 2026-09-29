
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Inmobiliaria_.Net_Core.Models
{
	public enum enRoles
	{
		Administrador = 1,
		Empleado = 2,
	}

	public class Usuario
	{
		[Key]
		[Display(Name = "Código")]
		public int Id { get; set; }
		[Required]
		public string Nombre { get; set; } = "";
		[Required]
		public string Apellido { get; set; } = "";
		[Required, EmailAddress]
		public string Email { get; set; } = "";
		[Required, DataType(DataType.Password)]
		public string Clave { get; set; } = "";
		public string? Avatar { get; set; }
		public IFormFile? AvatarFile { get; set; }

		public int Rol { get; set; }
		public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(enRoles);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor) ?? "");
			}
			return roles;
		}
	}

	public static class PasswordHash
	{
		private const int Iterations = 100000;

		public static string Hash(string password)
		{
			var salt = RandomNumberGenerator.GetBytes(16);
			var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
			return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
		}

		public static bool Verify(string stored, string password, string legacySalt = "")
		{
			if (!stored.StartsWith("PBKDF2$", StringComparison.Ordinal))
			{
				if (string.Equals(stored, password, StringComparison.Ordinal))
					return true;

				try
				{
					var expected = Convert.FromBase64String(stored);
					if (expected.Length != 32)
						return false;
					var actual = Rfc2898DeriveBytes.Pbkdf2(password, System.Text.Encoding.ASCII.GetBytes(legacySalt), 1000, HashAlgorithmName.SHA1, expected.Length);
					return CryptographicOperations.FixedTimeEquals(actual, expected);
				}
				catch (FormatException)
				{
					return false;
				}
			}

			var parts = stored.Split('$');
			if (parts.Length != 4 || parts[0] != "PBKDF2" || !int.TryParse(parts[1], out var iterations) || iterations is < 1 or > 1000000)
				return false;

			try
			{
				var salt = Convert.FromBase64String(parts[2]);
				var expected = Convert.FromBase64String(parts[3]);
				var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
				return CryptographicOperations.FixedTimeEquals(actual, expected);
			}
			catch (FormatException)
			{
				return false;
			}
		}

		public static bool NeedsUpgrade(string stored) => !stored.StartsWith("PBKDF2$", StringComparison.Ordinal);
	}
}
