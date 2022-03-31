using UnityEngine;
using UnityEngine.UI;

public class LoadingIconScript : MonoBehaviour
{
	public Texture[] frames;

	public int framesPerSecond = 15;

	public RawImage RI;

	public bool Play = false;

	private void Start()
	{
		RI.color = Color.clear;
	}

	private void Update()
	{
		if (Play)
		{
			RI.color = Color.white;
			int index = Mathf.RoundToInt(Time.time * (float)framesPerSecond % (float)frames.Length);
			if (index != frames.Length)
			{
				RI.texture = frames[index];
			}
		}
	}
}
