using System.Collections;
using UnityEngine;

public class InstantiateOneParticle : MonoBehaviour
{
	public GameObject Particle;

	public AudioClip[] Sound;

	public AudioClip[] ChargedSound;

	public bool IsTowerCharged;

	private void Start()
	{
		StartCoroutine("clouds");
	}

	private IEnumerator clouds()
	{
		if (IsTowerCharged)
		{
			if (ChargedSound != null)
			{
				if (ChargedSound.Length != 0)
				{
					int ClipToPlay2 = Random.Range(0, ChargedSound.Length);
					if (ChargedSound[ClipToPlay2] != null)
					{
						SFXManager.instance.PlaySFX(ChargedSound[ClipToPlay2], 1f, null);
					}
				}
				else
				{
					Debug.LogError("sound missing");
				}
			}
		}
		else if (Sound != null && Sound.Length != 0)
		{
			int ClipToPlay = Random.Range(0, Sound.Length);
			if (Sound[ClipToPlay] != null)
			{
				SFXManager.instance.PlaySFX(Sound[ClipToPlay], 1f, null);
			}
		}
		yield return new WaitForSeconds(0.04f);
		Vector3 rot2 = base.transform.rotation.eulerAngles;
		rot2 = new Vector3(rot2.x, rot2.y + 180f, rot2.z - 90f);
		if ((bool)Particle)
		{
			GameObject poof = Object.Instantiate(Particle, base.transform.position, Quaternion.Euler(rot2));
			Object.Destroy(poof.gameObject, 2f);
		}
	}
}
