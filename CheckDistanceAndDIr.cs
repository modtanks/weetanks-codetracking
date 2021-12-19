using UnityEngine;

public class CheckDistanceAndDIr : MonoBehaviour
{
	private void Start()
	{
		InvokeRepeating("CheckForMines", 0.1f, 0.1f);
	}

	private void Update()
	{
	}

	public void CheckForMines()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 5f);
		foreach (Collider collider in array)
		{
			if (collider.tag == "Mine")
			{
				Vector3 vector = collider.transform.position - base.transform.position;
				Debug.Log("Position of that mine is " + collider.transform.position.ToString());
				Debug.DrawRay(base.transform.position, vector * 3f, Color.blue);
				Debug.Log(string.Concat(str2: Vector3.Angle(vector, base.transform.forward).ToString(), str0: base.name, str1: "incomingAngle is "));
				if (Physics.Raycast(base.transform.position, vector * 3f, out var hitInfo) && (hitInfo.collider.tag == "Solid" || hitInfo.collider.tag == "Enemy"))
				{
					Debug.Log(base.name + ": There is something in the way over the mine!");
				}
			}
		}
	}
}
