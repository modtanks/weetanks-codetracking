using UnityEngine;

public class PlaySound : MonoBehaviour
{
	public AudioClip Sound;

	public void PlayASound()
	{
		if ((bool)GameMaster.instance)
		{
			GameMaster.instance.Play2DClipAtPoint(Sound, 1f);
		}
	}
}
