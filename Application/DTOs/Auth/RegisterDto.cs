using System.ComponentModel.DataAnnotations;

namespace TrailHub.API.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, MinimumLength = 3)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
