using System.IO;
using System.Linq;

namespace Agent.Core;

public static class ProjectLoader
{
    public static Project Load(string folder)
    {
        var project = new Project();
        var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".scl", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".stl", StringComparison.OrdinalIgnoreCase));
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            var name = Path.GetFileNameWithoutExtension(file);
            var block = Parser.ParseBlock(name, lines);
            project.Blocks.Add(block);
        }
        return project;
    }
}
