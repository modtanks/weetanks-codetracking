using UnityEngine;

public class AimAssistScript : MonoBehaviour
{
	private LineRenderer assist;

	private Vector2 input;

	private LookAtMouse parentLAM;

	private void Start()
	{
		parentLAM = GetComponentInParent<LookAtMouse>();
		assist = GetComponentInChildren<LineRenderer>();
	}

	private void Update()
	{
	}
}
