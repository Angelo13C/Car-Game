
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class WeaponTriggerSystemGroup : ComponentSystemGroup { }

[UpdateAfter(typeof(WeaponTriggerSystemGroup))]
public class WeaponCanFireCheckSystemGroup : ComponentSystemGroup { }

[UpdateAfter(typeof(WeaponCanFireCheckSystemGroup))]
public class WeaponFireSystemGroup : ComponentSystemGroup { }