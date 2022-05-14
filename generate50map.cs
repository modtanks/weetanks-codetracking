using UnityEngine;

public class generate50map : MonoBehaviour
{
	public int xsize = 20;

	public int zsize = 20;

	public GameObject prefabFloorTile;

	private void Start()
	{
		for (int i = 0; i < xsize; i++)
		{
			for (int j = 0; j < zsize; j++)
			{
				if (i != 0 || j != 0)
				{
					Object.Instantiate(prefabFloorTile, prefabFloorTile.transform.position + new Vector3(i * 2, 0f, -j * 2), Quaternion.identity, base.transform).transform.rotation = prefabFloorTile.transform.rotation;
				}
			}
		}
	}

	private void Update()
	{
	}
}
