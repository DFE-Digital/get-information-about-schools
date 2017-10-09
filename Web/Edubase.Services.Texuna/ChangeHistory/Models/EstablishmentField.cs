namespace Edubase.Services.Texuna.ChangeHistory.Models
{
    using Newtonsoft.Json;

    public class EstablishmentField
    {
        [JsonProperty("id")]
        public string Key { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public EstablishmentField()
        {
                
        }

        public EstablishmentField(string key, string text)
        {
            Key = key;
            Text = text;
        }
    }
}
