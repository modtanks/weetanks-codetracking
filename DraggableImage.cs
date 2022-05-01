using UnityEngine;

public class DraggableImage : MonoBehaviour
{
	public Vector2 _lastPos;

	public bool _isDragging = false;

	public Transform RotateObject;

	public float RotationSpeed = 3f;

	public void OnPointerDown()
	{
		_lastPos = Input.mousePosition;
		_isDragging = true;
	}

	public void LateUpdate()
	{
		if (_isDragging)
		{
			RotateObject.transform.RotateAround(RotateObject.transform.position, Vector3.up, (0f - Input.GetAxisRaw("Mouse X")) * RotationSpeed);
		}
	}

	public void OnPointerUp()
	{
		_isDragging = false;
	}
}
