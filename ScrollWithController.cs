using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWithController : MonoBehaviour
{
	public ScrollRect SR;

	public Player player;

	public Vector2 ScrollPart;

	private Vector2 prev;

	public Transform content;

	public bool CalculatePositionAndChangeTexts;

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
		ScrollPart = SR.normalizedPosition;
		if (ScrollPart != prev && CalculatePositionAndChangeTexts)
		{
			prev = ScrollPart;
			if ((bool)content)
			{
				int childCount = content.childCount;
				int num = Mathf.RoundToInt((1f - ScrollPart.y) * (float)childCount);
				int num2 = 0;
				foreach (Transform item in content)
				{
					if (num2 < num - 5 || num2 > num + 5)
					{
						TextMeshProUGUI[] componentsInChildren = item.GetComponentsInChildren<TextMeshProUGUI>();
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							componentsInChildren[j].enabled = false;
						}
					}
					else
					{
						TextMeshProUGUI[] componentsInChildren = item.GetComponentsInChildren<TextMeshProUGUI>();
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							componentsInChildren[j].enabled = true;
						}
					}
					num2++;
				}
			}
		}
		if (vector.y < 0f || vector.y > 0f)
		{
			SR.verticalNormalizedPosition += vector.y / 16f;
		}
	}
}
