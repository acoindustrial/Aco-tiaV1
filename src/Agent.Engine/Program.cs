using System;
using Agent.Core;
using System.Linq;

if (args.Length >= 3 && args[0] == "analyze" && args[1] == "--src")
{
    var folder = args[2].Trim('"');
    if (!System.IO.Directory.Exists(folder))
    {
        Console.WriteLine($"Folder not found: {folder}");
        return 1;
    }
    var project = ProjectLoader.Load(folder);
    int blockCount = project.Blocks.Count;
    int elementCount = project.Blocks.Sum(b => b.Networks.Sum(n => n.Rungs.Sum(r => r.Elements.Count)));
    Console.WriteLine($"Blocks: {blockCount}");
    Console.WriteLine($"Elements: {elementCount}");
    return 0;
}

Console.WriteLine("Usage: Agent.Engine analyze --src <folder>");
return 1;
