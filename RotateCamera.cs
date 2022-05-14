using UnityEngine;

public class RotateCamera : MonoBehaviour
{
	public bool isRotating;

	public Quaternion BeginRot;

	public bool canRotateCamera = true;

	public float speed = 3.5f;

	private float X;

	private float Y;

	private void Start()
	{
		BeginRot = base.transform.rotation;
	}

	private void Update()
	{
		if (!GameMaster.instance)
		{
			return;
		}
		if (GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode)
		{
			if (!canRotateCamera)
			{
				base.transform.rotation *= Quaternion.Euler(0f, Time.deltaTime / 0.25f, 0f);
			}
			if (!GameMaster.instance.inMenuMode && !MapEditorMaster.instance && canRotateCamera && Input.GetMouseButton(1))
			{
				base.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, (0f - Input.GetAxis("Mouse X")) * speed, 0f));
				X = base.transform.rotation.eulerAngles.x;
				Y = base.transform.rotation.eulerAngles.y;
				base.transform.rotation = Quaternion.Euler(X, Y, 0f);
			}
			else if (GameMaster.instance.inMenuMode && !MapEditorMaster.instance && canRotateCamera)
			{
				base.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * 0.25f, (0f - Input.GetAxis("Mouse X")) * 0.25f, 0f));
				X = base.transform.rotation.eulerAngles.x;
				Y = base.transform.rotation.eulerAngles.y;
				base.transform.rotation = Quaternion.Euler(X, Y, 0f);
				base.transform.rotation *= Quaternion.Euler(0f, Time.deltaTime / 0.25f, 0f);
				if (base.transform.rotation.eulerAngles.x < -10f)
				{
					base.transform.rotation = Quaternion.Euler(-10f, base.transform.rotation.eulerAngles.y, base.transform.rotation.eulerAngles.z);
				}
				else if (base.transform.rotation.eulerAngles.x > 10f)
				{
					base.transform.rotation = Quaternion.Euler(10f, base.transform.rotation.eulerAngles.y, base.transform.rotation.eulerAngles.z);
				}
			}
		}
		else
		{
			base.transform.rotation = BeginRot;
		}
	}
}
