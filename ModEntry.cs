using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
namespace BetterMotion;

public sealed class ModEntry : Mod
{
    GrassMotionManager grassMotionManager;
    public override void Entry(IModHelper helper)
    {
        Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        var h = new Harmony(this.ModManifest.UniqueID);
        h.PatchAll();
        grassMotionManager = new(h);
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
        grassMotionManager.Ticked(e);
    }

    private void Display_RenderedWorld(object? sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
    {
        var b = e.SpriteBatch;
        var player = Game1.player;
        if (player == null)
            return;
    }
}