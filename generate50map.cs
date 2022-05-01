using UnityEngine;

public class generate50map : MonoBehaviour
{
	public int xsize = 20;

	public int zsize = 20;

	public GameObject prefabFloorTile;

	private void Start()
	{
		for (int x = 0; x < xsize; x++)
		{
			for (int z = 0; z < zsize; z++)
			{
				if (x != 0 || z != 0)
				{
					GameObject spawnedIn = Object.Instantiate(prefabFloorTile, prefabFloorTile.transform.position + new Vector3(x * 2, 0f, -z * 2), Quaternion.identity, base.transform);
					spawnedIn.transform.rotation = prefabFloorTile.transform.rotation;
				}
			}
		}
	}

	private void Update()
	{
	}
}
