using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialMaster : MonoBehaviour
{
	private static TutorialMaster _instance;

	public Animator myAnimator;

	public TextMeshProUGUI TheMessage;

	public Vector2 TextSize;

	public RectTransform Box;

	public float Padding;

	public Vector2 StartingSize;

	public string message;

	public bool IsShowingTutorial = false;

	public static TutorialMaster instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
		StartingSize = Box.sizeDelta;
	}

	private void Update()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
		TextSize = TheMessage.GetRenderedValues(onlyVisibleCharacters: false);
	}

	public IEnumerator ExtendBox(Vector2 size)
	{
		float t = 0f;
		Vector2 OriginalScale = Box.sizeDelta;
		while (t < 1f)
		{
			t += Time.deltaTime * 3f;
			Box.sizeDelta = Vector2.Lerp(OriginalScale, size, easeInOutQuint(t));
			yield return null;
		}
		Box.sizeDelta = size;
	}

	private float easeInOutQuint(float x)
	{
		return ((double)x < 0.5) ? (4f * x * x * x) : (1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f);
	}

	public IEnumerator Inscend()
	{
		float t = 0f;
		Vector2 CurrentScale = Box.sizeDelta;
		while (t < 1f)
		{
			t += Time.deltaTime * 3f;
			Box.sizeDelta = Vector2.Lerp(CurrentScale, StartingSize, easeInOutQuint(t));
			yield return null;
		}
		Box.sizeDelta = StartingSize;
	}

	public void ShowTutorial(string message)
	{
		if (message.Length > 2 && !IsShowingTutorial)
		{
			IsShowingTutorial = true;
			myAnimator.SetBool("ShowTutorial", value: true);
			TheMessage.text = message;
			StartCoroutine(HideTutorialAgain());
		}
	}

	private IEnumerator HideTutorialAgain()
	{
		float WaitTime = 4f + Box.sizeDelta.x / 250f;
		yield return new WaitForSeconds(WaitTime);
		myAnimator.SetBool("ShowTutorial", value: false);
		IsShowingTutorial = false;
	}
}
