using UnityEngine;

public class DIfficultyChange : MonoBehaviour
{
	public bool isSpotLightDarkMission;

	public Light spotLight;

	public float[] spotLightSizes;

	private void Start()
	{
		if (isSpotLightDarkMission)
		{
			spotLight.spotAngle = spotLightSizes[OptionsMainMenu.instance.currentDifficulty];
		}
	}

	private void Update()
	{
	}
}
