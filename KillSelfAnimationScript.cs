using UnityEngine;

public class KillSelfAnimationScript : MonoBehaviour
{
	public void KillSelf()
	{
		Object.Destroy(base.gameObject);
	}
}
