using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OneWayBlock : MonoBehaviour
{
	public Collider theblocker;

	public float blocker = 10f;

	public Animator blockAnimator;

	public bool fixer = false;

	public OneWayBlock wayblockScript;

	public bool fixField = false;

	public AudioSource mySource;

	public AudioClip repairSound;

	public AudioClip breakSound;

	public bool inField = false;

	public Collider other;

	public GameObject ImageIndicator;

	public GameObject Indicator;

	public GameObject CanvasIndicator;

	public float Volume;

	public float breakspeed = 2f;

	public float[] buildspeed;

	public List<Collider> MyColliders = new List<Collider>();

	public GameObject invisibleBlocker;

	public bool PlayerInMe;

	public Collider PlayerObject;

	public bool FastBreaker = false;

	private void Start()
	{
		CanvasIndicator.SetActive(value: true);
		DisableCollision();
		InvokeRepeating("Checker", 0.5f, 0.5f);
	}

	private void Checker()
	{
		List<Collider> ToRemove = new List<Collider>();
		PlayerInMe = false;
		foreach (Collider Coll in MyColliders)
		{
			if (Coll == null)
			{
				ToRemove.Add(Coll);
			}
			else if (Coll.tag == "Player")
			{
				PlayerObject = Coll;
				PlayerInMe = true;
			}
		}
		if (ToRemove.Count > 0)
		{
			for (int i = 0; i < ToRemove.Count; i++)
			{
				MyColliders.Remove(ToRemove[i]);
			}
		}
		if (MyColliders.Count < 1)
		{
			FastBreaker = false;
			DisableCollision();
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if ((collision.tag == "Player" || collision.tag == "Enemy") && !MyColliders.Contains(collision))
		{
			HealthTanks HT = collision.gameObject.GetComponent<HealthTanks>();
			if (HT.EnemyID == -13)
			{
				FastBreaker = true;
			}
			MyColliders.Add(collision);
			CheckEnteringObject(collision);
		}
		if ((!(blocker < 1f) || fixer) && fixer && !(collision.tag == "Player"))
		{
		}
	}

	private void CheckEnteringObject(Collider EnteredObject)
	{
		if (!fixer && EnteredObject.tag == "Enemy")
		{
			if (blocker > 0.5f)
			{
				if (!mySource.isPlaying)
				{
					mySource.volume = Volume;
					mySource.clip = breakSound;
					mySource.Play();
				}
				repareTheBlock();
				{
					foreach (Collider Enemy2 in MyColliders)
					{
						if (Enemy2.tag == "Enemy")
						{
							NavMeshAgent NMA2 = Enemy2.GetComponent<NavMeshAgent>();
							NMA2.isStopped = true;
						}
					}
					return;
				}
			}
			mySource.volume = 0f;
			mySource.clip = null;
			mySource.Stop();
			if (EnteredObject.tag == "Enemy")
			{
				foreach (Collider Enemy in MyColliders)
				{
					if (Enemy.tag == "Enemy")
					{
						NavMeshAgent NMA = Enemy.GetComponent<NavMeshAgent>();
						NMA.isStopped = false;
					}
				}
			}
			DisableCollision();
		}
		else if (EnteredObject.tag == "Player")
		{
			Physics.IgnoreCollision(EnteredObject, theblocker, ignore: false);
		}
	}

	private void CheckLeavingObject(Collider EnteredObject)
	{
	}

	private void disableTheBlock()
	{
		mySource.volume = 0f;
		mySource.clip = null;
		mySource.Stop();
		DisableCollision();
		if (invisibleBlocker.activeSelf && !fixer)
		{
			invisibleBlocker.SetActive(value: false);
			GenerateNavMeshSurface.instance.GenerateSurface();
		}
		foreach (Collider Enemy in MyColliders)
		{
			if (Enemy.tag == "Enemy")
			{
				NavMeshAgent NMA = Enemy.GetComponent<NavMeshAgent>();
				NMA.isStopped = false;
			}
		}
	}

	private void DisableCollision()
	{
		if (invisibleBlocker.activeSelf && !fixer)
		{
			invisibleBlocker.SetActive(value: false);
			GenerateNavMeshSurface.instance.GenerateSurface();
		}
	}

	private void EnableCollision(Collider collision)
	{
		if (!invisibleBlocker.activeSelf)
		{
			invisibleBlocker.SetActive(value: true);
		}
	}

	private void repareTheBlock()
	{
		if (!invisibleBlocker.activeSelf && !fixer)
		{
			invisibleBlocker.SetActive(value: true);
			GenerateNavMeshSurface.instance.GenerateSurface();
		}
	}

	private void OnTriggerStay(Collider collision)
	{
	}

	private void OnTriggerExit(Collider collision)
	{
		if (MyColliders.Contains(collision))
		{
			HealthTanks HT = collision.gameObject.GetComponent<HealthTanks>();
			if (HT.EnemyID == -13)
			{
				FastBreaker = false;
			}
			MyColliders.Remove(collision);
			CheckLeavingObject(collision);
		}
	}

	private void Update()
	{
		if (fixer && !PlayerInMe)
		{
			ImageIndicator.SetActive(value: false);
		}
		if (fixer && PlayerInMe && wayblockScript.MyColliders.Count < 1)
		{
			ImageIndicator.SetActive(value: true);
			RectTransform ImageRect = Indicator.GetComponent<RectTransform>();
			ImageRect.sizeDelta = new Vector2(0.5f * (float)Mathf.RoundToInt(wayblockScript.blocker), 0.25f);
			MoveTankScript MTS = PlayerObject.GetComponent<MoveTankScript>();
			wayblockScript.blocker += Time.deltaTime * (float)ZombieTankSpawner.instance.UpgradedBuildSpeeds[MTS.Upgrades[0]];
			if (wayblockScript.blocker > 9.5f)
			{
				mySource.volume = 0f;
				mySource.clip = null;
				mySource.Stop();
				ImageIndicator.SetActive(value: false);
			}
			else if (wayblockScript.blocker < 0f)
			{
				mySource.volume = 0f;
				mySource.clip = null;
				mySource.Stop();
				ImageIndicator.SetActive(value: false);
			}
			else if (!mySource.isPlaying)
			{
				mySource.volume = Volume;
				mySource.clip = repairSound;
				mySource.Play();
			}
		}
		else if (!fixer)
		{
			blockAnimator.SetFloat("Health", blocker);
			if (MyColliders.Count > 0 && blocker > 0.4f)
			{
				float addition = (FastBreaker ? 3f : 1f);
				blocker -= Time.deltaTime * breakspeed * addition;
			}
			else if (blocker > 9.5f)
			{
				mySource.volume = 0f;
				mySource.clip = null;
				mySource.Stop();
			}
			else if (blocker < 0.5f)
			{
				disableTheBlock();
			}
			else if (mySource.isPlaying && MyColliders.Count < 1)
			{
				mySource.volume = 0f;
				mySource.clip = null;
				mySource.Stop();
			}
		}
	}
}
