using System.Collections.Generic;

namespace Agent.PdfImporter
{
    public class PdfIndex
    {
        public string PdfPath { get; set; } = string.Empty;
        public List<PdfPageInfo> Pages { get; set; } = new();
        public List<BlockEntry> Blocks { get; set; } = new();
    }

    public class PdfPageInfo
    {
        public int PageNumber { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool ImageOnly { get; set; }
        public List<string> Blocks { get; set; } = new();
        public List<string> Networks { get; set; } = new();
    }

    public class BlockEntry
    {
        public string Name { get; set; } = string.Empty;
        public List<NetworkEntry> Networks { get; set; } = new();
    }

    public class NetworkEntry
    {
        public string Name { get; set; } = string.Empty;
        public int Page { get; set; }
    }
}
