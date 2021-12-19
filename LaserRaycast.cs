using UnityEngine;

public class LaserRaycast : MonoBehaviour
{
	public LineRenderer myLine;

	private void Start()
	{
		myLine = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		Vector3 vector = base.transform.forward - base.transform.position;
		Debug.DrawRay(base.transform.position, vector * 50f, Color.green, 0.1f);
		LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
		if (Physics.Raycast(base.transform.position, vector, out var hitInfo, 50f, layerMask))
		{
			float num = Vector3.Distance(base.transform.position, hitInfo.point);
			Vector3 position = new Vector3(0f, 0f, num / 4f);
			myLine.SetPosition(1, position);
		}
	}
}
