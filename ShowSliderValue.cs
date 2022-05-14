using UnityEngine;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
	private Slider MySlider;

	public bool IsSelected;

	private void Start()
	{
		MySlider = GetComponent<Slider>();
	}

	public void ShowValue()
	{
		IsSelected = true;
		MapEditorMaster.instance.OnCursorText.text = (Mathf.Round(MySlider.value * 100f) / 100f).ToString();
		MapEditorMaster.instance.OnCursorText.transform.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (IsSelected && Input.GetMouseButtonUp(0))
		{
			IsSelected = false;
			MapEditorMaster.instance.OnCursorText.transform.gameObject.SetActive(value: false);
		}
	}
}
