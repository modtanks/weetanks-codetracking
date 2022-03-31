using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
	public int state = 0;

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

	private bool PlayedDeadSound = false;

	public Vector3 TargetPosition;

	public Vector3 FlyDirection;

	private void Start()
	{
		rocketLight.enabled = false;
		boxC.enabled = false;
		if (papaTank != null)
		{
			ParticleSystem[] childrenParticleSytems = root.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = childrenParticleSytems;
			foreach (ParticleSystem system in array)
			{
				system.Stop();
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
			GameObject poof = Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity);
			Object.Destroy(poof.gameObject, 2f);
			Transform PE = root.Find("SmokeBullet");
			ParticleSystem PEsystem = PE.GetComponent<ParticleSystem>();
			PEsystem.Stop();
			PE.parent = null;
			Object.Destroy(PE.gameObject, 5f);
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
			Vector3 TargetMyHeight = new Vector3(TargetPosition.x, base.transform.position.y, TargetPosition.z);
			Vector3 dir = FlyDirection;
			float dist = Vector3.Distance(base.transform.position, TargetMyHeight);
			float speed = 15f;
			if (dist <= 6f)
			{
				speed = 12f;
				if (theAnim.GetBool("Swerving"))
				{
					theAnim.SetBool("Swerving", value: false);
				}
			}
			if (dist <= 3f)
			{
				speed = 9f;
				float amountDown = Mathf.Lerp(0f, 90f, 1f - dist / 4f);
				base.transform.RotateAround(base.transform.position, base.transform.right, amountDown);
				dir = Vector3.Lerp(b: new Vector3(FlyDirection.x, -2f, FlyDirection.z), a: FlyDirection, t: 1f - dist / 4f);
			}
			base.transform.Translate(dir * Time.deltaTime * speed, Space.World);
			if (dist <= 0.6f)
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
		SFXManager.instance.PlaySFX(launchSound);
		ParticleSystem[] childrenParticleSytems = root.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = childrenParticleSytems;
		foreach (ParticleSystem system in array)
		{
			system.Play();
		}
		state = 1;
		base.transform.parent = null;
	}

	private IEnumerator Impact()
	{
		yield return new WaitForSeconds(1.5f);
		SFXManager.instance.PlaySFX(incomingSound);
		boxC.enabled = true;
		yield return new WaitForSeconds(2.5f);
		base.transform.RotateAround(base.transform.position, base.transform.right, 180f);
		ParticleSystem[] childrenParticleSytems = root.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = childrenParticleSytems;
		foreach (ParticleSystem system in array)
		{
			system.Play();
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
		List<GameObject> PossibleTargets = new List<GameObject>();
		if (!onlyPlayers)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject go2 in array)
			{
				EnemyAI EA2 = go2.GetComponent<EnemyAI>();
				if ((bool)EA2 && (EA2.MyTeam != MyTeam || EA2.MyTeam == 0) && EA2.gameObject != papaTank)
				{
					PossibleTargets.Add(go2);
				}
			}
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject go in array2)
		{
			EnemyAI EA = go.GetComponent<EnemyAI>();
			if ((bool)EA)
			{
				if (EA.MyTeam != MyTeam || EA.MyTeam == 0)
				{
					PossibleTargets.Add(go);
				}
				continue;
			}
			MoveTankScript MTS = go.GetComponent<MoveTankScript>();
			if (MTS.MyTeam != MyTeam || MTS.MyTeam == 0)
			{
				PossibleTargets.Add(go);
			}
		}
		if (PossibleTargets.Count > 0)
		{
			int target = Random.Range(0, PossibleTargets.Count);
			Vector3 playerLocation = new Vector3(0f, 3f, 0f);
			playerLocation = PossibleTargets[target].transform.position;
			state = 2;
			TargetPosition = CalculateEnemyPosition(PossibleTargets[target]);
			SpawnTarget(new Vector3(TargetPosition.x, 0.1f, TargetPosition.z));
			Vector3 heading = TargetPosition - base.transform.position;
			heading.y = 0f;
			float distance = heading.magnitude;
			FlyDirection = heading / distance;
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
		if (Physics.Raycast(pos, Vector3.down * 150f, out var hitPoint, 150f, layerMask) && (hitPoint.collider.tag == "Solid" || hitPoint.collider.tag == "Floor"))
		{
			spawnedTarget = Object.Instantiate(GroundTarget, hitPoint.point + new Vector3(0f, 0.3f, 0f), Quaternion.identity);
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
		ParticleSystem[] childrenParticleSytems = root.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = childrenParticleSytems;
		foreach (ParticleSystem system in array)
		{
			system.Play();
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
			CameraShake CS = Camera.main.GetComponent<CameraShake>();
			if ((bool)CS)
			{
				CS.StartCoroutine(CS.Shake(0.08f, 0.12f));
			}
			GameObject poof = Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity);
			Object.Destroy(poof.gameObject, 2f);
			Transform PE = root.Find("SmokeBullet");
			ParticleSystem PEsystem = PE.GetComponent<ParticleSystem>();
			PEsystem.Stop();
			PE.parent = null;
			Object.Destroy(PE.gameObject, 5f);
			Object.Destroy(base.gameObject);
		}
	}

	private Vector3 CalculateEnemyPosition(GameObject TargetObject)
	{
		Rigidbody rb = TargetObject.GetComponent<Rigidbody>();
		if (rb.velocity.magnitude > 0.5f)
		{
			EnemyAI EA = TargetObject.GetComponent<EnemyAI>();
			Vector3 calcPos;
			if ((bool)EA)
			{
				calcPos = ((!EA.DrivingBackwards) ? (TargetObject.transform.position + TargetObject.transform.forward * 2f) : (TargetObject.transform.position + -TargetObject.transform.forward * 2f));
			}
			else
			{
				MoveTankScript MTS = TargetObject.GetComponent<MoveTankScript>();
				calcPos = ((!MTS.DrivingBackwards) ? (TargetObject.transform.position + TargetObject.transform.forward * 2f) : (TargetObject.transform.position + -TargetObject.transform.forward * 2f));
			}
			Debug.DrawLine(calcPos, base.transform.position, Color.blue);
			return calcPos + new Vector3(Random.Range(0f - landingOffset, landingOffset), 50f, Random.Range(0f - landingOffset, landingOffset));
		}
		return TargetObject.transform.position + new Vector3(Random.Range(0f - landingOffset, landingOffset), 50f, Random.Range(0f - landingOffset, landingOffset));
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			HealthTanks enemy = col.GetComponent<HealthTanks>();
			LookAtMyDirection friendbullet = col.GetComponent<LookAtMyDirection>();
			EnemyBulletScript enemybullet = col.GetComponent<EnemyBulletScript>();
			MineScript mines = col.GetComponent<MineScript>();
			DestroyableWall DestroyWall = col.GetComponent<DestroyableWall>();
			ExplosiveBlock EB = col.GetComponent<ExplosiveBlock>();
			if ((bool)EB && !EB.isExploding)
			{
				EB.StartCoroutine(EB.Death());
			}
			if (enemy != null)
			{
				if (enemy.ShieldFade != null)
				{
					if (enemy.ShieldFade.ShieldHealth > 0)
					{
						enemy.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						enemy.DamageMe(1);
					}
				}
				else
				{
					enemy.DamageMe(1);
				}
			}
			if (friendbullet != null)
			{
				friendbullet.BounceAmount = 999;
			}
			if (enemybullet != null)
			{
				enemybullet.BounceAmount = 999;
			}
			if (mines != null)
			{
				mines.DetinationTime = 0f;
			}
			if (DestroyWall != null)
			{
				DestroyWall.StartCoroutine(DestroyWall.destroy());
			}
		}
		Collider[] smallRange = Physics.OverlapSphere(location, 1f);
		Collider[] array2 = smallRange;
		foreach (Collider col2 in array2)
		{
			DestroyableBlock destroyBlocks = col2.GetComponent<DestroyableBlock>();
			if (destroyBlocks != null)
			{
				destroyBlocks.blockHealth--;
			}
		}
	}

	private void DeadSound()
	{
		int lengthClips = DeadHit.Length;
		int randomPick = Random.Range(0, lengthClips);
		SFXManager.instance.PlaySFX(DeadHit[randomPick], 0.6f, null);
	}
}
