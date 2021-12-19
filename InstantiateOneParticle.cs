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
					int num = Random.Range(0, ChargedSound.Length);
					if (ChargedSound[num] != null)
					{
						GameMaster.instance.Play2DClipAtPoint(ChargedSound[num], 1f);
					}
				}
				else
				{
					Debug.LogError("sound missing");
				}
			}
		}
		else if (Sound != null)
		{
			if (Sound.Length != 0)
			{
				int num2 = Random.Range(0, Sound.Length);
				if (Sound[num2] != null)
				{
					GameMaster.instance.Play2DClipAtPoint(Sound[num2], 1f);
				}
			}
			else
			{
				Debug.LogError("sound missing");
			}
		}
		yield return new WaitForSeconds(0.04f);
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y + 180f, eulerAngles.z - 90f);
		if ((bool)Particle)
		{
			Object.Destroy(Object.Instantiate(Particle, base.transform.position, Quaternion.Euler(eulerAngles)).gameObject, 2f);
		}
	}
}
