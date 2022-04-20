using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
	public RectTransform[] rt;

	public float originalWidth;

	public float originalHeight;

	public GameObject[] BossOverlays;

	public bool generateQuads = true;

	public GameObject barStripe;

	public RawImage TankImage;

	private KingTankScript KTS;

	private int latestKnownMission = -1;

	private void Start()
	{
		originalWidth = rt[0].rect.width;
	}

	public IEnumerator MakeBarWhite()
	{
		int mission = -1;
		switch (GameMaster.instance.CurrentMission)
		{
		case 9:
			mission = 0;
			break;
		case 29:
			mission = 1;
			break;
		case 49:
			mission = 2;
			break;
		case 69:
			mission = 3;
			break;
		case 99:
			mission = 4;
			break;
		}
		Color beforeClr = Color.black;
		RawImage BarImage = null;
		bool SetColor = false;
		if (mission > -1)
		{
			BarImage = rt[mission].GetComponent<RawImage>();
			if (BarImage.color != Color.white)
			{
				beforeClr = BarImage.color;
				BarImage.color = Color.white;
				SetColor = true;
			}
		}
		yield return new WaitForSeconds(0.04f);
		if (mission > -1 && (bool)BarImage && SetColor)
		{
			BarImage.color = beforeClr;
		}
	}

	private void OnGUI()
	{
		if ((GameMaster.instance.PlayerAlive && GameMaster.instance.AmountGoodTanks > 0 && GameMaster.instance.AmountEnemyTanks > 0 && GameMaster.instance.GameHasStarted && GameMaster.instance.CurrentMission > 8) || GameMaster.instance.CurrentMission == 99)
		{
			GameObject[] bossOverlays = BossOverlays;
			foreach (GameObject Overlay2 in bossOverlays)
			{
				Overlay2.SetActive(value: false);
			}
			GameObject theBoss = GameObject.FindGameObjectWithTag("Boss");
			if (theBoss != null)
			{
				if (GameMaster.instance.CurrentMission == 99 && !KTS)
				{
					KTS = theBoss.transform.parent.GetComponent<KingTankScript>();
				}
				else if (GameMaster.instance.CurrentMission == 99 && (bool)KTS && !KTS.IsInFinalBattle)
				{
					return;
				}
				HealthTanks healthBoss = theBoss.GetComponent<HealthTanks>();
				int mission = -1;
				mission = GameMaster.instance.CurrentMission switch
				{
					9 => 0, 
					29 => 1, 
					49 => 2, 
					69 => 3, 
					99 => 4, 
					_ => -1, 
				};
				if (mission > -1 && healthBoss.health > 0)
				{
					latestKnownMission = mission;
					BossOverlays[mission].SetActive(value: true);
					rt[mission].sizeDelta = Vector2.Lerp(rt[mission].sizeDelta, new Vector2(originalWidth / (float)healthBoss.maxHealth * (float)healthBoss.health, rt[mission].sizeDelta.y), Time.deltaTime * 2f);
				}
				if (GameMaster.instance.CurrentMission == 99 && healthBoss.health < 1)
				{
					BossOverlays[mission].SetActive(value: false);
				}
			}
			else if (latestKnownMission > -1)
			{
				rt[latestKnownMission].sizeDelta = new Vector2(0f, rt[latestKnownMission].sizeDelta.y);
				StartCoroutine(disableBar(latestKnownMission));
				latestKnownMission = -1;
			}
		}
		else if (latestKnownMission > -1)
		{
			rt[latestKnownMission].sizeDelta = new Vector2(0f, rt[latestKnownMission].sizeDelta.y);
			StartCoroutine(disableBar(latestKnownMission));
			latestKnownMission = -1;
		}
		if (GameMaster.instance.Bosses.Length < 1)
		{
			GameObject[] bossOverlays2 = BossOverlays;
			foreach (GameObject Overlay in bossOverlays2)
			{
				Overlay.SetActive(value: false);
			}
		}
	}

	private IEnumerator disableBar(int bar)
	{
		yield return new WaitForSeconds(3f);
		BossOverlays[bar].SetActive(value: false);
	}
}
