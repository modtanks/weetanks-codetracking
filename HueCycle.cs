using TMPro;
using UnityEngine;

public class HueCycle : MonoBehaviour
{
	public float time = 3f;

	public TextMeshProUGUI theText;

	public Color theColor;

	public float amount = 0f;

	private void Start()
	{
	}

	private void Update()
	{
		amount += Time.deltaTime;
		theColor = Color.HSVToRGB(amount / time % 1f, 1f, 1f);
		theText.color = theColor;
	}
}
