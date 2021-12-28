using UnityEngine;

public class TankeyTownMaster : MonoBehaviour
{
	private static TankeyTownMaster _instance;

	public static TankeyTownMaster instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}
}
