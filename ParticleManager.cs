using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	[Header("Limits")]
	public int MaxTNTExplosions = 8;

	private int CurrentTNTExplosions = 0;

	public List<GameObject> TNTExplosions;

	private static ParticleManager _instance;

	private int AllowAnyways = 0;

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
		for (int i = TNTExplosions.Count - 1; i >= 0; i--)
		{
			if (TNTExplosions[i] == null)
			{
				TNTExplosions.RemoveAt(i);
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
