using UnityEngine;

public class GlobalHealthTanks : MonoBehaviour
{
	private static GlobalHealthTanks _instance;

	public GameObject[] BloodSplatters;

	public GameObject[] Explosion;

	public AudioClip[] InPain;

	public static GlobalHealthTanks instance => _instance;

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

	private void Update()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
