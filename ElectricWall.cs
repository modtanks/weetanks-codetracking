using UnityEngine;

public class ElectricWall : MonoBehaviour
{
	public ParticleSystem PS;

	private bool isCharged = false;

	public MeshRenderer MR;

	private void Start()
	{
		if ((bool)GameMaster.instance)
		{
			if (GameMaster.instance.CurrentMission == 69)
			{
				InvokeRepeating("UpdateCheck", 0.5f, 0.5f);
			}
			else
			{
				ActivateWall();
			}
		}
	}

	private void UpdateCheck()
	{
		if (!GameMaster.instance.GameHasStarted && isCharged)
		{
			PS.Stop();
			isCharged = false;
			MR.material.SetColor("_EmissionColor", Color.black);
		}
	}

	public void ActivateWall()
	{
		if (!isCharged)
		{
			if (OptionsMainMenu.instance.currentGraphicSettings > 4)
			{
				PS.Play();
			}
			isCharged = true;
			MR.material.SetColor("_EmissionColor", Color.white);
		}
	}

	public void ActivateAroundMe()
	{
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 5f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (col.tag == "ElectricWall")
			{
				ElectricWall EW = col.GetComponent<ElectricWall>();
				if ((bool)EW)
				{
					EW.ActivateWall();
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && isCharged)
		{
			MoveTankScript MTS = other.GetComponent<MoveTankScript>();
			if ((bool)MTS)
			{
				MTS.StunMe(2f);
				return;
			}
			EnemyAI AIscript = other.GetComponent<EnemyAI>();
			if ((bool)AIscript)
			{
				AIscript.StunMe(2f);
			}
		}
		else if (other.tag == "Enemy" && isCharged && GameMaster.instance.CurrentMission != 69)
		{
			EnemyAI EA = other.GetComponent<EnemyAI>();
			if ((bool)EA && !EA.isElectric)
			{
				EA.StunMe(2f);
			}
		}
		else if (other.tag == "Bullet")
		{
			PlayerBulletScript PBS = other.GetComponent<PlayerBulletScript>();
			if ((bool)PBS)
			{
				if (PBS.isElectric && !isCharged)
				{
					ActivateWall();
					ActivateAroundMe();
				}
				if (isCharged)
				{
					PBS.ChargeElectric();
				}
			}
		}
		else if (other.tag == "Boss" && !isCharged)
		{
			ActivateWall();
			ActivateAroundMe();
		}
	}
}
