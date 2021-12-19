using UnityEngine;

public class RandomScaleRotation : MonoBehaviour
{
	public bool randomScale;

	public float minScaleX;

	public float maxScaleX;

	public float minScaleY;

	public float maxScaleY;

	public float minScaleZ;

	public float maxScaleZ;

	public bool randomRotation;

	public float minRotX;

	public float maxRotX;

	public float minRotY;

	public float maxRotY;

	public float minRotZ;

	public float maxRotZ;

	private void Start()
	{
		if (randomRotation)
		{
			float x = Random.Range(minRotX, maxRotX);
			float y = Random.Range(minRotY, maxRotY);
			float z = Random.Range(minRotZ, maxRotZ);
			base.transform.rotation = Quaternion.Euler(x, y, z);
		}
		if (randomScale)
		{
			float x2 = Random.Range(minScaleX, maxScaleX);
			float y2 = Random.Range(minScaleY, maxScaleY);
			float z2 = Random.Range(minScaleZ, maxScaleZ);
			base.transform.localScale = new Vector3(x2, y2, z2);
		}
	}
}
