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
			HealthTanks component = other.gameObject.GetComponent<HealthTanks>();
			if ((bool)component && component.health_armour < 4)
			{
				component.health_armour += AmountArmourPlates;
				if (component.maxArmour < 4)
				{
					component.maxArmour += AmountArmourPlates;
				}
				if (component.health_armour > 4)
				{
					component.health_armour = 4;
				}
				if (component.health_armour > 4)
				{
					component.health_armour = 4;
				}
			}
		}
		else if (other.gameObject.tag == "Bullet")
		{
			HealthTanks component2 = other.gameObject.GetComponent<PlayerBulletScript>().papaTank.GetComponent<HealthTanks>();
			if ((bool)component2)
			{
				if (component2.health_armour < 4)
				{
					component2.health_armour += AmountArmourPlates;
					if (component2.maxArmour < 4)
					{
						component2.maxArmour += AmountArmourPlates;
					}
					if (component2.health_armour > 4)
					{
						component2.health_armour = 4;
					}
					if (component2.health_armour > 4)
					{
						component2.health_armour = 4;
					}
				}
				other.gameObject.GetComponent<PlayerBulletScript>().TimesBounced = 9999;
			}
		}
		Object.Destroy(Object.Instantiate(DestructionParticles, base.transform.position, Quaternion.identity), 3f);
		Object.Destroy(base.gameObject);
	}
}
