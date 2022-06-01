using UnityEngine;

public class DeathZoneScript : MonoBehaviour
{
	public bool OnlyBullets;

	private void OnTriggerEnter(Collider other)
	{
		if ((other.transform.tag == "Enemy" || other.transform.tag == "Player") && !OnlyBullets)
		{
			other.GetComponent<HealthTanks>().health = -999;
		}
		else if (other.transform.tag == "Bullet")
		{
			PlayerBulletScript component = other.GetComponent<PlayerBulletScript>();
			if ((bool)component)
			{
				component.TimesBounced = 99;
			}
			else
			{
				other.GetComponent<PlayerBulletScript>().TimesBounced = 99;
			}
		}
	}
}
