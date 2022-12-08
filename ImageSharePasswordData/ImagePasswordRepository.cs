using System;
using System.Data.SqlClient;

namespace ImageSharePasswordData
{
    public class ImagePasswordRepository
    {
        private string _connectionString;
        public ImagePasswordRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(string imagePath, string password)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Images (ImagePath, Password, Views) VALUES (@path, @password, @views) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@path", imagePath);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@views", 0);
            conn.Open();
           
            int id = (int)(decimal)cmd.ExecuteScalar();
            return id;
        }

        public Image GetImage(int id)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if(reader.Read() == false)
            {
                return null;
            }
            else
            {
                return (new Image
                {
                    Id = (int)reader["Id"],
                    ImagePath = (string)reader["ImagePath"],
                    Password = (string)reader["Password"],
                    Views = (int)reader["Views"]
                });
            }
        }

        public void EditAmountOfViews(int id, int views)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views = @views WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@views", views);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
