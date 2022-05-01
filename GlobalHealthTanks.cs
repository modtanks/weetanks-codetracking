using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHealthTanks : MonoBehaviour
{
	[Serializable]
	public class TankStats
	{
		public int TankID;

		public int TankSpeed;

		public int BodyTurnSpeed;

		public int DifficultyDodging;

		public int NotSeeBulletChancePercentage;

		public int Accuracy;

		public int MineLaySpeed;

		public bool CanLayMines;

		public bool HasRockets;

		public float RocketShootSpeed;

		public int maxFiredBullets;

		public int HeadTurnSpeed;
	}

	private static GlobalHealthTanks _instance;

	public GameObject[] BloodSplatters;

	public GameObject[] Explosion;

	public AudioClip[] InPain;

	public List<TankStats> GlobalTankStats = new List<TankStats>();

	public static GlobalHealthTanks instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Update()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
