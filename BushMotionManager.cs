using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterMotion;

public class BushMotionManager : IMotionManager
{
    public static BushMotionManager Instance { get; private set; }

    public BushMotionManager(Type featureType) : base(featureType)
    {
        Instance = this;
        InitHookDraw(this);
    }

    public override void Ticked(UpdateTickedEventArgs e)
    {
        TickedInternal(e);
    }
    static void PrefixDraw(TerrainFeature __instance, SpriteBatch spriteBatch)
    {
        Instance.onPrefixDraw(__instance, spriteBatch);
    }

    private void onPrefixDraw(TerrainFeature feature, SpriteBatch spriteBatch)
    {
        var tile = feature.Tile;
        var motion = ObjectMotionContainer[tile];
        var noise = new FastNoiseLite();
        var time = Game1.ticks * 0.3f;
        var noiseScale = 6f;
        var rot = noise.GetNoise(tile.X * noiseScale + time, tile.Y * noiseScale + time);
        motion.SetRotation(rot * 0.1f);
    }
}
