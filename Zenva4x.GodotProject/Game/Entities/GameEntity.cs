using System;

namespace Zenva4x.GodotProject.Game.Entities;

public class GameEntity : INotifyGameEntityPropertyChanged
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public event EventHandler<GameEntityChangedEventArgs> EntityChanged;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}