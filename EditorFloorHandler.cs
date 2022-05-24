using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditorFloorHandler : MonoBehaviour
{
	private TMP_Dropdown MyDropdown;

	private void Awake()
	{
		Debug.Log("START FLOOORRR");
		MyDropdown = GetComponent<TMP_Dropdown>();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < 5; i++)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
			optionData.text = GlobalAssets.instance.TheFloors[i].FloorName;
			list.Add(optionData);
		}
		for (int j = 0; j < GlobalAssets.instance.StockDatabase.Count; j++)
		{
			if (GlobalAssets.instance.StockDatabase[j].IsMapEditorFloor)
			{
				Debug.Log("YES!! FLOOORRR");
				if (AccountMaster.instance.Inventory.InventoryItems.Contains(GlobalAssets.instance.StockDatabase[j].ItemID))
				{
					Debug.Log("FOUND FLOOORRR");
					TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData();
					optionData2.text = GlobalAssets.instance.StockDatabase[j].ItemName;
					list.Add(optionData2);
				}
			}
		}
		MyDropdown.options = list;
	}
}
