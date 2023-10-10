using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
	[SerializeField]
	private float _destroyInSeconds = 1f;

	protected void OnEnable()
	{
		Invoke(nameof(DestroySelf), _destroyInSeconds);
	}

	protected void OnDisable()
	{
		CancelInvoke(nameof(DestroySelf));
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}
}