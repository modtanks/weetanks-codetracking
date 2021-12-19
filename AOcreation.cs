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
			int num = Random.Range(0, 4) * 90;
			int num2 = Random.Range(0, 4) * 90;
			int num3 = Random.Range(0, 4) * 90;
			base.transform.rotation = Quaternion.Euler(num, num2, num3);
		}
		else if (randomRotationOnlyY)
		{
			int num4 = Random.Range(0, 4) * 90;
			if (!onlyY)
			{
				base.transform.rotation = Quaternion.Euler(base.transform.rotation.x, num4, base.transform.rotation.z);
			}
			else
			{
				base.transform.rotation = Quaternion.Euler(xRot, num4, 0f);
			}
		}
		if (randomTextureTiling)
		{
			float num5 = Random.Range(minTiling, maxTiling);
			if ((bool)rend)
			{
				rend.material.mainTextureScale = new Vector2(num5, num5);
			}
		}
		if ((bool)AO)
		{
			GameObject obj = Object.Instantiate(AO, new Vector3(base.transform.position.x, base.transform.position.y - posYoffset + 0.02f, base.transform.position.z), Quaternion.identity);
			obj.transform.localRotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
			obj.transform.parent = base.transform;
		}
		if (GetComponent<SnowEnabler>() != null && !GameMaster.instance.CM)
		{
			GetComponent<SnowEnabler>().CheckSnow();
		}
	}
}
