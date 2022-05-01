using UnityEngine;

public class BounceWallScript : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		Vector3 pos = contact.point;
		Rigidbody rigi = collision.gameObject.GetComponent<Rigidbody>();
		if (rigi != null && (collision.transform.tag == "Player" || collision.transform.tag == "Enemy" || collision.transform.tag == "Boss"))
		{
			Vector3 direction = rigi.transform.position - pos;
			direction.y = 0f;
			rigi.AddForce(direction * 50f, ForceMode.Impulse);
			SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.WallBoingSounds);
		}
	}
}
