using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace Agent.Core;

public static class Parser
{
    public static Block ParseBlock(string name, IEnumerable<string> lines)
    {
        var block = new Block { Name = name };
        var network = new Network { Index = 0 };
        var rung = new Rung { Index = 0 };
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            var element = ParseLine(trimmed.ToUpper());
            if (element != null)
                rung.Elements.Add(element);
        }
        network.Rungs.Add(rung);
        block.Networks.Add(network);
        return block;
    }

    static LadderElement? ParseLine(string line)
    {
        if (line.StartsWith("A "))
            return new LadderElement { Type = ElementType.NO, Label = line.Substring(2).Trim() };
        if (line.StartsWith("AN "))
            return new LadderElement { Type = ElementType.NC, Label = line.Substring(3).Trim() };
        if (line.StartsWith("="))
            return new LadderElement { Type = ElementType.COIL, Label = line.Substring(1).Trim() };
        if (line.StartsWith("S "))
            return new LadderElement { Type = ElementType.SET, Label = line.Substring(2).Trim() };
        if (line.StartsWith("R "))
            return new LadderElement { Type = ElementType.RESET, Label = line.Substring(2).Trim() };
        if (line.StartsWith("TON"))
            return new LadderElement { Type = ElementType.TON };
        if (line.StartsWith("TOF"))
            return new LadderElement { Type = ElementType.TOF };
        if (line.StartsWith("TP"))
            return new LadderElement { Type = ElementType.TP };
        if (line.StartsWith("CTU"))
            return new LadderElement { Type = ElementType.CTU };
        if (line.StartsWith("CTD"))
            return new LadderElement { Type = ElementType.CTD };
        if (line.StartsWith("MOVE") || line.StartsWith("MOV"))
            return new LadderElement { Type = ElementType.MOVE };
        if (line.StartsWith("ADD"))
            return new LadderElement { Type = ElementType.ADD };
        if (line.StartsWith("SUB"))
            return new LadderElement { Type = ElementType.SUB };
        if (line.StartsWith("MUL"))
            return new LadderElement { Type = ElementType.MUL };
        if (line.StartsWith("DIV"))
            return new LadderElement { Type = ElementType.DIV };
        if (Regex.IsMatch(line, "[<>=]"))
            return new LadderElement { Type = ElementType.COMP };
        return null;
    }
}
