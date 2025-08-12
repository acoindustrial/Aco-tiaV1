using System.Collections.Generic;

namespace Agent.PdfImporter.Chat
{
    public enum ChatRole
    {
        System,
        User,
        Assistant
    }

    public class ChatMessage
    {
        public ChatRole Role { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Thumbnails { get; set; } = new();
    }
}
