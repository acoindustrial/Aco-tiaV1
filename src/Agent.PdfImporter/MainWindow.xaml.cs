using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UglyToad.PdfPig;

namespace Agent.PdfImporter
{
    public partial class MainWindow : Window
    {
        private string? _pdfPath;
        private PdfIndex? _index;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportPdf_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf" };
            if (dlg.ShowDialog() == true)
            {
                LoadPdf(dlg.FileName);
            }
        }

        private void LoadPdf(string path)
        {
            _pdfPath = path;
            PdfViewer.Source = new Uri("file:///" + path);
            StatusText.Text = System.IO.Path.GetFileName(path);
            BuildIndex(path);
            PageList.ItemsSource = Enumerable.Range(1, _index?.Pages.Count ?? 0).ToList();
            BuildTree();
        }

        private void BuildIndex(string path)
        {
            var blockRegex = new Regex(@"^(?:Block\s+)?(OB|FB|FC|DB)\s*\d+", RegexOptions.Multiline);
            var networkRegex = new Regex(@"^(?:Network|Retea)\s*\d+", RegexOptions.Multiline);
            var index = new PdfIndex { PdfPath = path };
            string? currentBlock = null;
            using (var doc = PdfDocument.Open(path))
            {
                foreach (var page in doc.GetPages())
                {
                    var text = page.Text;
                    var info = new PdfPageInfo { PageNumber = page.Number, Text = text, ImageOnly = string.IsNullOrWhiteSpace(text) };
                    foreach (Match m in blockRegex.Matches(text))
                    {
                        currentBlock = m.Value.Trim();
                        var blockEntry = index.Blocks.FirstOrDefault(b => b.Name == currentBlock);
                        if (blockEntry == null)
                        {
                            blockEntry = new BlockEntry { Name = currentBlock };
                            index.Blocks.Add(blockEntry);
                        }
                        info.Blocks.Add(currentBlock);
                    }
                    foreach (Match m in networkRegex.Matches(text))
                    {
                        var name = m.Value.Trim();
                        info.Networks.Add(name);
                        if (currentBlock != null)
                        {
                            var blockEntry = index.Blocks.First(b => b.Name == currentBlock);
                            blockEntry.Networks.Add(new NetworkEntry { Name = name, Page = page.Number });
                        }
                    }
                    index.Pages.Add(info);
                }
            }
            _index = index;
            var indexPath = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileName(path) + ".index.json");
            File.WriteAllText(indexPath, JsonSerializer.Serialize(index, new JsonSerializerOptions { WriteIndented = true }));
        }

        private void BuildTree()
        {
            if (_index == null) return;
            IndexTree.Items.Clear();
            var root = new TreeViewItem { Header = $"PDF {System.IO.Path.GetFileName(_index.PdfPath)}" };
            var blocksNode = new TreeViewItem { Header = "Blocks" };
            root.Items.Add(blocksNode);
            foreach (var block in _index.Blocks)
            {
                var blockItem = new TreeViewItem { Header = block.Name };
                blocksNode.Items.Add(blockItem);
                foreach (var net in block.Networks)
                {
                    var netItem = new TreeViewItem { Header = net.Name, Tag = net.Page };
                    blockItem.Items.Add(netItem);
                }
            }
            IndexTree.SelectedItemChanged += IndexTree_SelectedItemChanged;
            IndexTree.Items.Add(root);
            root.IsExpanded = true;
        }

        private void IndexTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IndexTree.SelectedItem is TreeViewItem item && item.Tag is int page && _pdfPath != null)
            {
                PdfViewer.Source = new Uri($"file:///{_pdfPath}#page={page}");
                PageList.SelectedItem = page;
            }
        }

        private void PageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageList.SelectedItem is int page && _pdfPath != null)
            {
                PdfViewer.Source = new Uri($"file:///{_pdfPath}#page={page}");
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _pdfPath != null)
            {
                var term = SearchBox.Text;
                PdfViewer.Source = new Uri($"file:///{_pdfPath}#search={Uri.EscapeDataString(term)}");
            }
        }
    }
}
