using UnityEngine;

public class CursorTrail : MonoBehaviour
{
	public Color trailColor = new Color(1f, 0f, 0.38f);

	public float distanceFromCamera = 5f;

	public float startWidth = 0.1f;

	public float endWidth;

	public float trailTime = 0.24f;

	private Transform trailTransform;

	private Camera thisCamera;

	private void Start()
	{
		thisCamera = GetComponent<Camera>();
		GameObject gameObject = new GameObject("Mouse Trail");
		trailTransform = gameObject.transform;
		TrailRenderer trailRenderer = gameObject.AddComponent<TrailRenderer>();
		trailRenderer.time = -1f;
		MoveTrailToCursor(Input.mousePosition);
		trailRenderer.time = trailTime;
		trailRenderer.startWidth = startWidth;
		trailRenderer.endWidth = endWidth;
		trailRenderer.numCapVertices = 2;
		trailRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
		trailRenderer.sharedMaterial.color = trailColor;
		trailRenderer.sortingOrder = 2;
		trailRenderer.sortingLayerName = "Trails";
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
