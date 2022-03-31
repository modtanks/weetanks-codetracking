using TMPro;
using UnityEngine;

public class DestroyableBox : MonoBehaviour
{
	public GameObject ObjectToSpawn;

	public GameObject DestructionParticles;

	public AudioClip BreakSound;

	public int AmountArmourPlates = 3;

	public TextMeshProUGUI TextAmountHolder;

	private void Start()
	{
		TextAmountHolder.text = AmountArmourPlates.ToString();
	}

	private void OnCollisionEnter(Collision other)
	{
		Destruct(other);
	}

	public void Destruct(Collision other)
	{
		if (!(other.gameObject.tag == "Bullet") && !(other.gameObject.tag == "Player"))
		{
			return;
		}
		if ((bool)GameMaster.instance)
		{
			SFXManager.instance.PlaySFX(BreakSound, 1f, null);
		}
		if (other.gameObject.tag == "Player")
		{
			HealthTanks HT = other.gameObject.GetComponent<HealthTanks>();
			if ((bool)HT && HT.health < 4)
			{
				HT.health += AmountArmourPlates;
				if (HT.maxHealth < 4)
				{
					HT.maxHealth += AmountArmourPlates;
				}
				if (HT.health > 4)
				{
					HT.health = 4;
				}
				if (HT.maxHealth > 4)
				{
					HT.maxHealth = 4;
				}
			}
		}
		else if (other.gameObject.tag == "Bullet")
		{
			HealthTanks HT2 = other.gameObject.GetComponent<PlayerBulletScript>().papaTank.GetComponent<HealthTanks>();
			if ((bool)HT2)
			{
				if (HT2.health_armour < 4)
				{
					HT2.health_armour += AmountArmourPlates;
					if (HT2.maxArmour < 4)
					{
						HT2.maxArmour += AmountArmourPlates;
					}
					if (HT2.health_armour > 4)
					{
						HT2.health_armour = 4;
					}
					if (HT2.health_armour > 4)
					{
						HT2.health_armour = 4;
					}
				}
				other.gameObject.GetComponent<PlayerBulletScript>().TimesBounced = 9999;
			}
		}
		GameObject Particles = Object.Instantiate(DestructionParticles, base.transform.position, Quaternion.identity);
		Object.Destroy(Particles, 3f);
		Object.Destroy(base.gameObject);
	}
}
