using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	[Header("Limits")]
	public int MaxTNTExplosions = 8;

	private int CurrentTNTExplosions;

	public List<GameObject> TNTExplosions;

	private static ParticleManager _instance;

	private int AllowAnyways;

	public int AllowAfter = 4;

	public static ParticleManager instance => _instance;

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

	private void CleanUpLists()
	{
		for (int num = TNTExplosions.Count - 1; num >= 0; num--)
		{
			if (TNTExplosions[num] == null)
			{
				TNTExplosions.RemoveAt(num);
				CurrentTNTExplosions--;
			}
		}
	}

	public bool CanDoTNTExplosion()
	{
		CleanUpLists();
		if (CurrentTNTExplosions < MaxTNTExplosions || AllowAnyways >= AllowAfter)
		{
			CurrentTNTExplosions++;
			AllowAnyways = 0;
			return true;
		}
		AllowAnyways++;
		return false;
	}
}
