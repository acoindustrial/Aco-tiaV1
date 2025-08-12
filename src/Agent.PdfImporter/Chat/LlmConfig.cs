namespace Agent.PdfImporter.Chat
{
    public class LlmConfig
    {
        public string Url { get; set; } = "";
        public string Model { get; set; } = "";
        public int MaxTokens { get; set; } = 512;
    }
}
