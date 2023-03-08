using Unity.Entities;

public struct WeaponTrigger : IComponentData
{
    public bool IsTriggered;
    public bool ShouldFire;
    public byte BulletsToFire;

    public byte BulletsToFireCount => ShouldFire ? BulletsToFire : (byte) 0;

    public static WeaponTrigger Resetted => new WeaponTrigger {
        IsTriggered = false,
        ShouldFire = false,
        BulletsToFire = 0
    };

    public void Trigger()
    {
        IsTriggered = true;
        ShouldFire = true;
    }
}