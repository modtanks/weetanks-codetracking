using UnityEngine;

public class BounceWallScript : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		ContactPoint contactPoint = collision.contacts[0];
		Vector3 point = contactPoint.point;
		Rigidbody component = collision.gameObject.GetComponent<Rigidbody>();
		if (component != null && (collision.transform.tag == "Player" || collision.transform.tag == "Enemy" || collision.transform.tag == "Boss"))
		{
			Vector3 vector = component.transform.position - point;
			vector.y = 0f;
			component.AddForce(vector * 50f, ForceMode.Impulse);
			SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.WallBoingSounds);
		}
	}
}
