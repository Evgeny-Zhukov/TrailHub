using TrailHub.API.Application.Interfaces;

namespace TrailHub.API.Application.Services
{
    public class HashingService : IHashingService
    {
        public string Hash(string password) 
        {
            // Генерирует соль и хэш (автоматически добавляет соль)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            // Проверяет, совпадает ли пароль с хэшем
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
