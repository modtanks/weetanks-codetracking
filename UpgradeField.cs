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

	[Header("Audio")]
	public AudioClip Katsjing;

	public AudioClip denied;

	[Header("Prices")]
	public int[] BuildPrices;

	public int[] SpeedPrices;

	public int[] ShieldPrices;

	public int RocketPrice;

	public int TripminePrice;

	public int TurretPrice;

	public int TurretRepairPrice;

	public int[] TurretUpgradePrices;

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
		if (isBuild && MTS.Upgrades[0] <= 2)
		{
			UpgradeLevel = MTS.Upgrades[0];
			string text2 = ((UpgradeLevel + 1 == 1) ? "I" : ((UpgradeLevel + 1 == 2) ? "II" : "III"));
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Build Upgrade " + text2;
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Build Upgrade " + text2;
			}
			CostText.text = "(cost x " + BuildPrices[UpgradeLevel] + ")";
			theCost = BuildPrices[UpgradeLevel];
			upgradeType = 0;
		}
		else if (isSpeed && MTS.Upgrades[1] <= 2)
		{
			UpgradeLevel = MTS.Upgrades[1];
			string text3 = ((UpgradeLevel + 1 == 1) ? "I" : ((UpgradeLevel + 1 == 2) ? "II" : "III"));
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Speed Upgrade " + text3;
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Speed Upgrade " + text3;
			}
			CostText.text = "(cost x " + SpeedPrices[UpgradeLevel] + ")";
			theCost = SpeedPrices[UpgradeLevel];
			upgradeType = 1;
		}
		else if (isShield && MTS.Upgrades[2] <= 2)
		{
			UpgradeLevel = MTS.Upgrades[2];
			string text4 = ((UpgradeLevel + 1 == 1) ? "I" : ((UpgradeLevel + 1 == 2) ? "II" : "III"));
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press " + text + " to buy: Armour Upgrade " + text4;
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy: Armour Upgrade " + text4;
			}
			CostText.text = "(cost x " + ShieldPrices[UpgradeLevel] + ")";
			theCost = ShieldPrices[UpgradeLevel];
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
			CostText.text = "(cost x " + RocketPrice + ")";
			theCost = RocketPrice;
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
			CostText.text = "(cost x " + TripminePrice + ")";
			theCost = TripminePrice;
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
			CostText.text = "(cost x " + TurretPrice + ")";
			theCost = TurretPrice;
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
				if (myTurret.upgradeLevel < 1)
				{
					if (GameMaster.instance.isPlayingWithController || isPlayer2)
					{
						UpgradeText.text = "Press " + text + " to upgrade Turret";
					}
					else
					{
						UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to upgrade Turret";
					}
					CostText.text = "(cost x " + TurretUpgradePrices[myTurret.upgradeLevel] + ")";
					theCost = TurretUpgradePrices[myTurret.upgradeLevel];
				}
			}
			else
			{
				int num = ((myTurret.upgradeLevel < 1) ? TurretRepairPrice : (TurretRepairPrice * 2));
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
				if (myTurret.upgradeLevel < 1)
				{
					GameMaster.instance.Playerkills[playerID] -= theCost;
					if ((bool)myTurret)
					{
						myTurret.Upgrade();
					}
					Play2DClipAtPoint(Katsjing);
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
				Play2DClipAtPoint(Katsjing);
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
			if (myMTS.Upgrades[0] > 2 && myMTS.Upgrades[1] > 2 && myMTS.Upgrades[2] > 2 && myMTS.Upgrades[3] > 0 && myMTS.Upgrades[4] > 0 && myMTS.Upgrades[5] > 0)
			{
				AchievementsTracker.instance.completeOtherAchievement(13);
			}
		}
		if (upgradeType == 2)
		{
			myMTS.HTtanks.health++;
			myMTS.HTtanks.maxHealth++;
		}
		if (upgradeType == 5)
		{
			Show = true;
			startTime = Time.time;
			inPlacingMode = true;
			if (GameMaster.instance.isPlayingWithController || isPlayer2)
			{
				UpgradeText.text = "Press <sprite=5> to place Turret";
			}
			else
			{
				UpgradeText.text = "Press " + myMTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to place Turret";
			}
			CostText.text = "";
			OP.StartPlacing();
			GameMaster.instance.PKU.NewUpgrade(1, upgradeType, UpgradeLevel);
			Play2DClipAtPoint(Katsjing);
		}
		else
		{
			Show = false;
			startTime = Time.time;
			StartCoroutine(WaitBox());
			GameMaster.instance.PKU.NewUpgrade(playerID, upgradeType, UpgradeLevel);
			Play2DClipAtPoint(Katsjing);
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
					Play2DClipAtPoint(denied);
					GameMaster.instance.PKU.StartCoroutine("StartPlayerDeniedAnimation", 1);
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
