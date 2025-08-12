using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agent.PdfImporter.Chat
{
    public class ChatService
    {
        private readonly LlmClient _llm;
        private readonly ContextBuilder _contextBuilder;
        private readonly List<ChatMessage> _history = new();

        public IReadOnlyList<ChatMessage> History => _history;

        public ChatService(LlmClient llm, ContextBuilder contextBuilder)
        {
            _llm = llm;
            _contextBuilder = contextBuilder;
        }

        public async Task<ChatMessage> AskAsync(string question, string ocrText, IEnumerable<string> hints)
        {
            var prompt = _contextBuilder.BuildPrompt(ocrText, hints, question);
            _history.Add(new ChatMessage { Role = ChatRole.User, Content = question });
            var completion = await _llm.GetCompletionAsync(prompt);
            var reply = new ChatMessage { Role = ChatRole.Assistant, Content = completion };
            _history.Add(reply);
            return reply;
        }

        public void ExportMarkdown(string path)
        {
            var sb = new StringBuilder();
            foreach (var msg in _history)
            {
                sb.AppendLine($"### {msg.Role}");
                sb.AppendLine(msg.Content);
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString());
        }

        public void ExportHtml(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body>");
            foreach (var msg in _history)
            {
                sb.AppendLine($"<h3>{msg.Role}</h3>");
                sb.AppendLine($"<p>{System.Net.WebUtility.HtmlEncode(msg.Content)}</p>");
            }
            sb.AppendLine("</body></html>");
            File.WriteAllText(path, sb.ToString());
        }
    }
}
