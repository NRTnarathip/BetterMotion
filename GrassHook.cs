using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Reflection;

namespace BetterMotion;

[HarmonyPatch]
static class GrassHook
{
    static BindingFlags PrivateStaticFlag = BindingFlags.NonPublic | BindingFlags.Static;
    static BindingFlags PrivateObjFlag = BindingFlags.NonPublic | BindingFlags.Instance;
    public static void Init()
    {
        var drawMethod = typeof(Grass).GetMethod(nameof(Grass.draw));
        var prefixMethod = typeof(GrassHook).GetMethod(nameof(Draw), PrivateStaticFlag);
        var h = ModEntry.HarmonyObj;
        h.Patch(drawMethod, prefix: new(prefixMethod));
    }
    static FieldInfo whichWeedField = typeof(Grass).GetField("whichWeed", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo offset1Field = typeof(Grass).GetField("offset1", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo offset2Field = typeof(Grass).GetField("offset2", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo offset3Field = typeof(Grass).GetField("offset3", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo offset4Field = typeof(Grass).GetField("offset4", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo shakeRandomField = typeof(Grass).GetField("shakeRandom", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo flip_Field = typeof(Grass).GetField("flip", BindingFlags.Instance | BindingFlags.NonPublic);
    static FieldInfo shakeRotation_Field = typeof(Grass).GetField("shakeRotation", BindingFlags.Instance | BindingFlags.NonPublic);


    static bool Draw(Grass __instance, SpriteBatch spriteBatch)
    {
        var tileLocation = __instance.Tile;
        int numberOfWeeds = __instance.numberOfWeeds;
        int[] whichWeed = whichWeedField.GetValue(__instance) as int[];
        int[] offset1 = offset1Field.GetValue(__instance) as int[];
        int[] offset2 = offset2Field.GetValue(__instance) as int[];
        int[] offset3 = offset3Field.GetValue(__instance) as int[];
        int[] offset4 = offset4Field.GetValue(__instance) as int[];
        bool[] flip = flip_Field.GetValue(__instance) as bool[];
        var texture = __instance.texture;
        double[] shakeRandom = shakeRandomField.GetValue(__instance) as double[];

        float shakeRotation = (float)shakeRotation_Field.GetValue(__instance);
        var objMotion = GrassMotionManager.Instance.ObjectMotionContainer[tileLocation];
        shakeRotation += objMotion.motion;

        for (int i = 0; i < numberOfWeeds; i++)
        {
            Vector2 pos = (i != 4) ? (tileLocation * 64f + new Vector2((float)(i % 2 * 64 / 2 + offset3[i] * 4 - 4) + 30f, i / 2 * 64 / 2 + offset4[i] * 4 + 40)) : (tileLocation * 64f + new Vector2((float)(16 + offset1[i] * 4 - 4) + 30f, 16 + offset2[i] * 4 + 40));
            spriteBatch.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, pos),
                new Rectangle(whichWeed[i] * 15, __instance.grassSourceOffset, 15, 20), Color.White, shakeRotation / (float)(shakeRandom[i] + 1.0), new Vector2(7.5f, 17.5f), 4f, flip[i] ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (pos.Y + 16f - 20f) / 10000f + pos.X / 10000000f);
        }

        return false;
    }
}
