using UnityEngine;

public class ScrollMaterial : MonoBehaviour
{
	public float scrollSpeed;

	private Renderer rend;

	private void Start()
	{
		rend = GetComponent<Renderer>();
	}

	private void Update()
	{
		float num = Time.time * scrollSpeed;
		rend.material.SetTextureOffset("_MainTex", new Vector2(num, num));
	}
}
