namespace PGB.Chat.Application.Interfaces
{
    public class OpenAiSettings
    {
        #region Properties
        public string ApiKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty; 
        #endregion
    }
}