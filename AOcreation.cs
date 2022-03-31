using UnityEngine;

public class AOcreation : MonoBehaviour
{
	public GameObject AO;

	public float posYoffset = 1f;

	public bool randomRotation;

	public bool randomRotationOnlyY;

	public bool randomTextureTiling;

	public float minTiling;

	public float maxTiling;

	private Renderer rend;

	public float xRot = 90f;

	public bool onlyY;

	private void Awake()
	{
	}

	private void Start()
	{
		rend = GetComponent<Renderer>();
		if (randomRotation)
		{
			int x = Random.Range(0, 4) * 90;
			int y2 = Random.Range(0, 4) * 90;
			int z = Random.Range(0, 4) * 90;
			base.transform.rotation = Quaternion.Euler(x, y2, z);
		}
		else if (randomRotationOnlyY)
		{
			int y = Random.Range(0, 4) * 90;
			if (!onlyY)
			{
				base.transform.rotation = Quaternion.Euler(base.transform.rotation.x, y, base.transform.rotation.z);
			}
			else
			{
				base.transform.rotation = Quaternion.Euler(xRot, y, 0f);
			}
		}
		if (randomTextureTiling)
		{
			float scale = Random.Range(minTiling, maxTiling);
			if ((bool)rend)
			{
				rend.material.mainTextureScale = new Vector2(scale, scale);
			}
		}
		if ((bool)AO)
		{
			GameObject ao = Object.Instantiate(AO, new Vector3(base.transform.position.x, base.transform.position.y - posYoffset + 0.02f, base.transform.position.z), Quaternion.identity);
			ao.transform.localRotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
			ao.transform.parent = base.transform;
		}
		if (GetComponent<SnowEnabler>() != null && !GameMaster.instance.CM)
		{
			GetComponent<SnowEnabler>().CheckSnow();
		}
	}
}
