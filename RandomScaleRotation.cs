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
			float x2 = Random.Range(minRotX, maxRotX);
			float y2 = Random.Range(minRotY, maxRotY);
			float z2 = Random.Range(minRotZ, maxRotZ);
			base.transform.rotation = Quaternion.Euler(x2, y2, z2);
		}
		if (randomScale)
		{
			float x = Random.Range(minScaleX, maxScaleX);
			float y = Random.Range(minScaleY, maxScaleY);
			float z = Random.Range(minScaleZ, maxScaleZ);
			base.transform.localScale = new Vector3(x, y, z);
		}
	}
}
