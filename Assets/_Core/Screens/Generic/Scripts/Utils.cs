using UnityEngine;

public static class Utils
{
	public static void PlayRandomOneShot(this AudioSource audioSource, GameSound[] sounds)
	{
		if(audioSource == null)
		{
			return;
		}

		if(sounds == null || sounds.Length == 0)
		{
			return;
		}

		GameSound sound = sounds[Random.Range(0, sounds.Length)];

		audioSource.PlayOneShot(sound.Clip, sound.VolumeScaler);
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
