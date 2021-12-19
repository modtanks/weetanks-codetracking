using UnityEngine;

public class SurvivalEvent : MonoBehaviour
{
	public bool isDoor;

	public Transform myDoor;

	public int price;

	[Header("Extra")]
	public bool ShowMessage;

	public GameObject Message;

	public GameObject NotEnoughMoneyPrefab;

	public GameObject Parent;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Message.SetActive(value: true);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.tag == "Player"))
		{
			return;
		}
		MoveTankScript component = other.GetComponent<MoveTankScript>();
		if (component.isPlayer2 && Input.GetButtonDown("P2 Abutton"))
		{
			if (SurvivalMaster.instance.MoneyP2 >= (float)price)
			{
				Debug.LogError("P2 Bought Door!");
				return;
			}
			Debug.LogError("Cant buy door!");
			GameObject obj = Object.Instantiate(NotEnoughMoneyPrefab, base.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
			obj.transform.SetParent(Parent.transform);
			obj.transform.rotation *= Quaternion.Euler(0f, 0f, 0f);
		}
		else if (!component.isPlayer2 && Input.GetButtonDown("P1Abutton"))
		{
			if (SurvivalMaster.instance.MoneyP1 >= (float)price)
			{
				Debug.LogError("P1 Bought Door!");
				return;
			}
			Debug.LogError("Cant buy door!");
			GameObject obj2 = Object.Instantiate(NotEnoughMoneyPrefab, base.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
			obj2.transform.SetParent(Parent.transform);
			obj2.transform.rotation *= Quaternion.Euler(0f, 0f, 0f);
		}
	}

	private void OnTriggerLeave(Collider other)
	{
		if (other.tag == "Player")
		{
			Message.SetActive(value: false);
		}
	}
}
