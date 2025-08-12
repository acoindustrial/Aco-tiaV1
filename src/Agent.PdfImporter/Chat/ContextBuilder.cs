using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent.PdfImporter.Chat
{
    public class ContextBuilder
    {
        private readonly int _maxOcrChars;

        public ContextBuilder(int maxOcrChars = 2000)
        {
            _maxOcrChars = maxOcrChars;
        }

        public string BuildPrompt(string ocrText, IEnumerable<string> hints, string question)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are a helpful assistant for PLC ladder logic.");
            if (!string.IsNullOrWhiteSpace(ocrText))
            {
                var truncated = ocrText.Length > _maxOcrChars ? ocrText[.._maxOcrChars] : ocrText;
                sb.AppendLine("OCR Text:");
                sb.AppendLine(truncated);
            }
            if (hints != null && hints.Any())
            {
                sb.AppendLine("Hints:");
                foreach (var hint in hints)
                {
                    sb.AppendLine("- " + hint);
                }
            }
            sb.AppendLine("Question:");
            sb.AppendLine(question);
            return sb.ToString();
        }
    }
}
