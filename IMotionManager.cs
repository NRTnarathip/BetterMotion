using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Reflection;

namespace BetterMotion;

public abstract class IMotionManager
{
    protected GameLocation _currentMap = null;
    public Dictionary<Vector2, ObjectMotion> ObjectMotionContainer = new();
    protected Type _featureType;
    public MethodInfo drawMethodOriginal;

    public static Dictionary<Type, IMotionManager> Registry = new Dictionary<Type, IMotionManager>();

    protected IMotionManager(Type featureType)
    {
        Registry.Add(featureType, this);
        _featureType = featureType;
    }

    //dont forget add static method at main class
    protected void InitHookDraw(IMotionManager motionManager)
    {
        var drawMethod = _featureType.GetMethod(nameof(TerrainFeature.draw),
            BindingFlags.Instance | BindingFlags.Public, [typeof(SpriteBatch)]);
        var h = ModEntry.HarmonyObj;
        var prefixDrawMethod = motionManager.GetType().GetMethod("PrefixDraw", BindingFlags.Static | BindingFlags.NonPublic);
        if (prefixDrawMethod != null)
            drawMethodOriginal = h.Patch(drawMethod, prefix: new(prefixDrawMethod));
    }

    public static Farmer player => Game1.player;

    protected void TickedInternal(UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player == null)
            return;

        var map = player.currentLocation;
        if (_currentMap != map)
            OnInitNewMap(map);

        if (_currentMap == null)
            return;

        var objects = GetObjects(_featureType);
        foreach (var feature in objects)
        {
            var tile = feature.Tile;
            ObjectMotionContainer.TryGetValue(tile, out var motion);
            //first create obj
            if (motion == null)
            {
                motion = new(feature);
                ObjectMotionContainer[tile] = motion;
            }
            //set new obj alway
            motion.feature = feature;
        }
    }

    public virtual void OnInitNewMap(GameLocation newMap)
    {
        _currentMap = newMap;
        ObjectMotionContainer.Clear();
    }

    public abstract void Ticked(UpdateTickedEventArgs e);

    public TerrainFeature[] GetObjects(Type featureType)
    {
        //check terrain features class
        var baseType = featureType.BaseType;
        if (baseType == typeof(TerrainFeature))
        {
            return _currentMap.terrainFeatures.Values.AsParallel().Where(f => f.GetType() == featureType).ToArray();
        }
        else if (baseType == typeof(LargeTerrainFeature))
        {
            return _currentMap.largeTerrainFeatures.Where(f => f.GetType() == featureType).ToArray();
        }
        return null;
    }
}
