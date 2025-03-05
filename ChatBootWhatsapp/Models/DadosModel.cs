using MySqlConnector;
using System;

namespace ChatBootWhatsapp.Models
{
    public class DadosModel
    {
        private readonly string _connectionString = "Server=localhost;User ID=root;Password=;Database=chatboot";

        public bool InsertStatus(string idWhatsapp, string status, string timestamp, string recipient_id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO status_mensagens (id_whatsapp, status, timestamp, recipient_id) VALUES (@idWhatsapp, @status, @timestamp, @recipient_id)";
                        command.Parameters.AddWithValue("@idWhatsapp", idWhatsapp);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@timestamp", timestamp);
                        command.Parameters.AddWithValue("@recipient_id", recipient_id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Retorna true se a inserção foi bem-sucedida
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar status no banco: {ex.Message}");
                return false;
            }
        }
    }
    public class Dados
    {
    }
    public class WebHookResponseModel
    {
        public Entry[] entry { get; set; }
    }
    public class Entry
    {
        public Change[] changes { get; set; }
    }
    public class Change
    {
        public Value value { get; set; }
    }
    public class Value
    {
        public int ad_id { get; set; }
        public long form_id { get; set; }
        public long leadgen_id { get; set; }
        public int created_time { get; set; }
        public long page_id { get; set; }
        public int adgroup_id { get; set; }
        public Messages[] messages { get; set; }
        public Status[] statuses { get; set; }
    }
    public class Messages
    {
        public string id { get; set; }
        public string from { get; set; }
        public Text text { get; set; }
    }
    public class Status
    {
        public string id { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; } 
        public string recipient_id { get; set; }
    }
    public class Delivery
    {
        public string id { get; set; }
        public string status { get; set; }
    }
    public class Text
    {
        public string body { get; set; }
    }
}
