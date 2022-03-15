using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWithController : MonoBehaviour
{
	public ScrollRect SR;

	public Player player;

	private void Start()
	{
		SR = GetComponent<ScrollRect>();
	}

	private void Update()
	{
		Vector2 vector = default(Vector2);
		for (int i = 0; i < ReInput.players.playerCount; i++)
		{
			Player player = ReInput.players.GetPlayer(i);
			if (player.isPlaying)
			{
				vector.y = player.GetAxis("Look Vertically");
				if (vector.y < 0f || vector.y > 0f)
				{
					break;
				}
			}
		}
		if (vector.y < 0f || vector.y > 0f)
		{
			SR.verticalNormalizedPosition += vector.y / 16f;
		}
	}
}
