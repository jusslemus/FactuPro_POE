using System.Security.Cryptography;

public static class PasswordHelper
{
    // Devuelve el hash y el salt para guardar en la tabla Usuario
    public static (string hash, string salt) Crear(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        return (Calcular(password, salt), Convert.ToBase64String(salt));
    }

    // Compara la contraseña escrita contra el hash y salt guardados
    public static bool Verificar(string password, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        return Calcular(password, saltBytes) == hash;
    }

    private static string Calcular(string password, byte[] salt)
    {
        byte[] h = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(h);
    }
}