using System.Collections;
using TMPro;
using UnityEngine;

public class UpgradeField : MonoBehaviour
{
	private RectTransform myRect;

	public Vector3 oldpos;

	public Vector3 newpos;

	public float speed = 2f;

	public float startTime;

	private float journeyLength;

	public Transform followObject;

	public float posYoffset;

	[Header("Texts")]
	public TextMeshProUGUI UpgradeText;

	public TextMeshProUGUI CostText;

	private int theCost;

	public bool isPlayer2;

	private MoveTankScript myMTS;

	private WeeTurret myTurret;

	private int upgradeType;

	private int UpgradeLevel;

	public ObjectPlacing OP;

	public bool Show;

	public bool canShow = true;

	public bool inPlacingMode;

	private void Start()
	{
		myRect = GetComponent<RectTransform>();
		oldpos = new Vector3(0f, 0f, 0f);
		newpos = myRect.localScale;
		myRect.localScale = oldpos;
		startTime = Time.time;
		journeyLength = Vector3.Distance(newpos, oldpos);
	}

	public void ChangeMessage(bool isBuild, bool isSpeed, bool isShield, bool isRocket, bool isMine, bool isTurret, bool isTurretRepair, MoveTankScript MTS, WeeTurret WT)
	{
		if (inPlacingMode)
		{
			return;
		}
		string text = "<sprite=2>";
		myMTS = MTS;
		if ((bool)WT)
		{
			myTurret = WT;
		}
		else
		{
			myTurret = null;
		}
		isPlayer2 = MTS.isPlayer2;
		if (isBuild && MTS.Upgrades[0] <= 3)
		{
			UpgradeLevel = MTS.Upgrades[0];
			string text2 = ((UpgradeLevel + 1 == 1) ? "I" : ((UpgradeLevel + 1 == 2) ? "II" : ((UpgradeLevel + 1 == 3) ? "III" : "IV")));
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Build Upgrade " + text2;
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Build Upgrade " + text2;
			}
			CostText.text = "(cost x " + ZombieTankSpawner.instance.BuildPrices[UpgradeLevel] + ")";
			theCost = ZombieTankSpawner.instance.BuildPrices[UpgradeLevel];
			upgradeType = 0;
		}
		else if (isSpeed && MTS.Upgrades[1] <= 3)
		{
			UpgradeLevel = MTS.Upgrades[1];
			string text3 = ((UpgradeLevel + 1 == 1) ? "I" : ((UpgradeLevel + 1 == 2) ? "II" : ((UpgradeLevel + 1 == 3) ? "III" : "IV")));
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Speed Upgrade " + text3;
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Speed Upgrade " + text3;
			}
			CostText.text = "(cost x " + ZombieTankSpawner.instance.SpeedPrices[UpgradeLevel] + ")";
			theCost = ZombieTankSpawner.instance.SpeedPrices[UpgradeLevel];
			upgradeType = 1;
		}
		else if (isShield && MTS.Upgrades[2] <= 3)
		{
			UpgradeLevel = MTS.Upgrades[2];
			string text4 = ((UpgradeLevel + 1 == 1) ? "I" : ((UpgradeLevel + 1 == 2) ? "II" : ((UpgradeLevel + 1 == 3) ? "III" : "IV")));
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Armour Upgrade " + text4;
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Armour Upgrade " + text4;
			}
			CostText.text = "(cost x " + ZombieTankSpawner.instance.ShieldPrices[UpgradeLevel] + ")";
			theCost = ZombieTankSpawner.instance.ShieldPrices[UpgradeLevel];
			upgradeType = 2;
		}
		else if (isRocket && MTS.Upgrades[3] <= 0)
		{
			UpgradeLevel = MTS.Upgrades[3];
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Rocket Upgrade ";
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Rocket Upgrade ";
			}
			CostText.text = "(cost x " + ZombieTankSpawner.instance.RocketPrice + ")";
			theCost = ZombieTankSpawner.instance.RocketPrice;
			upgradeType = 3;
		}
		else if (isMine && MTS.Upgrades[4] <= 0)
		{
			UpgradeLevel = MTS.Upgrades[4];
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Tripmine Upgrade ";
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Tripmine Upgrade ";
			}
			CostText.text = "(cost x " + ZombieTankSpawner.instance.TripminePrice + ")";
			theCost = ZombieTankSpawner.instance.TripminePrice;
			upgradeType = 4;
		}
		else if (isTurret && MTS.Upgrades[5] <= 0)
		{
			UpgradeLevel = MTS.Upgrades[5];
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Turret";
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Turret";
			}
			CostText.text = "(cost x " + ZombieTankSpawner.instance.TurretPrice + ")";
			theCost = ZombieTankSpawner.instance.TurretPrice;
			upgradeType = 5;
		}
		else
		{
			if (!isTurretRepair || !myTurret)
			{
				return;
			}
			UpgradeLevel = MTS.Upgrades[5];
			if (myTurret.Health >= myTurret.maxHealth)
			{
				if (myTurret.upgradeLevel < 2)
				{
					if (GameMaster.instance.isPlayingWithController || isPlayer2)
					{
						UpgradeText.text = "Press " + text + " to upgrade Turret";
					}
					else
					{
						UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to upgrade Turret";
					}
					CostText.text = "(cost x " + ZombieTankSpawner.instance.TurretUpgradePrices[myTurret.upgradeLevel] + ")";
					theCost = ZombieTankSpawner.instance.TurretUpgradePrices[myTurret.upgradeLevel];
				}
			}
			else
			{
				int num = ((myTurret.upgradeLevel == 0) ? ZombieTankSpawner.instance.TurretRepairPrice : ((myTurret.upgradeLevel == 1) ? (ZombieTankSpawner.instance.TurretRepairPrice * 2) : (ZombieTankSpawner.instance.TurretRepairPrice * 3)));
				if (GameMaster.instance.isPlayingWithController || isPlayer2)
				{
					UpgradeText.text = "Press " + text + " to buy: Repair Turret";
				}
				else
				{
					UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Repair Turret";
				}
				CostText.text = "(cost x " + num + ")";
				theCost = num;
			}
			upgradeType = 6;
		}
	}

	private void DoUpgrade(int playerID)
	{
		if (upgradeType == 6)
		{
			Debug.Log("REPAIRING OR UPGRADING!");
			if (myTurret.Health >= myTurret.maxHealth)
			{
				if (myTurret.upgradeLevel < 2)
				{
					GameMaster.instance.Playerkills[playerID] -= theCost;
					if ((bool)myTurret)
					{
						myTurret.Upgrade();
					}
					Play2DClipAtPoint(ZombieTankSpawner.instance.Katsjing);
					Show = false;
					startTime = Time.time;
					StartCoroutine(WaitBox());
				}
				else
				{
					Show = false;
					startTime = Time.time;
					StartCoroutine(WaitBox());
				}
			}
			else
			{
				GameMaster.instance.Playerkills[playerID] -= theCost;
				if ((bool)myTurret)
				{
					myTurret.Repair();
				}
				Play2DClipAtPoint(ZombieTankSpawner.instance.Katsjing);
				Show = false;
				startTime = Time.time;
				StartCoroutine(WaitBox());
			}
			return;
		}
		GameMaster.instance.Playerkills[playerID] -= theCost;
		myMTS.Upgrades[upgradeType]++;
		if ((bool)AchievementsTracker.instance)
		{
			AchievementsTracker.instance.BoughtUpgrade = true;
			if (myMTS.Upgrades[0] > 3 && myMTS.Upgrades[1] > 3 && myMTS.Upgrades[2] > 3 && myMTS.Upgrades[3] > 0 && myMTS.Upgrades[4] > 0 && myMTS.Upgrades[5] > 0)
			{
				AchievementsTracker.instance.completeOtherAchievement(13);
			}
		}
		if (upgradeType == 2)
		{
			myMTS.HTtanks.health_armour++;
			myMTS.HTtanks.maxArmour++;
			if (myMTS.Upgrades[upgradeType] == 4)
			{
				myMTS.HTtanks.health_armour = 7;
				myMTS.HTtanks.maxArmour = 7;
			}
		}
		if (upgradeType == 5)
		{
			Show = true;
			startTime = Time.time;
			inPlacingMode = true;
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to place Turret";
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to place Turret";
			}
			CostText.text = "";
			OP.StartPlacing();
			GameMaster.instance.PKU.NewUpgrade(1, upgradeType, UpgradeLevel);
			Play2DClipAtPoint(ZombieTankSpawner.instance.Katsjing);
		}
		else
		{
			Show = false;
			startTime = Time.time;
			StartCoroutine(WaitBox());
			GameMaster.instance.PKU.NewUpgrade(playerID, upgradeType, UpgradeLevel);
			Play2DClipAtPoint(ZombieTankSpawner.instance.Katsjing);
		}
	}

	private void Update()
	{
		myRect.position = followObject.position + new Vector3(0f, posYoffset, 0f);
		if (Show && canShow)
		{
			if (inPlacingMode)
			{
				if (myMTS.player.GetButtonUp("Use"))
				{
					Show = false;
					startTime = Time.time;
					inPlacingMode = false;
					StartCoroutine(WaitBox());
				}
				return;
			}
			if (myMTS.player.GetButtonUp("Use"))
			{
				if (theCost <= GameMaster.instance.Playerkills[myMTS.playerId])
				{
					DoUpgrade(myMTS.playerId);
				}
				else
				{
					Play2DClipAtPoint(ZombieTankSpawner.instance.denied);
					GameMaster.instance.PKU.StartCoroutine("StartPlayerDeniedAnimation", myMTS.playerId + 1);
				}
			}
			if (Vector3.Distance(myRect.localScale, newpos) > 0.01f)
			{
				float t = (Time.time - startTime) * speed / journeyLength;
				myRect.localScale = Vector3.Lerp(myRect.localScale, newpos, t);
			}
		}
		else if (Vector3.Distance(myRect.localScale, oldpos) > 0.01f)
		{
			float t2 = (Time.time - startTime) * speed / journeyLength;
			myRect.localScale = Vector3.Lerp(myRect.localScale, oldpos, t2);
		}
		else
		{
			myRect.localScale = Vector3.zero;
		}
	}

	public IEnumerator WaitBox()
	{
		canShow = false;
		yield return new WaitForSeconds(0.6f);
		canShow = true;
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
