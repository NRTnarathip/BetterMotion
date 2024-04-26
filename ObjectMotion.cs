using StardewValley.TerrainFeatures;
using System.Reflection;

namespace BetterMotion;

public class ObjectMotion
{
    public TerrainFeature feature;
    public float motion;
    public Type type;
    public FieldInfo shakeRotationField;
    public float shakeRotation => (float)shakeRotationField.GetValue(feature);
    public ObjectMotion(TerrainFeature feature)
    {
        this.feature = feature;
        this.type = feature.GetType();
        shakeRotationField = type.GetField("shakeRotation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | BindingFlags.Public);
    }
    public void SetRotation(float rot)
    {
        shakeRotationField.SetValue(feature, rot);
    }
}
