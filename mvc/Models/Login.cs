using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models;

public class Login
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string Clave { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}