using MySqlConnector;
using System;

namespace ChatBootWhatsapp.Models
{
    public class DadosModel
    {
        private readonly string _connectionString = "Server=localhost;User ID=root;Password=;Database=chatboot";

        public bool Insert(string mensagemRecebida, string mensagemEnviada, string idWhatsapp, string telefoneWhatsapp)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO registros (mensagem_recebida, mensagem_enviada, id_whatsapp, telefone_whatsapp) VALUES (@mensagemRecebida, @mensagemEnviada, @idWhatsapp, @telefoneWhatsapp)";
                        command.Parameters.AddWithValue("@mensagemRecebida", mensagemRecebida);
                        command.Parameters.AddWithValue("@mensagemEnviada", mensagemEnviada);
                        command.Parameters.AddWithValue("@idWhatsapp", idWhatsapp);
                        command.Parameters.AddWithValue("@telefoneWhatsapp", telefoneWhatsapp);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Retorna true se a inserção foi bem-sucedida
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar no banco: {ex.Message}");
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
    }
    public class Messages
    {
        public string id { get; set; }
        public string from { get; set; }
        public Text text { get; set; }
    }
    public class Text
    {
        public string body { get; set; }
    }
}
