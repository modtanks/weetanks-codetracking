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
			GameMaster.instance.Play2DClipAtPoint(BreakSound, 1f);
		}
		if (other.gameObject.tag == "Player")
		{
			HealthTanks component = other.gameObject.GetComponent<HealthTanks>();
			if ((bool)component && component.health < 4)
			{
				component.health += AmountArmourPlates;
				if (component.maxHealth < 4)
				{
					component.maxHealth += AmountArmourPlates;
				}
				if (component.health > 4)
				{
					component.health = 4;
				}
				if (component.maxHealth > 4)
				{
					component.maxHealth = 4;
				}
			}
		}
		else if (other.gameObject.tag == "Bullet")
		{
			HealthTanks component2 = other.gameObject.GetComponent<PlayerBulletScript>().papaTank.GetComponent<HealthTanks>();
			if ((bool)component2)
			{
				if (component2.health < 4)
				{
					component2.health += AmountArmourPlates;
					if (component2.maxHealth < 4)
					{
						component2.maxHealth += AmountArmourPlates;
					}
					if (component2.health > 4)
					{
						component2.health = 4;
					}
					if (component2.maxHealth > 4)
					{
						component2.maxHealth = 4;
					}
				}
				other.gameObject.GetComponent<PlayerBulletScript>().TimesBounced = 9999;
			}
		}
		Object.Destroy(Object.Instantiate(DestructionParticles, base.transform.position, Quaternion.identity), 3f);
		Object.Destroy(base.gameObject);
	}
}
