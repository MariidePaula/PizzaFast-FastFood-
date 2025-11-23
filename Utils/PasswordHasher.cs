using System;
using System.Security.Cryptography;
using System.Text;

namespace FastPizza.Utils
{
    // Utilitário para hash e verificação de senhas usando SHA256
    public static class PasswordHasher
    {
        // Gera hash SHA256 da senha para armazenamento seguro
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Converte bytes para string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Verifica se a senha informada corresponde ao hash armazenado
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            // Gera hash da senha informada e compara com o hash armazenado
            string hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}

