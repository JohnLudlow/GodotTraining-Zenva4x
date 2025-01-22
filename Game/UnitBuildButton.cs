using Godot;
using System;

namespace ZenvaHexMap.Game;
public partial class UnitBuildButton : Button
{
  public Unit? Unit {get;set;}

  [Signal]
  public delegate void OnPressedEventHandler(Unit unit);

  public override void _Ready() {
    base._Ready();

    Pressed += () => {
      if (Unit is not null)
        EmitSignal(SignalName.OnPressed, Unit);
      };
  }
}
