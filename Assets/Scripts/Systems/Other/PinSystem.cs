using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[DisableAutoCreation]
[BurstCompile]
public partial struct PinSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (pin, transform) in SystemAPI.Query<Pin, TransformAspect>())
        {
            var pinnedEntityTransform = SystemAPI.GetComponent<LocalToWorld>(pin.PinnedEntity);
            transform.LocalPosition = pinnedEntityTransform.Position;
            transform.LocalRotation = pinnedEntityTransform.Rotation;
        }
    }
}