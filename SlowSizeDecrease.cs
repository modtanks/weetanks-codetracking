using System.Collections;
using UnityEngine;

public class SlowSizeDecrease : MonoBehaviour
{
	private Vector3 StartScale;

	public Vector3 EndScale;

	public float TimeItTakes;

	private bool isDecreasing = false;

	private void Start()
	{
		TimeItTakes -= ((OptionsMainMenu.instance.currentDifficulty == 1) ? 30 : ((OptionsMainMenu.instance.currentDifficulty == 2) ? 45 : 60));
		StartScale = base.transform.localScale;
		if (!isDecreasing)
		{
			StartCoroutine(DecreaseSize());
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			base.transform.localScale = StartScale;
		}
		else if (!isDecreasing)
		{
			StartCoroutine(DecreaseSize());
		}
	}

	private IEnumerator DecreaseSize()
	{
		isDecreasing = true;
		float t = 0f;
		while (t < 1f)
		{
			if (!GameMaster.instance.GameHasStarted)
			{
				isDecreasing = false;
				break;
			}
			t += Time.deltaTime / TimeItTakes;
			base.transform.localScale = Vector3.Lerp(StartScale, EndScale, t);
			yield return null;
		}
	}
}
