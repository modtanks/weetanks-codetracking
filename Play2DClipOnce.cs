using UnityEngine;

public class Play2DClipOnce : MonoBehaviour
{
	public AudioClip sound;

	public bool overrideGameStarted = false;

	private void Start()
	{
		if (GameMaster.instance != null && ((GameMaster.instance.AmountGoodTanks > 0 && GameMaster.instance.GameHasStarted) || overrideGameStarted))
		{
			SFXManager.instance.PlaySFX(sound);
		}
	}
}
