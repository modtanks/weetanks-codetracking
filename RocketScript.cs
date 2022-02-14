using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
	public int state;

	public float landingOffset = 3f;

	public float impactRange = 1.25f;

	public Transform target;

	public GameObject explosionPrefab;

	public AudioClip[] DeadHit;

	public BoxCollider boxC;

	public AudioClip launchSound;

	public AudioClip incomingSound;

	public GameObject GroundTarget;

	public GameObject spawnedTarget;

	public int MyTeam = 2;

	public Light rocketLight;

	public bool isHuntingEnemies;

	public GameObject papaTank;

	public Transform root;

	public Animator theAnim;

	private bool PlayedDeadSound;

	public Vector3 TargetPosition;

	public Vector3 FlyDirection;

	private void Start()
	{
		rocketLight.enabled = false;
		boxC.enabled = false;
		if (papaTank != null)
		{
			ParticleSystem[] componentsInChildren = root.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop();
			}
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted && state > 0)
		{
			if ((bool)spawnedTarget)
			{
				Object.Destroy(spawnedTarget);
			}
			Object.Destroy(Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity).gameObject, 2f);
			Transform obj = root.Find("SmokeBullet");
			obj.GetComponent<ParticleSystem>().Stop();
			obj.parent = null;
			Object.Destroy(obj.gameObject, 5f);
			Object.Destroy(base.gameObject);
		}
		if (state == 1)
		{
			rocketLight.enabled = true;
			base.transform.Translate(Vector3.up * Time.deltaTime * 25f, Space.World);
			theAnim.SetBool("Swerving", value: true);
			if (base.transform.position.y >= 12f)
			{
				GetTarget();
			}
		}
		else if (state == 2)
		{
			base.transform.LookAt(new Vector3(TargetPosition.x, base.transform.position.y, TargetPosition.z));
			base.transform.RotateAround(base.transform.position, base.transform.right, 90f);
			Vector3 b = new Vector3(TargetPosition.x, base.transform.position.y, TargetPosition.z);
			Vector3 vector = FlyDirection;
			float num = Vector3.Distance(base.transform.position, b);
			if (num <= 6f && theAnim.GetBool("Swerving"))
			{
				theAnim.SetBool("Swerving", value: false);
			}
			if (num <= 3f)
			{
				float angle = Mathf.Lerp(0f, 90f, 1f - num / 4f);
				base.transform.RotateAround(base.transform.position, base.transform.right, angle);
				vector = Vector3.Lerp(b: new Vector3(FlyDirection.x, -2f, FlyDirection.z), a: FlyDirection, t: 1f - num / 4f);
			}
			base.transform.Translate(vector * Time.deltaTime * 15f, Space.World);
			if (num <= 0.6f)
			{
				state = 3;
			}
		}
		else if (state == 3)
		{
			base.transform.LookAt(new Vector3(base.transform.position.x, -100f, base.transform.position.z));
			base.transform.RotateAround(base.transform.position, base.transform.right, 90f);
			base.transform.Translate(-Vector3.up * Time.deltaTime * 25f, Space.World);
			theAnim.SetBool("Swerving", value: false);
			if (base.transform.position.y < 15f && !PlayedDeadSound)
			{
				DeadSound();
				PlayedDeadSound = true;
			}
		}
		if (base.transform.position.y < 0f)
		{
			Explode(base.gameObject, Override: true, GameEnd: false);
		}
		if (GameMaster.instance != null && MapEditorMaster.instance == null)
		{
			if (!isHuntingEnemies && state > 0 && (GameMaster.instance.AmountEnemyTanks < 1 || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive || !GameMaster.instance.GameHasStarted))
			{
				state = 4;
				Explode(base.gameObject, Override: true, GameEnd: true);
			}
		}
		else if ((bool)MapEditorMaster.instance && state > 0 && (GameMaster.instance.restartGame || !GameMaster.instance.GameHasStarted))
		{
			state = 4;
			Explode(base.gameObject, Override: true, GameEnd: true);
		}
	}

	public void Launch()
	{
		Play2DClipAtPoint(launchSound);
		ParticleSystem[] componentsInChildren = root.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		state = 1;
		base.transform.parent = null;
	}

	private IEnumerator Impact()
	{
		yield return new WaitForSeconds(1.5f);
		Play2DClipAtPoint(incomingSound);
		boxC.enabled = true;
		yield return new WaitForSeconds(2.5f);
		base.transform.RotateAround(base.transform.position, base.transform.right, 180f);
		ParticleSystem[] componentsInChildren = root.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		GetTarget();
	}

	private void GetTarget()
	{
		if (GameMaster.instance != null && !MapEditorMaster.instance)
		{
			if (isHuntingEnemies)
			{
				TargetTanks(onlyPlayers: true);
			}
			else
			{
				TargetTanks(onlyPlayers: false);
			}
		}
		else if (GameMaster.instance.isZombieMode)
		{
			TargetTanks(onlyPlayers: true);
		}
		else if ((bool)MapEditorMaster.instance)
		{
			TargetTanks(onlyPlayers: false);
		}
	}

	private void TargetTanks(bool onlyPlayers)
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] array;
		if (!onlyPlayers)
		{
			array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject in array)
			{
				EnemyAI component = gameObject.GetComponent<EnemyAI>();
				if ((bool)component && (component.MyTeam != MyTeam || component.MyTeam == 0) && component.gameObject != papaTank)
				{
					list.Add(gameObject);
				}
			}
		}
		array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject2 in array)
		{
			EnemyAI component2 = gameObject2.GetComponent<EnemyAI>();
			if ((bool)component2)
			{
				if (component2.MyTeam != MyTeam || component2.MyTeam == 0)
				{
					list.Add(gameObject2);
				}
				continue;
			}
			MoveTankScript component3 = gameObject2.GetComponent<MoveTankScript>();
			if (component3.MyTeam != MyTeam || component3.MyTeam == 0)
			{
				list.Add(gameObject2);
			}
		}
		if (list.Count > 0)
		{
			int index = Random.Range(0, list.Count);
			Vector3 vector = new Vector3(0f, 3f, 0f);
			vector = list[index].transform.position;
			state = 2;
			TargetPosition = vector + new Vector3(Random.Range(0f - landingOffset, landingOffset), 50f, Random.Range(0f - landingOffset, landingOffset));
			SpawnTarget(new Vector3(TargetPosition.x, 0.1f, TargetPosition.z));
			Vector3 vector2 = TargetPosition - base.transform.position;
			vector2.y = 0f;
			float magnitude = vector2.magnitude;
			FlyDirection = vector2 / magnitude;
			Debug.Log(FlyDirection);
			state = 2;
		}
		else
		{
			StartCoroutine(DestroyMe());
		}
	}

	private void SpawnTarget(Vector3 pos)
	{
		LayerMask layerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("FLOOR")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("CorkWall"));
		Debug.DrawRay(pos, Vector3.down * 150f, Color.red, 1f);
		if (Physics.Raycast(pos, Vector3.down * 150f, out var hitInfo, 150f, layerMask) && (hitInfo.collider.tag == "Solid" || hitInfo.collider.tag == "Floor"))
		{
			spawnedTarget = Object.Instantiate(GroundTarget, hitInfo.point + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		Explode(collision.gameObject, Override: false, GameEnd: false);
	}

	private void OnTriggerStay(Collider other)
	{
		Explode(other.gameObject, Override: false, GameEnd: false);
	}

	private IEnumerator ActivateParticles(float sec)
	{
		yield return new WaitForSeconds(sec);
		base.transform.Find("SmokeBullet").gameObject.SetActive(value: false);
		yield return new WaitForSeconds(0.1f);
		base.transform.Find("SmokeBullet").gameObject.SetActive(value: true);
		ParticleSystem[] componentsInChildren = root.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
	}

	private IEnumerator DestroyMe()
	{
		yield return new WaitForSeconds(6f);
		Explode(base.gameObject, Override: true, GameEnd: true);
	}

	private void Explode(GameObject collision, bool Override, bool GameEnd)
	{
		if (state == 3 && (collision.transform.tag == "Solid" || collision.transform.tag == "Player" || collision.transform.tag == "Enemy" || collision.transform.tag == "Floor" || Override))
		{
			state = 4;
			if ((bool)spawnedTarget)
			{
				Object.Destroy(spawnedTarget);
			}
			if (!GameEnd)
			{
				AreaDamageEnemies(base.transform.position, impactRange, 1.5f);
			}
			CameraShake component = Camera.main.GetComponent<CameraShake>();
			if ((bool)component)
			{
				component.StartCoroutine(component.Shake(0.08f, 0.12f));
			}
			Object.Destroy(Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity).gameObject, 2f);
			Transform obj = root.Find("SmokeBullet");
			obj.GetComponent<ParticleSystem>().Stop();
			obj.parent = null;
			Object.Destroy(obj.gameObject, 5f);
			Object.Destroy(base.gameObject);
		}
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] array = Physics.OverlapSphere(location, radius);
		foreach (Collider obj in array)
		{
			HealthTanks component = obj.GetComponent<HealthTanks>();
			LookAtMyDirection component2 = obj.GetComponent<LookAtMyDirection>();
			EnemyBulletScript component3 = obj.GetComponent<EnemyBulletScript>();
			MineScript component4 = obj.GetComponent<MineScript>();
			DestroyableWall component5 = obj.GetComponent<DestroyableWall>();
			ExplosiveBlock component6 = obj.GetComponent<ExplosiveBlock>();
			if ((bool)component6 && !component6.isExploding)
			{
				component6.StartCoroutine(component6.Death());
			}
			if (component != null)
			{
				if (component.ShieldFade != null)
				{
					if (component.ShieldFade.ShieldHealth > 0)
					{
						component.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						component.DamageMe(1);
					}
				}
				else
				{
					component.DamageMe(1);
				}
			}
			if (component2 != null)
			{
				component2.BounceAmount = 999;
			}
			if (component3 != null)
			{
				component3.BounceAmount = 999;
			}
			if (component4 != null)
			{
				component4.DetinationTime = 0f;
			}
			if (component5 != null)
			{
				component5.StartCoroutine(component5.destroy());
			}
		}
		array = Physics.OverlapSphere(location, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			DestroyableBlock component7 = array[i].GetComponent<DestroyableBlock>();
			if (component7 != null)
			{
				component7.blockHealth--;
			}
		}
	}

	private void DeadSound()
	{
		int maxExclusive = DeadHit.Length;
		int num = Random.Range(0, maxExclusive);
		SFXManager.instance.PlaySFX(DeadHit[num], 0.6f, null);
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		obj.transform.tag = "Temp";
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
