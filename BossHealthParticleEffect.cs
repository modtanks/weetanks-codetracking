using UnityEngine;

public class BossHealthParticleEffect : MonoBehaviour
{
	public ParticleSystem[] firstBreak;

	public ParticleSystem[] secondBreak;

	public EnemyAI HealthTankScript;

	private bool firstActive;

	private bool secondActive;

	public int firstStateLives = 30;

	public int secondStateLives = 10;

	[Header("Zombie Attributes")]
	public HealthTanks HT;

	private void Update()
	{
		if ((bool)HT)
		{
			if (HT.health > firstStateLives && firstActive)
			{
				StopParticles();
			}
			else if (HT.health < firstStateLives && !firstActive)
			{
				ActivateFirst();
			}
			else if (HT.health < secondStateLives && !secondActive)
			{
				ActivateSecond();
			}
		}
		else if ((bool)HealthTankScript)
		{
			if (HealthTankScript.isTransporting)
			{
				StopParticles();
			}
			else if (HealthTankScript.HTscript.health > firstStateLives && firstActive)
			{
				StopParticles();
			}
			else if (HealthTankScript.HTscript.health < firstStateLives && !firstActive)
			{
				ActivateFirst();
			}
			else if (HealthTankScript.HTscript.health < secondStateLives && !secondActive)
			{
				ActivateSecond();
			}
		}
	}

	private void ActivateSecond()
	{
		ParticleSystem[] array = secondBreak;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		secondActive = true;
	}

	private void ActivateFirst()
	{
		ParticleSystem[] array = firstBreak;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		firstActive = true;
	}

	private void StopParticles()
	{
		ParticleSystem[] array = firstBreak;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		array = secondBreak;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		firstActive = false;
		secondActive = false;
	}
}
