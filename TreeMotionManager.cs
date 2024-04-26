using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterMotion;

public class TreeMotionManager : IMotionManager
{
    public static TreeMotionManager Instance { get; private set; }
    public TreeMotionManager(Type featureType) : base(featureType)
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
        Instance.OnPostfixDraw(__instance, spriteBatch);
    }
    public void OnPostfixDraw(TerrainFeature feature, SpriteBatch spriteBatch)
    {
        var tile = feature.Tile;
        var motion = ObjectMotionContainer[tile];
        var noise = new FastNoiseLite();
        var time = Game1.ticks * 0.3f;
        var noiseScale = 3f;
        var rot = noise.GetNoise(tile.X * noiseScale + time, tile.Y * noiseScale + time);
        motion.SetRotation(rot * 0.02f);
    }
}
