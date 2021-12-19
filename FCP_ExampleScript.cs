using UnityEngine;

public class FCP_ExampleScript : MonoBehaviour
{
	public FlexibleColorPicker fcp;

	public Material material;

	public Color externalColor;

	private Color internalColor;

	private void Start()
	{
		internalColor = externalColor;
	}

	private void Update()
	{
		if (internalColor != externalColor)
		{
			fcp.color = externalColor;
			internalColor = externalColor;
		}
		material.color = fcp.color;
	}
}
