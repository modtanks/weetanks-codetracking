using UnityEngine;

public class PlaySound : MonoBehaviour
{
	public AudioClip Sound;

	public void PlayASound()
	{
		if ((bool)GameMaster.instance)
		{
			SFXManager.instance.PlaySFX(Sound, 1f, null);
		}
	}
}
