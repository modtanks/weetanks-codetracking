using UnityEngine;

public class DeathZoneScript : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Enemy" || other.transform.tag == "Player")
		{
			HealthTanks HTcol = other.GetComponent<HealthTanks>();
			HTcol.health = -999;
		}
		else if (other.transform.tag == "Bullet")
		{
			PlayerBulletScript bs = other.GetComponent<PlayerBulletScript>();
			if ((bool)bs)
			{
				bs.TimesBounced = 99;
				return;
			}
			PlayerBulletScript myBullet = other.GetComponent<PlayerBulletScript>();
			myBullet.TimesBounced = 99;
		}
	}
}
