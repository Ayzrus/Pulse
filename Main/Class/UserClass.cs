using Konscious.Security.Cryptography;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Main.Class
{
    public class UserClass
    {
        #region Constants

        public const string Table = "usuarios";

        #endregion Constants

        #region Properties

        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public int Telefone { get; set; }
        public required string Password { get; set; }
        public required string Foto { get; set; }
        public DateTime DataCriacao { get; set; }

        #endregion Properties

        #region Static Methods

        /// <summary>
        /// Method to get all users (Read)
        /// </summary>
        public static List<UserClass> GetUsuarios()
        {
            var usuarios = new List<UserClass>();
            using (var connection = new Connection())
            {
                var query = $"SELECT * FROM {Table}";
                using (var command = new MySqlCommand(query, connection.MySqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return usuarios;
        }

		/// <summary>
		/// Method to insert a new user (Create)
		/// </summary>
		/// <param name="usuario"></param>
		public static bool InserirUsuario(UserClass usuario)
		{
			try
			{
				using (var connection = new Connection())
				{
					var query = $"INSERT INTO {Table} (Nome, Username, Email, Telefone, Password, Foto) VALUES (@Nome, @Username, @Email, @Telefone, @Password, @Foto)";
					using (var command = new MySqlCommand(query, connection.MySqlConnection))
					{
						command.Parameters.AddWithValue("@Nome", usuario.Nome);
						command.Parameters.AddWithValue("@Username", usuario.Username);
						command.Parameters.AddWithValue("@Email", usuario.Email);
						command.Parameters.AddWithValue("@Telefone", usuario.Telefone);
						command.Parameters.AddWithValue("@Password", HashPassword(usuario.Password));
						command.Parameters.AddWithValue("@Foto", usuario.Foto ?? "~/lib/Template/dist/assets/media/users/default.jpg");
						command.ExecuteNonQuery();
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		/// <summary>
		/// Method to validate user login (Login)
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static (bool success, string message, string userName, string Foto, string Nome, string Telefone, string Email) ValidarLogin(string login, string password)
		{
			UserClass usuario = null;

			using (var connection = new Connection())
			{
				// Modifica a query para buscar pelo Username ou Email
				var query = $"SELECT * FROM {Table} WHERE Username = @Login OR Email = @Login";
				using (var command = new MySqlCommand(query, connection.MySqlConnection))
				{
					command.Parameters.AddWithValue("@Login", login);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							usuario = MapFromReader(reader);
						}
					}
				}
			}

			// Verifica se o usuário foi encontrado
			if (usuario == null)
			{
				return (false, "Usuário não encontrado.", "", "", "", "", "");
			}

			// Verifica se a senha está correta
			if (!VerifyPassword(password, usuario.Password))
			{
				return (false, "Senha incorreta.", "", "", "", "", "");
			}

			return (true, string.Empty, usuario.Username, usuario.Foto, usuario.Nome, usuario.Telefone.ToString(), usuario.Email);
		}



		private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // 16 bytes de salt
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static string HashPassword(string password)
        {
            byte[] salt = GenerateSalt(); // Gera um salt
            using (var hasher = new Argon2i(Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = salt;
                hasher.DegreeOfParallelism = 8;
                hasher.Iterations = 4;
                hasher.MemorySize = 65536;

                byte[] hash = hasher.GetBytes(32); // 32 bytes de hash
                                                   // Concatena o salt e o hash para armazenar
                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);
                return Convert.ToBase64String(hashBytes); // Retorna a string codificada em base64
            }
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, salt.Length); // Recupera o salt do hash armazenado

            using (var hasher = new Argon2i(Encoding.UTF8.GetBytes(password)))
            {
                hasher.Salt = salt;
                hasher.DegreeOfParallelism = 8;
                hasher.Iterations = 4;
                hasher.MemorySize = 65536;

                byte[] hash = hasher.GetBytes(32); // 32 bytes de hash
                                                   // Compara o hash gerado com o hash armazenado
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != hashBytes[salt.Length + i])
                    {
                        return false; // Senha não coincide
                    }
                }
            }
            return true; // Senha coincide
        }

		/// <summary>
		/// Method to update an existing user (Update)
		/// </summary>
		/// <param name="usuario"></param>
		public static bool AtualizarUsuario(UserClass usuario)
		{
			using (var connection = new Connection())
			{
				var query = $"UPDATE {Table} SET Nome = @Nome, Username = @Username, Email = @Email, Telefone = @Telefone, Password = @Password, Foto = @Foto WHERE Id = @Id";
				using (var command = new MySqlCommand(query, connection.MySqlConnection))
				{
					command.Parameters.AddWithValue("@Id", usuario.Id);
					command.Parameters.AddWithValue("@Nome", usuario.Nome);
					command.Parameters.AddWithValue("@Username", usuario.Username);
					command.Parameters.AddWithValue("@Email", usuario.Email);
					command.Parameters.AddWithValue("@Telefone", usuario.Telefone);
					command.Parameters.AddWithValue("@Password", HashPassword(usuario.Password));
					command.Parameters.AddWithValue("@Foto", usuario.Foto);

					int rowsAffected = command.ExecuteNonQuery();
					return rowsAffected > 0;
				}
			}
		}

		/// <summary>
		/// Method to delete a user (Delete)
		/// </summary>
		/// <param name="id"></param>
		public static void DeletarUsuario(int id)
        {
            using (var connection = new Connection())
            {
                var query = $"DELETE FROM {Table} WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection.MySqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets a specific user by Id (Read)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserClass GetUsuario(int id)
        {
            UserClass usuario = null;
            using (var connection = new Connection())
            {
                var query = $"SELECT * FROM {Table} WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection.MySqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapFromReader(reader);
                        }
                    }
                }
            }
            return usuario;
        }

		private static UserClass MapFromReader(MySqlDataReader reader)
		{
			return new UserClass
			{
				Id = reader.Cast<int>("Id"),
				Nome = reader.Cast<string>("Nome"),
				Username = reader.Cast<string>("Username"),
				Email = reader.Cast<string>("Email"),
				Telefone = reader.Cast<int>("Telefone"),
				Password = reader.Cast<string>("Password"),
				Foto = reader.Cast<string>("Foto"),
				DataCriacao = reader.Cast<DateTime>("DataCriacao")
			};
		}

		#endregion Static Methods
	}
}
