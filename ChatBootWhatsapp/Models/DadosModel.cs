﻿namespace ChatBootWhatsapp.Models
{
    using MySqlConnector;
    public class DadosModel
    {
        public void insert(string mensagem_recebida, string mensagem_enviada, string id_whatsapp, string telefone_whatsapp)
        {
            var connection = new MySqlConnection("Server=localhost;User ID=root;Password=;Database=chatboot");
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `registros` " +
                    "(`mensagem_recebida`        ,`mensagem_enviada`        ,`id_whatsapp`         , `telefone_whatsapp`) VALUES " +
                    "('" + mensagem_recebida + "','" + mensagem_enviada + "', '" + id_whatsapp + "', '" + telefone_whatsapp + "');";
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //return ex.Message;
            }
            finally
            {
                connection.Close();
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
