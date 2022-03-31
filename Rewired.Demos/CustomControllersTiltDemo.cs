using UnityEngine;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class CustomControllersTiltDemo : MonoBehaviour
{
	public Transform target;

	public float speed = 10f;

	private CustomController controller;

	private Player player;

	private void Awake()
	{
		Screen.orientation = ScreenOrientation.Landscape;
		player = ReInput.players.GetPlayer(0);
		ReInput.InputSourceUpdateEvent += OnInputUpdate;
		controller = (CustomController)player.controllers.GetControllerWithTag(ControllerType.Custom, "TiltController");
	}

	private void Update()
	{
		if (!(target == null))
		{
			Vector3 dir = Vector3.zero;
			dir.y = player.GetAxis("Tilt Vertical");
			dir.x = player.GetAxis("Tilt Horizontal");
			if (dir.sqrMagnitude > 1f)
			{
				dir.Normalize();
			}
			dir *= Time.deltaTime;
			target.Translate(dir * speed);
		}
	}

	private void OnInputUpdate()
	{
		Vector3 acceleration = Input.acceleration;
		controller.SetAxisValue(0, acceleration.x);
		controller.SetAxisValue(1, acceleration.y);
		controller.SetAxisValue(2, acceleration.z);
	}
}
