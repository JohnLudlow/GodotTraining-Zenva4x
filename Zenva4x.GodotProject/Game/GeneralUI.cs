using Godot;

namespace Zenva4x.GodotProject.Game;

public partial class GeneralUI : Panel
{
  Label _turnLabel;
  private int _turnNumber;

  public int TurnNumber
  {
    get => _turnNumber;
    private set
    {
      _turnNumber = value;

      _turnLabel ??= GetNode<Label>("TurnLabel");
      _turnLabel.Text = $"Turn : {_turnNumber}";
    }
  }

  public override void _Ready() => TurnNumber = 1;

  public void EndTurn()
  {
    TurnNumber++;
  }
}
