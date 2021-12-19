using UnityEngine;
using UnityEngine.UI;

public class TankIdentifierScript : MonoBehaviour
{
	public string[] TankCode;

	public int myID = -1;

	public HealthTanks HT;

	public Text[] Texts;

	private void Start()
	{
		HT = base.transform.parent.gameObject.GetComponent<HealthTanks>();
		myID = HT.EnemyID;
		if (OptionsMainMenu.instance != null)
		{
			if (!OptionsMainMenu.instance.MarkedTanks)
			{
				Object.Destroy(base.gameObject);
			}
			else if (myID > -1)
			{
				Text[] texts = Texts;
				for (int i = 0; i < texts.Length; i++)
				{
					texts[i].text = TankCode[myID];
				}
			}
			else
			{
				Text[] texts = Texts;
				for (int i = 0; i < texts.Length; i++)
				{
					texts[i].text = "";
				}
			}
		}
		else
		{
			Text[] texts = Texts;
			for (int i = 0; i < texts.Length; i++)
			{
				texts[i].text = "";
			}
		}
	}
}
