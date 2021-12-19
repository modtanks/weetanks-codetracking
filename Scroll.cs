using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour
{
	public float speed = 2f;

	public GameObject ImageOnPanel;

	public Texture NewTexture;

	private RawImage img;

	public float rawImageW = 15f;

	public float rawImageH = 15f;

	private void Start()
	{
		img = ImageOnPanel.GetComponent<RawImage>();
	}

	private void Update()
	{
		img.uvRect = new Rect(img.uvRect.x - speed / 1000f, img.uvRect.y - speed / 1000f, rawImageW, rawImageH);
	}
}
