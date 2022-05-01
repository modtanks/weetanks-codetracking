using UnityEngine;

public class CursorTrail : MonoBehaviour
{
	public Color trailColor = new Color(1f, 0f, 0.38f);

	public float distanceFromCamera = 5f;

	public float startWidth = 0.1f;

	public float endWidth = 0f;

	public float trailTime = 0.24f;

	private Transform trailTransform;

	private Camera thisCamera;

	private void Start()
	{
		thisCamera = GetComponent<Camera>();
		GameObject trailObj = new GameObject("Mouse Trail");
		trailTransform = trailObj.transform;
		TrailRenderer trail = trailObj.AddComponent<TrailRenderer>();
		trail.time = -1f;
		MoveTrailToCursor(Input.mousePosition);
		trail.time = trailTime;
		trail.startWidth = startWidth;
		trail.endWidth = endWidth;
		trail.numCapVertices = 2;
		trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
		trail.sharedMaterial.color = trailColor;
		trail.sortingOrder = 2;
		trail.sortingLayerName = "Trails";
	}

	private void Update()
	{
		MoveTrailToCursor(Input.mousePosition);
	}

	private void MoveTrailToCursor(Vector3 screenPosition)
	{
		trailTransform.position = thisCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));
	}
}
