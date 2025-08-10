using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System;
using Agent.Core;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace AgentTIA.UI;

public partial class MainWindow : Window
{
    private Project? _project;
    private readonly Dictionary<ElementType,string> _symbols = new();

    public MainWindow()
    {
        InitializeComponent();
        LoadSymbolPack("Default");
    }

    private void ImportFolder_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _project = ProjectLoader.Load(dialog.SelectedPath);
            Tree.ItemsSource = _project.Blocks;
        }
    }

    private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is Block block && block.Networks.Count > 0)
        {
            var rung = block.Networks[0].Rungs.FirstOrDefault();
            if (rung != null)
            {
                Editor.Items.Clear();
                foreach (var el in rung.Elements)
                {
                    if (_symbols.TryGetValue(el.Type, out var path))
                    {
                        var img = new Image { Source = new BitmapImage(new Uri(path, UriKind.Absolute)), Width = 32, Height = 32 };
                        Editor.Items.Add(img);
                    }
                }
            }
        }
    }

    private void LoadSymbolPack(string name)
    {
        var baseDir = Path.Combine(AppContext.BaseDirectory, "assets", "SymbolPacks", name);
        var manifest = Path.Combine(baseDir, "manifest.json");
        if (!File.Exists(manifest)) return;
        var json = File.ReadAllText(manifest);
        using var doc = JsonDocument.Parse(json);
        foreach (var prop in doc.RootElement.GetProperty("symbols").EnumerateObject())
        {
            if (Enum.TryParse<ElementType>(prop.Name, out var type))
            {
                _symbols[type] = Path.Combine(baseDir, prop.Value.GetString()!);
            }
        }
    }
}
