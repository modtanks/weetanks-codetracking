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
		Vector2 input = default(Vector2);
		for (int i = 0; i < ReInput.players.playerCount; i++)
		{
			Player p = ReInput.players.GetPlayer(i);
			if (p.isPlaying)
			{
				input.y = p.GetAxis("Look Vertically");
				if (input.y < 0f || input.y > 0f)
				{
					break;
				}
			}
		}
		if (input.y < 0f || input.y > 0f)
		{
			SR.verticalNormalizedPosition += input.y / 16f;
		}
	}
}
