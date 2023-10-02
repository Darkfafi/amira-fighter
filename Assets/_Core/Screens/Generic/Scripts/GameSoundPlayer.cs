using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameSoundPlayer : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audioSource = null;

	[SerializeField]
	private GameSound _sound = null;

	protected void OnValidate()
	{
		if(!_audioSource) _audioSource = GetComponent<AudioSource>();
	}

	protected void Awake()
	{
		_audioSource.PlayOneShot(_sound.Clip, _sound.VolumeScaler);
	}
}
