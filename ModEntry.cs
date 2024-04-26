using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Reflection;
namespace BetterMotion;

public sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        var h = new Harmony(this.ModManifest.UniqueID);
        h.PatchAll();
        GrassHook.Init(h);
    }

    static FieldInfo shakeRotationField = typeof(Grass).GetField("shakeRotation", BindingFlags.NonPublic | BindingFlags.Instance);
    static FastNoiseLite noise
    {
        get
        {
            if (_noise == null)
            {
                _noise = new();
                _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
                noise.SetFractalOctaves(3);
            }
            return _noise;
        }
    }
    static FastNoiseLite _noise = null;
    static TerrainFeature[] Grasses;
    static Vector2 HalfVec2 = new Vector2(0.5f, 0.5f);
    private void GameLoop_UpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player == null)
            return;

        var map = player.currentLocation;
        if (map == null)
            return;
        //delayShake = 5.0;
        var time = Game1.ticks;
        Grasses = map.terrainFeatures.Values.Where(f => f as Grass != null).ToArray();
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(3);
        noise.SetFractalLacunarity(.5f);
        noise.SetFractalGain(0.2f);

        foreach (Grass grass in Grasses)
        {
            var tile = grass.Tile;
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


            float rot = 0;
            rot += noise.GetNoise(x, y) * 0.2f;
            rot += noise.GetNoise(x * .2f, y * .2f) * 0.7f;
            rot /= 0.7f + 0.2f;

            if (offsetLen <= 1)
            {
                float distort = 0;
                if (offset.X >= 0)
                    distort -= 1 - offset.X;
                else
                    distort += 1 + offset.X;

                distort = distort * (1 - offsetLen);
                rot += distort;
                rot = Math.Clamp(rot, -1, 1);
            }

            shakeRotationField.SetValue(grass, rot);
        }
    }

    private void Display_RenderedWorld(object? sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
    {
        var b = e.SpriteBatch;
        var player = Game1.player;
        if (player == null)
            return;

        //var drawPos = Game1.GlobalToLocal(Game1.viewport, player.Position);
        //b.Draw(Game1.cropSpriteSheet,
        //    new Rectangle((int)drawPos.X, (int)drawPos.Y, 8, 8),
        //    new Rectangle(104, 305, 1, 1), Color.White);
    }
}