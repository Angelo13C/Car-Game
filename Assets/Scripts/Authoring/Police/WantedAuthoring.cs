using Unity.Entities;
using UnityEngine;

public class WantedLevelAuthoring : MonoBehaviour
{
	[SerializeField] private int _initialStars;
	[SerializeField] private WantedLevel[] _wantedLevels;

	class Baker : Baker<WantedLevelAuthoring>
	{
		public override void Bake(WantedLevelAuthoring authoring)
		{
			var wantedLevels = AddBuffer<WantedLevel>();
			wantedLevels.CopyFrom(authoring._wantedLevels);

			if(authoring._wantedLevels.Length != 0)
			{
				var wanted = new Wanted {
					CurrentCrimePoints = wantedLevels[authoring._initialStars - 1].RequiredCrimePoints
				};

				AddComponent(wanted);
			}
		}
	}

	private void OnValidate() {
		_initialStars = Mathf.Clamp(_initialStars, _wantedLevels.Length > 0 ? 1 : 0, _wantedLevels.Length);
	}
}