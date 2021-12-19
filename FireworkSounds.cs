using UnityEngine;

public class FireworkSounds : MonoBehaviour
{
	public AudioClip OnBirthSound;

	private int _numberOfParticles;

	private ParticleSystem particleSystem;

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if ((bool)OnBirthSound)
		{
			int particleCount = particleSystem.particleCount;
			if (particleCount >= _numberOfParticles && particleCount > _numberOfParticles)
			{
				GameMaster.instance.Play2DClipAtPoint(OnBirthSound, 1f);
			}
			_numberOfParticles = particleCount;
		}
	}
}
