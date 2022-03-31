using UnityEngine;

public class SetLivesIcons : MonoBehaviour
{
	public Animator[] TanksIcons;

	private void Start()
	{
		SetIcons();
	}

	public void SetIcons()
	{
		int amount = ((GameMaster.instance.Lives < 7) ? GameMaster.instance.Lives : 7);
		for (int i = 0; i < amount; i++)
		{
			if (!TanksIcons[i].GetBool("ShowLife"))
			{
				TanksIcons[i].SetBool("ShowLife", value: true);
			}
			if (i == GameMaster.instance.Lives - 1 && i < TanksIcons.Length)
			{
				for (int j = i + 1; j < TanksIcons.Length; j++)
				{
					if (TanksIcons[j].GetBool("ShowLife"))
					{
						TanksIcons[j].SetBool("ShowLife", value: false);
					}
				}
			}
			if (i > TanksIcons.Length)
			{
				break;
			}
		}
	}

	public void RemoveIcons()
	{
		Animator[] tanksIcons = TanksIcons;
		foreach (Animator TankIcon in tanksIcons)
		{
			TankIcon.SetBool("ShowLife", value: false);
		}
	}
}
