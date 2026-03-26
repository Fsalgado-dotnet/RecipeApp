using RecipeApp.DAL;
using RecipeApp.Models;
using RecipeApp.Services; // Se tiver aqui o PasswordHelper

namespace RecipeApp.Services
{
    public class UserService
    {
        private readonly UserDAL _userDal;

        public UserService(UserDAL userDal)
        {
            _userDal = userDal;
        }

        public bool Register(string name, string email, string password)
        {
            // Lógica de Negócio 1: Verificar se o utilizador já existe
            if (_userDal.EmailExists(email))
            {
                return false;
            }

            // Lógica de Negócio 2: Preparar o objeto User com valores padrão
            var newUser = new User
            {
                Name = name,
                Email = email,
                // A lógica de segurança (BCrypt) agora é centralizada aqui
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsAdmin = false, // Por padrão, ninguém nasce admin
                IsLocked = false,
                CreatedAt = DateTime.Now
            };

            _userDal.CreateUser(newUser);
            return true;
        }

        public User? Authenticate(string email, string password)
        {
            var user = _userDal.GetByEmail(email);

            if (user == null || user.IsLocked)
                return null;

            // Lógica de Negócio 3: Verificar a password
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isValid ? user : null;
        }
    }
}