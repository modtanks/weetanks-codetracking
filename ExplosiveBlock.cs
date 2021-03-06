using System.Collections;
using UnityEngine;

public class ExplosiveBlock : MonoBehaviour
{
	public GameObject deathExplosion;

	public AudioClip Deathsound;

	public AudioClip FuseSound;

	public bool isExploding;

	public bool isFusing;

	public ParticleSystem FuseSystem;

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collision!:" + collision.collider.name + collision.collider.tag);
		PlayerBulletScript component = collision.collider.GetComponent<PlayerBulletScript>();
		if (component.IsAirBullet)
		{
			isFusing = false;
			GetComponent<Animator>().speed = 0f;
			FuseSystem.Stop();
		}
		else if ((bool)component && !isFusing)
		{
			StartFusing();
		}
	}

	public void StartFusing()
	{
		FuseSystem.Play();
		Animator component = GetComponent<Animator>();
		SFXManager.instance.PlaySFX(FuseSound, 1f, null);
		component.SetBool("InitiateDestroy", value: true);
		component.speed = 1f;
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
		if (ParticleManager.instance.CanDoTNTExplosion())
		{
			GameObject gameObject = Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
			Object.Destroy(gameObject.gameObject, 4f);
			ParticleManager.instance.TNTExplosions.Add(gameObject);
			CameraShake component = Camera.main.GetComponent<CameraShake>();
			if ((bool)component)
			{
				component.StartCoroutine(component.Shake(0.1f, 0.15f));
			}
			if (GameMaster.instance.GameHasStarted || GameMaster.instance.isZombieMode)
			{
				SFXManager.instance.PlaySFX(Deathsound, 1f, null);
			}
		}
		Object.Destroy(base.gameObject);
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] array = Physics.OverlapSphere(location, radius);
		foreach (Collider obj in array)
		{
			HealthTanks component = obj.GetComponent<HealthTanks>();
			PlayerBulletScript component2 = obj.GetComponent<PlayerBulletScript>();
			MineScript component3 = obj.GetComponent<MineScript>();
			DestroyableWall component4 = obj.GetComponent<DestroyableWall>();
			WeeTurret component5 = obj.GetComponent<WeeTurret>();
			ExplosiveBlock component6 = obj.GetComponent<ExplosiveBlock>();
			if ((bool)component6 && !component6.isExploding)
			{
				component6.StartCoroutine(component6.Death());
			}
			if (component5 != null && GameMaster.instance.GameHasStarted)
			{
				component5.Health--;
			}
			if (component != null && !component.immuneToExplosion)
			{
				if (GameMaster.instance.isZombieMode && component.health > 1 && GameMaster.instance.GameHasStarted)
				{
					SFXManager.instance.PlaySFX(component.Buzz);
				}
				if (GameMaster.instance.GameHasStarted)
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
			}
			if (component2 != null)
			{
				if (!component2.isSilver)
				{
					component2.TimesBounced = 999;
				}
				if (!component2.isElectric)
				{
					component2.TimesBounced = 999;
				}
			}
			if (component3 != null)
			{
				component3.DetinationTime = 0f;
			}
			if (component4 != null)
			{
				if (component4.IsStone)
				{
					component4.StartCoroutine(component4.stonedestroy());
				}
				else
				{
					component4.StartCoroutine(component4.destroy());
				}
			}
		}
		array = Physics.OverlapSphere(location, radius * 2f);
		foreach (Collider collider in array)
		{
			Rigidbody component7 = collider.GetComponent<Rigidbody>();
			if (component7 != null && (collider.tag == "Player" || collider.tag == "Enemy"))
			{
				float num = Vector3.Distance(component7.transform.position, base.transform.position);
				float num2 = (radius * 2f - num) * 2f;
				Vector3 vector = component7.transform.position - base.transform.position;
				component7.AddForce(vector * num2, ForceMode.Impulse);
			}
		}
	}
}
