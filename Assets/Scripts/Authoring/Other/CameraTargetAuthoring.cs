using Unity.Entities;
using UnityEngine;

public class CameraTargetAuthoring : MonoBehaviour
{
	class Baker : Baker<CameraTargetAuthoring>
	{
		public override void Bake(CameraTargetAuthoring authoring)
		{            
            AddComponent(new CameraTarget());
		}
	}
}