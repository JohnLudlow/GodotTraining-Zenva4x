using Godot;

namespace Zenva4x.GodotProject.Game;

public partial class Camera : Camera2D
{
  [Export]
  public int Velocity { get; set; } = 15;

  [Export]
  public float ZoomSpeed { get; set; } = .05f;

  private float _leftBound, _rightBound, _topBound, _bottomBound;

  private HexTileMap _map;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    _map = GetNode<HexTileMap>("../HexTileMap");

    _leftBound = ToGlobal(_map.MapToLocal(new Vector2I(0, 0))).X + 100;
    _rightBound = ToGlobal(_map.MapToLocal(new Vector2I(_map.Width, 0))).X - 100;
    _topBound = ToGlobal(_map.MapToLocal(new Vector2I(0, 0))).Y + 50;
    _bottomBound = ToGlobal(_map.MapToLocal(new Vector2I(0, _map.Height))).Y - 50;

    Position = GetScreenCenterPosition();
  }

  public override void _PhysicsProcess(double delta)
  {
    if (Input.IsActionPressed("map_right") && Position.X < _rightBound)
    {
      Position += new Vector2(Velocity, 0);
    }

    if (Input.IsActionPressed("map_left") && Position.X > _leftBound)
    {
      Position += new Vector2(-Velocity, 0);
    }

    if (Input.IsActionPressed("map_up") && Position.Y > _topBound)
    {
      Position += new Vector2(0, -Velocity);
    }

    if (Input.IsActionPressed("map_down") && Position.Y < _bottomBound)
    {
      Position += new Vector2(0, Velocity);
    }

    if (Input.IsActionPressed("map_zoom_in") || Input.IsActionJustReleased("mouse_zoom_in"))
    {
      if (Zoom < new Vector2(3f, 3f))
        Zoom += new Vector2(ZoomSpeed, ZoomSpeed);
    }

    if (Input.IsActionPressed("map_zoom_out") || Input.IsActionJustReleased("mouse_zoom_out"))
    {
      if (Zoom > new Vector2(.1f, .1f))
        Zoom -= new Vector2(ZoomSpeed, ZoomSpeed);
    }
  }
}
