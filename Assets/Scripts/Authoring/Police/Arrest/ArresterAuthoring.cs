using Unity.Entities;
using UnityEngine;

public class ArresterAuthoring : MonoBehaviour
{
	[SerializeField] private float _arrestRadius;

	class Baker : Baker<ArresterAuthoring>
	{
		public override void Bake(ArresterAuthoring authoring)
		{
			var arrester = new Arrester {
				ArrestRadiusSqr = authoring._arrestRadius * authoring._arrestRadius
			};

			AddComponent(arrester);
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, _arrestRadius);
	}
}