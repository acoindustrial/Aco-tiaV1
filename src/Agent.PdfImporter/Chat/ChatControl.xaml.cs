using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Agent.PdfImporter.Chat
{
    public partial class ChatControl : UserControl
    {
        private readonly ChatService _chat;
        public string OcrText { get; set; } = string.Empty;
        public List<string> Hints { get; set; } = new();

        public ChatControl()
        {
            InitializeComponent();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine("..", "..", "configs", "appsettings.json"), optional: true)
                .Build();
            var llmConfig = configRoot.GetSection("LLM").Get<LlmConfig>() ?? new LlmConfig();
            _chat = new ChatService(new LlmClient(llmConfig), new ContextBuilder());

            Messages.ItemsSource = _chat.History;
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OcrText))
            {
                _chat.History.Add(new ChatMessage { Role = ChatRole.System, Content = "No OCR data. Please re-scan." });
                Messages.Items.Refresh();
                return;
            }

            var question = Input.Text;
            Input.Clear();
            await _chat.AskAsync(question, OcrText, Hints);
            Messages.Items.Refresh();
        }

        private async void QuickSuggestion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                Input.Text = btn.Content.ToString();
                Send_Click(sender, e);
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var path = Path.Combine(Path.GetTempPath(), "chat.md");
            _chat.ExportMarkdown(path);
            MessageBox.Show($"Exported to {path}");
        }
    }
}
