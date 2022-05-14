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

	public bool CheckForAmour;

	[Header("Zombie Attributes")]
	public HealthTanks HT;

	private void Update()
	{
		if ((bool)HT)
		{
			if (CheckForAmour)
			{
				if (HT.health_armour > firstStateLives && firstActive)
				{
					StopParticles();
				}
				if (HT.health_armour > secondStateLives && secondActive)
				{
					StopParticlesSecond();
				}
				if (HT.health_armour < firstStateLives && !firstActive)
				{
					ActivateFirst();
				}
				if (HT.health_armour < secondStateLives && !secondActive)
				{
					ActivateSecond();
				}
			}
			else
			{
				if (HT.health > firstStateLives && firstActive)
				{
					StopParticles();
				}
				if (HT.health > secondStateLives && secondActive)
				{
					StopParticlesSecond();
				}
				if (HT.health < firstStateLives && !firstActive)
				{
					ActivateFirst();
				}
				if (HT.health < secondStateLives && !secondActive)
				{
					ActivateSecond();
				}
			}
		}
		else
		{
			if (!HealthTankScript)
			{
				return;
			}
			if (CheckForAmour)
			{
				if (HealthTankScript.isTransporting)
				{
					StopParticles();
				}
				else if (HealthTankScript.HTscript.health_armour > firstStateLives && firstActive)
				{
					StopParticles();
				}
				else if (HealthTankScript.HTscript.health_armour < firstStateLives && !firstActive)
				{
					ActivateFirst();
				}
				else if (HealthTankScript.HTscript.health_armour < secondStateLives && !secondActive)
				{
					ActivateSecond();
				}
			}
			else if (HealthTankScript.isTransporting)
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
		firstActive = false;
	}

	private void StopParticlesSecond()
	{
		ParticleSystem[] array = secondBreak;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		secondActive = false;
	}
}
