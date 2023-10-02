using UnityEngine;

[System.Serializable]
public class GameSound
{
	[field: SerializeField]
	public AudioClip Clip
	{
		get; private set;
	}

	[field: SerializeField]
	[field: Range(0f, 1f)]
	public float VolumeScaler
	{
		get; private set;
	} = 1f;
}
