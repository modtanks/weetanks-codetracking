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
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 5f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (col.tag == "Mine")
			{
				Vector3 targetDir = col.transform.position - base.transform.position;
				Debug.Log("Position of that mine is " + col.transform.position.ToString());
				Debug.DrawRay(base.transform.position, targetDir * 3f, Color.blue);
				Debug.Log(string.Concat(str2: Vector3.Angle(targetDir, base.transform.forward).ToString(), str0: base.name, str1: "incomingAngle is "));
				if (Physics.Raycast(base.transform.position, targetDir * 3f, out var rayhit) && (rayhit.collider.tag == "Solid" || rayhit.collider.tag == "Enemy"))
				{
					Debug.Log(base.name + ": There is something in the way over the mine!");
				}
			}
		}
	}
}
