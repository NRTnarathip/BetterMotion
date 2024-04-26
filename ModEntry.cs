using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
namespace BetterMotion;

public sealed class ModEntry : Mod
{
    public static Harmony HarmonyObj { get; private set; }
    List<IMotionManager> _motionManagers = new();

    public override void Entry(IModHelper helper)
    {
        Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

        HarmonyObj = new Harmony(this.ModManifest.UniqueID);
        HarmonyObj.PatchAll();

        _motionManagers.Add(new GrassMotionManager(typeof(Grass)));
        _motionManagers.Add(new TreeMotionManager(typeof(Tree)));
        _motionManagers.Add(new BushMotionManager(typeof(Bush)));
    }

    private void GameLoop_UpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player == null)
            return;

        var map = player.currentLocation;
        if (map == null)
            return;
        //delayShake = 5.0;
        foreach (var motionManager in _motionManagers)
        {
            motionManager.Ticked(e);
        }
    }

    private void Display_RenderedWorld(object? sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
    {
        var b = e.SpriteBatch;
        var player = Game1.player;
        if (player == null)
            return;
    }
}