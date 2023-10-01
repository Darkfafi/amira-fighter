using UnityEngine;

public static class Utils
{
	public static void PlayRandomOneShot(this AudioSource audioSource, AudioClip[] clips, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volumeScale);
	}

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
