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
				foreach (Text text3 in texts)
				{
					text3.text = TankCode[myID];
				}
			}
			else
			{
				Text[] texts2 = Texts;
				foreach (Text text2 in texts2)
				{
					text2.text = "";
				}
			}
		}
		else
		{
			Text[] texts3 = Texts;
			foreach (Text text in texts3)
			{
				text.text = "";
			}
		}
	}
}
