using System.Collections;
using UnityEngine;

public class ExplosiveBlock : MonoBehaviour
{
	public GameObject deathExplosion;

	public AudioClip Deathsound;

	public AudioClip FuseSound;

	public bool isExploding = false;

	public bool isFusing = false;

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collision!:" + collision.collider.name + collision.collider.tag);
		PlayerBulletScript PBS = collision.collider.GetComponent<PlayerBulletScript>();
		if ((bool)PBS && !isFusing)
		{
			StartFusing();
		}
	}

	public void StartFusing()
	{
		Animator myAnimator = GetComponent<Animator>();
		SFXManager.instance.PlaySFX(FuseSound, 1f, null);
		myAnimator.SetBool("InitiateDestroy", value: true);
		isFusing = true;
	}

	public IEnumerator Death()
	{
		isExploding = true;
		yield return new WaitForSeconds(0.1f);
		if (GameMaster.instance.GameHasStarted || GameMaster.instance.isZombieMode)
		{
			AreaDamageEnemies(base.transform.position, 4.5f, 1f);
		}
		GameObject poof = Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
		CameraShake CS = Camera.main.GetComponent<CameraShake>();
		if ((bool)CS)
		{
			CS.StartCoroutine(CS.Shake(0.1f, 0.15f));
		}
		Object.Destroy(poof.gameObject, 4f);
		if (GameMaster.instance.GameHasStarted || GameMaster.instance.isZombieMode)
		{
			SFXManager.instance.PlaySFX(Deathsound, 1f, null);
		}
		Object.Destroy(base.gameObject);
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			HealthTanks enemy = col.GetComponent<HealthTanks>();
			PlayerBulletScript friendbullet = col.GetComponent<PlayerBulletScript>();
			MineScript mines = col.GetComponent<MineScript>();
			DestroyableWall DestroyWall = col.GetComponent<DestroyableWall>();
			WeeTurret WT = col.GetComponent<WeeTurret>();
			ExplosiveBlock EB = col.GetComponent<ExplosiveBlock>();
			if ((bool)EB && !EB.isExploding)
			{
				EB.StartCoroutine(EB.Death());
			}
			if (WT != null && GameMaster.instance.GameHasStarted)
			{
				WT.Health--;
			}
			if (enemy != null && !enemy.immuneToExplosion)
			{
				if (GameMaster.instance.isZombieMode && enemy.health > 1 && GameMaster.instance.GameHasStarted)
				{
					SFXManager.instance.PlaySFX(enemy.Buzz);
				}
				if (GameMaster.instance.GameHasStarted)
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
			}
			if (friendbullet != null)
			{
				if (!friendbullet.isSilver)
				{
					friendbullet.TimesBounced = 999;
				}
				if (!friendbullet.isElectric)
				{
					friendbullet.TimesBounced = 999;
				}
			}
			if (mines != null)
			{
				mines.DetinationTime = 0f;
			}
			if (DestroyWall != null)
			{
				if (DestroyWall.IsStone)
				{
					DestroyWall.StartCoroutine(DestroyWall.stonedestroy());
				}
				else
				{
					DestroyWall.StartCoroutine(DestroyWall.destroy());
				}
			}
		}
		Collider[] bigobjectsInRange = Physics.OverlapSphere(location, radius * 2f);
		Collider[] array2 = bigobjectsInRange;
		foreach (Collider col2 in array2)
		{
			Rigidbody rigi = col2.GetComponent<Rigidbody>();
			if (rigi != null && (col2.tag == "Player" || col2.tag == "Enemy"))
			{
				float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
				float force = (radius * 2f - distance) * 2f;
				Vector3 direction = rigi.transform.position - base.transform.position;
				rigi.AddForce(direction * force, ForceMode.Impulse);
			}
		}
	}
}
