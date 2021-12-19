using System.Collections;
using UnityEngine;

public class ExplosiveBlock : MonoBehaviour
{
	public GameObject deathExplosion;

	public AudioClip Deathsound;

	public AudioClip FuseSound;

	public bool isExploding;

	public bool isFusing;

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collision!:" + collision.collider.name + collision.collider.tag);
		if ((bool)collision.collider.GetComponent<PlayerBulletScript>() && !isFusing)
		{
			StartFusing();
		}
	}

	public void StartFusing()
	{
		Animator component = GetComponent<Animator>();
		GameMaster.instance.Play2DClipAtPoint(FuseSound, 1f);
		component.SetBool("InitiateDestroy", value: true);
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
		GameObject obj = Object.Instantiate(deathExplosion, base.transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
		CameraShake component = Camera.main.GetComponent<CameraShake>();
		if ((bool)component)
		{
			component.StartCoroutine(component.Shake(0.1f, 0.15f));
		}
		Object.Destroy(obj.gameObject, 4f);
		if (GameMaster.instance.GameHasStarted || GameMaster.instance.isZombieMode)
		{
			GameMaster.instance.Play2DClipAtPoint(Deathsound, 1f);
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
					component.Play2DClipAtPoint(component.Buzz);
				}
				if (GameMaster.instance.GameHasStarted)
				{
					if (component.ShieldFade.ShieldHealth > 0)
					{
						component.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						component.health--;
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
