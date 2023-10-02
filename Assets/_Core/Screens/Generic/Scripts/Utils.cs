using UnityEngine;

public static class Utils
{
	public static bool PlayRandomOneShot(this AudioSource audioSource, GameSound[] sounds)
	{
		if(audioSource == null)
		{
			return false;
		}

		if(sounds == null || sounds.Length == 0)
		{
			return false;
		}

		GameSound sound = sounds[Random.Range(0, sounds.Length)];
		audioSource.PlayOneShot(sound.Clip, sound.VolumeScaler);
		return true;
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
