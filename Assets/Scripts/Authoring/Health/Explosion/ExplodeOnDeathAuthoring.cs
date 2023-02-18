using Unity.Entities;
using UnityEngine;

public class ExplodeOnDeathAuthoring : MonoBehaviour
{
	class Baker : Baker<ExplodeOnDeathAuthoring>
	{
		public override void Bake(ExplodeOnDeathAuthoring authoring)
		{
			AddComponent(new ExplodeOnDeath());
		}
	}
}