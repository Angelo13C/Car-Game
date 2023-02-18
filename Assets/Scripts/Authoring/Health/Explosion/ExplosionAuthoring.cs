using Unity.Entities;
using UnityEngine;

public class ExplosionAuthoring : MonoBehaviour
{
	class Baker : Baker<ExplosionAuthoring>
	{
		public override void Bake(ExplosionAuthoring authoring)
		{
            var explosion = new Explosion();
            explosion.ShouldExplode = false;

            AddComponent(explosion);
		}
	}
}