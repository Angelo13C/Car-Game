using Unity.Entities;

public struct Wanted : IComponentData
{
	public int CurrentCrimePoints;

	public int CalculateCurrentStars(DynamicBuffer<WantedLevel> wantedLevels)
	{
		for(var i = 1; i < wantedLevels.Length; i++)
		{
			if(CurrentCrimePoints <= wantedLevels[i].RequiredCrimePoints)
				return i + 1;
		}
		return 1;
	}
}

[System.Serializable]
[InternalBufferCapacity(5)]
public struct WantedLevel : IBufferElementData
{
	public int RequiredCrimePoints;
}