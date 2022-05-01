using UnityEngine;

public class FireworkSounds : MonoBehaviour
{
	public AudioClip OnBirthSound;

	private int _numberOfParticles;

	private ParticleSystem PS;

	private void Start()
	{
		PS = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if ((bool)OnBirthSound)
		{
			int count = PS.particleCount;
			if (count >= _numberOfParticles && count > _numberOfParticles)
			{
				SFXManager.instance.PlaySFX(OnBirthSound, 1f, null);
			}
			_numberOfParticles = count;
		}
	}
}
