using UnityEngine;

public class OnCusorText : MonoBehaviour
{
	public float Yoffset;

	public bool enable;

	public bool Updated;

	public bool isNormalText = true;

	private void OnEnable()
	{
		enable = true;
		Updated = false;
	}

	private void OnDisable()
	{
		Updated = false;
		enable = false;
	}

	private void Update()
	{
		if ((enable && !Updated) || isNormalText)
		{
			if (isNormalText)
			{
				base.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z) + new Vector3(0f, Yoffset, 0f);
			}
			Updated = true;
		}
	}
}
