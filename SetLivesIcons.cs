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
		int num = ((GameMaster.instance.Lives < 7) ? GameMaster.instance.Lives : 7);
		for (int i = 0; i < num; i++)
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
		for (int i = 0; i < tanksIcons.Length; i++)
		{
			tanksIcons[i].SetBool("ShowLife", value: false);
		}
	}
}
