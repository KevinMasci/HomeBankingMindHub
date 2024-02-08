namespace HomeBankingMindHub.Utils
{
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            // Generar un nuevo hash de contraseña con un salt aleatorio
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Verificar si la contraseña proporcionada coincide con el hash almacenado
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
