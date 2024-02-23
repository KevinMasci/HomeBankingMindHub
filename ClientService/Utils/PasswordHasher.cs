namespace ClientService.Utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generar un nuevo hash de contraseña con un salt aleatorio
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verificar si la contraseña proporcionada coincide con el hash almacenado
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
