using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Reflection;

namespace BetterMotion;

public class GrassMotionManager : IMotionManager
{
    public class GrassMotion
    {
        public Grass grass;
        public float motion = 0f;
    }
    public static FieldInfo shakeRotationField = typeof(Grass).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance);
    Vector2 HalfVec2 = new Vector2(0.5f, 0.5f);
    FastNoiseLite noise = null;

    public static GrassMotionManager Instance { get; private set; }
    public GrassMotionManager(Type featureType) : base(featureType)
    {
        Instance = this;
        GrassHook.Init();
    }

    public override void OnInitNewMap(GameLocation location)
    {
        base.OnInitNewMap(location);

        noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(3);
        noise.SetFractalLacunarity(.5f);
        noise.SetFractalGain(0.2f);
    }
    public override void Ticked(UpdateTickedEventArgs e)
    {
        TickedInternal(e);

        if (this._currentMap == null)
            return;

        var time = Game1.ticks;
        foreach (ObjectMotion objMotion in ObjectMotionContainer.Values)
        {
            var grass = objMotion.feature as Grass;
            var tile = grass.Tile;

            var playerTile = player.Position;
            playerTile.X /= 64f;
            playerTile.Y /= 64f;

            var tileCenter = grass.Tile + HalfVec2;
            var playerTileCenter = playerTile + HalfVec2;

            var offset = playerTileCenter - tileCenter;
            var offsetLen = offset.Length();

            float posScale = 20f;
            float x = tile.X * posScale;
            float y = tile.Y * posScale;

            float speedMultiply = 1.0f;
            float wildDir = -1f;
            x += speedMultiply * time * -wildDir;
            y += speedMultiply * time;


            float motion = 0;
            float baseWild = 0.4f;
            float offset1Wild = 0.9f;
            motion += noise.GetNoise(x, y) * baseWild;
            motion += noise.GetNoise(x * .2f, y * .2f) * offset1Wild;
            motion /= baseWild + offset1Wild;

            objMotion.motion = motion;
        }
    }
}
