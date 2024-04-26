using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Reflection;

namespace BetterMotion;

public class GrassMotionManager
{
    public class GrassMotion
    {
        public Grass grass;
        public float motion = 0f;
    }
    public static FieldInfo shakeRotationField = typeof(Grass).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance);
    TerrainFeature[] Grasses;
    Vector2 HalfVec2 = new Vector2(0.5f, 0.5f);
    FastNoiseLite _noise = null;
    public Dictionary<Vector2, GrassMotion> GrassMap = new();
    Harmony h;

    public static GrassMotionManager Instance { get; private set; }
    public GrassMotionManager(Harmony h)
    {
        Instance = this;
        h = h;
        GrassHook.Init(h);
    }

    public void Ticked(UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        var location = player.currentLocation;
        Grasses = location.terrainFeatures.Values.Where(f => f as Grass != null).ToArray();

        var noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(3);
        noise.SetFractalLacunarity(.5f);
        noise.SetFractalGain(0.2f);

        var time = Game1.ticks;
        foreach (Grass grass in Grasses)
        {
            var tile = grass.Tile;
            GrassMap.TryGetValue(tile, out var grassMotion);
            if (grassMotion == null)
            {
                grassMotion = new();
                grassMotion.grass = grass;
                GrassMap[tile] = grassMotion;
            }

            var playerTile = player.Position;
            playerTile.X /= 64f;
            playerTile.Y /= 64f;

            var tileCenter = grass.Tile + HalfVec2;
            var playerTileCenter = playerTile + HalfVec2;


            var offset = playerTileCenter - tileCenter;
            var offsetLen = offset.Length();

            float posScale = 20f;
            float x = tile.X;
            float y = tile.Y;
            x *= posScale;
            y *= posScale;

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

            //grass interaction
            //if (offsetLen <= 1)
            //{
            //    float distort = 0;
            //    if (offset.X >= 0)
            //        distort -= 1 - offset.X;
            //    else
            //        distort += 1 + offset.X;

            //    distort = distort * (1 - offsetLen);
            //    motion += distort;
            //    motion = Math.Clamp(motion, -1, 1);
            //}

            //shakeRotationField.SetValue(grass, rot);
            grassMotion.motion = motion;
        }
    }
}
