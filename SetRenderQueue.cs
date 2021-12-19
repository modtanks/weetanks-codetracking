using TMPro;
using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	public int queue = 3000;

	private void Start()
	{
		GetComponent<TextMeshProUGUI>().material.renderQueue = queue;
	}

	private void Update()
	{
	}
}
