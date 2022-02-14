using UnityEngine;

public class GlobalAssets : MonoBehaviour
{
	private static GlobalAssets _instance;

	public AudioDatabase AudioDB;

	public TankeyTownStockItem[] StockDatabase;

	public static GlobalAssets instance => _instance;

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

	private void Start()
	{
	}

	private void Update()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
