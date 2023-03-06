using Unity.Entities;

public struct Wanted : IComponentData
{
	public int CurrentCrimePoints;

	public int CalculateCurrentStars(DynamicBuffer<WantedLevel> wantedLevels)
	{
		for(var i = wantedLevels.Length - 1; i >= 0; i--)
		{
			if(CurrentCrimePoints >= wantedLevels[i].RequiredCrimePoints)
				return i + 1;
		}
		return -1;
	}

	public void GetCurrentWantedLevel(DynamicBuffer<WantedLevel> wantedLevels, out WantedLevel currentWantedLevel)
	{
		currentWantedLevel = wantedLevels[CalculateCurrentStars(wantedLevels) - 1];
	}
}

[System.Serializable]
[InternalBufferCapacity(5)]
public struct WantedLevel : IBufferElementData
{
	public int RequiredCrimePoints;

	public int MinPoliceCars;
}