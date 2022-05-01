using TMPro;
using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	public int queue = 3000;

	private void Start()
	{
		TextMeshProUGUI myText = GetComponent<TextMeshProUGUI>();
		Material mymat = myText.material;
		mymat.renderQueue = queue;
	}

	private void Update()
	{
	}
}
