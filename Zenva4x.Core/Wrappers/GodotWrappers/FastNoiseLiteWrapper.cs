using System.Reflection.Metadata.Ecma335;

using Godot;

namespace Zenva4x.Core.Wrappers.GodotWrappers;

public interface IFastNoiseLite
{
  /// <summary>
  /// <para>Returns the 2D noise value at the given position.</para>
  /// </summary>
  float GetNoise2D(float x, float y);

  /// <summary>
  /// <para>The noise algorithm used. See <see cref="Godot.FastNoiseLite.NoiseTypeEnum"/>.</para>
  /// </summary>
  FastNoiseLite.NoiseTypeEnum NoiseType {get;set;}

  /// <summary>
  /// <para>The random number seed for all noise types.</para>
  /// </summary>
  public int Seed { get; set; }

  /// <summary>
  /// <para>The frequency for all noise types. Low frequency results in smooth noise while high frequency results in rougher, more granular noise.</para>
  /// </summary>
  public float Frequency { get; set; }

  /// <summary>
  /// <para>The method for combining octaves into a fractal. See <see cref="Godot.FastNoiseLite.FractalTypeEnum"/>.</para>
  /// </summary>
  public FastNoiseLite.FractalTypeEnum FractalType { get; set; }

  /// <summary>
  /// <para>The number of noise layers that are sampled to get the final value for fractal noise types.</para>
  /// </summary>
  public int FractalOctaves { get;set;}

  /// <summary>
  /// <para>Frequency multiplier between subsequent octaves. Increasing this value results in higher octaves producing noise with finer details and a rougher appearance.</para>
  /// </summary>
  public float FractalLacunarity { get;set; }  
}

internal class FastNoiseLiteWrapper : IFastNoiseLite, IDisposable
{
  private readonly FastNoiseLite _noise = new();
  private bool _disposedValue;

  /// <inheritdoc/>
  public FastNoiseLite.NoiseTypeEnum NoiseType { get; set; }
  
  /// <inheritdoc/>
  public int Seed { get; set; }
  
  /// <inheritdoc/>
  public float Frequency { get; set; }

  /// <inheritdoc/>
  public FastNoiseLite.FractalTypeEnum FractalType { get; set; }

  /// <inheritdoc/>
  public int FractalOctaves { get; set; }

  /// <inheritdoc/>
  public float FractalLacunarity { get; set; }

  /// <inheritdoc/>
  public float GetNoise2D(float x, float y) => _noise.GetNoise2D(x, y);

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        _noise.Dispose();
      }

      // TODO: free unmanaged resources (unmanaged objects) and override finalizer
      // TODO: set large fields to null
      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}