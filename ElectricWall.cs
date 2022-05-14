using UnityEngine;

public class ElectricWall : MonoBehaviour
{
	public ParticleSystem PS;

	private bool isCharged;

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
			MR.material.EnableKeyword("_EMISSION");
			MR.material.SetColor("_EmissionColor", Color.white);
		}
	}

	public void ActivateAroundMe()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 5f);
		foreach (Collider collider in array)
		{
			if (collider.tag == "ElectricWall")
			{
				ElectricWall component = collider.GetComponent<ElectricWall>();
				if ((bool)component)
				{
					component.ActivateWall();
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && isCharged)
		{
			MoveTankScript component = other.GetComponent<MoveTankScript>();
			if ((bool)component)
			{
				component.StunMe(2f);
				return;
			}
			EnemyAI component2 = other.GetComponent<EnemyAI>();
			if ((bool)component2)
			{
				component2.StunMe(2f);
			}
		}
		else if (other.tag == "Enemy" && isCharged && GameMaster.instance.CurrentMission != 69)
		{
			EnemyAI component3 = other.GetComponent<EnemyAI>();
			if ((bool)component3 && !component3.isElectric)
			{
				component3.StunMe(2f);
			}
		}
		else if (other.tag == "Bullet")
		{
			PlayerBulletScript component4 = other.GetComponent<PlayerBulletScript>();
			if ((bool)component4)
			{
				if (component4.isElectric && !isCharged)
				{
					ActivateWall();
					ActivateAroundMe();
				}
				if (isCharged)
				{
					component4.ChargeElectric();
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
