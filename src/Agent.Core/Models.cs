using System.Collections.Generic;

namespace Agent.Core;

public enum ElementType
{
    NO,
    NC,
    COIL,
    SET,
    RESET,
    TON,
    TOF,
    TP,
    CTU,
    CTD,
    MOVE,
    COMP,
    ADD,
    SUB,
    MUL,
    DIV
}

public class Project
{
    public List<Block> Blocks { get; } = new();
}

public class Block
{
    public string Name { get; set; } = string.Empty;
    public List<Network> Networks { get; } = new();
    public override string ToString() => Name;
}

public class Network
{
    public int Index { get; set; }
    public List<Rung> Rungs { get; } = new();
}

public class Rung
{
    public int Index { get; set; }
    public List<LadderElement> Elements { get; } = new();
}

public class LadderElement
{
    public ElementType Type { get; set; }
    public string? Label { get; set; }
    public Dictionary<string,string> Parameters { get; } = new();
}
