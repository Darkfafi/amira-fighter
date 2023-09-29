using UnityEngine;

public static class Utils
{
	public static ParticleSystem SetSortingOrder(this ParticleSystem rootParticleSystem, int sortingOrder)
	{
		ParticleSystemRenderer[] allParticleSystems = rootParticleSystem.GetComponentsInChildren<ParticleSystemRenderer>(true);

		for (int i = 0; i < allParticleSystems.Length; i++)
		{
			allParticleSystems[i].sortingOrder = sortingOrder;
		}

		return rootParticleSystem;
	}
}
