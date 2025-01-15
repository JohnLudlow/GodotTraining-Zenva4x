using Godot;
using System;
using System.Diagnostics;

namespace ZenvaHexMap.Game;

[DebuggerDisplay("{UnitName}")]
public partial class Unit : Node2D
{
  public string UnitName { get; protected set; } = "DEFAULT";
  public int ProductionRequired { get; protected set; }
}
