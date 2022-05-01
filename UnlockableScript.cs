using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnlockableScript : MonoBehaviour
{
	public RawImage checkmark;

	private Button mybtn;

	public int ULID;

	public TextMeshProUGUI UnlockableTitle;

	public TextMeshProUGUI UnlockableRequire;

	public Color ActivatedColor;

	public Color NotActivatedColor;

	public AudioClip SuccesSound;

	public AudioClip ErrorSound;

	public AudioClip UnlockedSound;

	public AudioClip ClickSound;

	public AudioClip CheckSound;

	public Vector3 scale;

	public Transform origiParent;

	public UnlockableItem myUI;

	public bool isSkin;

	public bool isMine;

	public bool isSkidmarks;

	public bool isBullet;

	public bool isBoost;

	public bool isHitmarker;

	public string code;

	public string tempEnter;

	public bool isTankeyTownItem;

	private int prevLang = -1;

	public void SetText()
	{
		prevLang = LocalizationMaster.instance.CurrentLang;
		if (!AccountMaster.instance)
		{
			return;
		}
		if (AccountMaster.instance.PDO.AM.Length > ULID)
		{
			if (AccountMaster.instance.PDO.AM[ULID] == 1)
			{
				UnlockableTitle.text = myUI.UnlockableName;
				UnlockableRequire.text = "";
				GetComponent<MainMenuButtons>().MakeAvailable();
				if (myUI.code != "")
				{
					code = myUI.code;
				}
			}
			else if (!myUI.codeNeededToUnlock)
			{
				UnlockableTitle.text = myUI.UnlockableName;
				GetComponent<MainMenuButtons>().MakeUnavailable();
				if (myUI.code != "")
				{
					code = myUI.code;
				}
				UnlockableRequire.text = "( Achievement: '" + LocalizationMaster.instance.GetText("AM_" + ULID) + "' required!)";
			}
			else
			{
				UnlockableTitle.text = myUI.UnlockableName;
				code = myUI.code;
				GetComponent<MainMenuButtons>().MakeUnavailable();
			}
		}
		else if (!myUI.codeNeededToUnlock)
		{
			UnlockableTitle.text = myUI.UnlockableName;
			GetComponent<MainMenuButtons>().MakeUnavailable();
			if (myUI.code != "")
			{
				code = myUI.code;
			}
			UnlockableRequire.text = "( Achievement: '" + LocalizationMaster.instance.GetText("AM_" + ULID) + "' required!)";
		}
		else
		{
			UnlockableTitle.text = myUI.UnlockableName;
			GetComponent<MainMenuButtons>().MakeUnavailable();
			code = myUI.code;
		}
	}

	private void Start()
	{
		if (isTankeyTownItem)
		{
			base.transform.localScale = scale;
			base.transform.localRotation = Quaternion.Euler(Vector3.zero);
			EventTrigger triggerz = GetComponent<EventTrigger>();
			EventTrigger.Entry pointerDownz = new EventTrigger.Entry();
			pointerDownz.eventID = EventTriggerType.PointerDown;
			pointerDownz.callback.AddListener(delegate
			{
				TaskOnClick();
			});
			triggerz.triggers.Add(pointerDownz);
			StartCoroutine(LateCheck());
			return;
		}
		UnlockableItem[] uIs = OptionsMainMenu.instance.UIs;
		foreach (UnlockableItem UI in uIs)
		{
			if (UI.ULID == ULID)
			{
				myUI = UI;
			}
		}
		if (myUI == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (!myUI.UnlockableEnabled)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		isSkin = myUI.isSkin;
		isMine = myUI.isMine;
		isSkidmarks = myUI.isSkidmarks;
		isBullet = myUI.isBullet;
		isBoost = myUI.isBoost;
		isHitmarker = myUI.isHitmarker;
		if ((bool)checkmark)
		{
			checkmark.enabled = false;
		}
		origiParent = base.transform.parent;
		if (AccountMaster.instance.PDO.ActivatedAM.Contains(ULID) && !OptionsMainMenu.instance.AMUS.Contains(this))
		{
			OptionsMainMenu.instance.AMUS.Add(this);
		}
		base.transform.localScale = scale;
		base.transform.localRotation = Quaternion.Euler(Vector3.zero);
		EventTrigger trigger = GetComponent<EventTrigger>();
		EventTrigger.Entry pointerDown = new EventTrigger.Entry();
		pointerDown.eventID = EventTriggerType.PointerDown;
		pointerDown.callback.AddListener(delegate
		{
			TaskOnClick();
		});
		trigger.triggers.Add(pointerDown);
		SetText();
		StartCoroutine(LateCheck());
	}

	private IEnumerator LateCheck()
	{
		yield return new WaitForSeconds(2f);
		if (AccountMaster.instance.PDO.ActivatedAM.Contains(ULID) && !OptionsMainMenu.instance.AMUS.Contains(this))
		{
			OptionsMainMenu.instance.AMUS.Add(this);
		}
	}

	private void Update()
	{
		if (prevLang != LocalizationMaster.instance.CurrentLang)
		{
			SetText();
		}
		if (isTankeyTownItem)
		{
			if (AccountMaster.instance.PDO.ActivatedAM.Contains(ULID))
			{
				checkmark.enabled = true;
				UnlockableRequire.color = ActivatedColor;
				UnlockableRequire.text = "Activated.";
			}
			else if (!AccountMaster.instance.PDO.ActivatedAM.Contains(ULID))
			{
				checkmark.enabled = false;
				UnlockableRequire.text = "";
			}
		}
		else
		{
			if (!AccountMaster.instance.isSignedIn)
			{
				return;
			}
			if (AccountMaster.instance.PDO.AM[ULID] == 1)
			{
				if (myUI.codeNeededToUnlock && base.transform.parent == null)
				{
					base.transform.SetParent(origiParent);
					checkmark.texture = GetComponent<MainMenuButtons>().ButtonMetaPositive;
				}
				if (AccountMaster.instance.PDO.ActivatedAM.Contains(ULID))
				{
					checkmark.enabled = true;
					UnlockableTitle.color = ActivatedColor;
					UnlockableRequire.color = ActivatedColor;
					UnlockableRequire.text = "Activated.";
				}
				else if (!AccountMaster.instance.PDO.ActivatedAM.Contains(ULID))
				{
					checkmark.enabled = false;
					UnlockableRequire.text = "";
					UnlockableTitle.color = NotActivatedColor;
				}
			}
			else if (myUI.codeNeededToUnlock && myUI.codeNeededToUnlock)
			{
				base.transform.SetParent(null);
			}
		}
	}

	public void TaskOnClick()
	{
		if (!AccountMaster.instance.isSignedIn)
		{
			return;
		}
		Debug.Log("You have clicked the button!");
		if (ULID >= 1000 || AccountMaster.instance.PDO.AM[ULID] == 1)
		{
			if (!AccountMaster.instance.PDO.ActivatedAM.Contains(ULID) && ULID < 1000)
			{
				bool skinAlready = false;
				bool tracksAlready = false;
				bool boostAlready = false;
				bool mineAlready = false;
				bool bulletAlready = false;
				bool hitmarkerAlready = false;
				if (OptionsMainMenu.instance.AMUS.Count > 0)
				{
					foreach (UnlockableScript sel in OptionsMainMenu.instance.AMUS)
					{
						if (sel.isBoost)
						{
							boostAlready = true;
						}
						if (sel.isMine)
						{
							mineAlready = true;
						}
						if (sel.isBullet)
						{
							bulletAlready = true;
						}
						if (sel.isSkidmarks)
						{
							tracksAlready = true;
						}
						if (sel.isSkin)
						{
							skinAlready = true;
						}
						if (sel.isHitmarker)
						{
							hitmarkerAlready = true;
						}
					}
				}
				if (isSkin && skinAlready)
				{
					foreach (UnlockableScript US6 in OptionsMainMenu.instance.AMUS)
					{
						if (US6.isSkin && US6 != this)
						{
							ReplaceAMUS(US6);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isMine && mineAlready)
				{
					foreach (UnlockableScript US5 in OptionsMainMenu.instance.AMUS)
					{
						if (US5.isMine && US5 != this)
						{
							ReplaceAMUS(US5);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isBullet && bulletAlready)
				{
					foreach (UnlockableScript US4 in OptionsMainMenu.instance.AMUS)
					{
						if (US4.isBullet && US4 != this)
						{
							ReplaceAMUS(US4);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isSkidmarks && tracksAlready)
				{
					foreach (UnlockableScript US3 in OptionsMainMenu.instance.AMUS)
					{
						if (US3.isSkidmarks && US3 != this)
						{
							ReplaceAMUS(US3);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isBoost && boostAlready)
				{
					foreach (UnlockableScript US2 in OptionsMainMenu.instance.AMUS)
					{
						if (US2.isBoost && US2 != this)
						{
							ReplaceAMUS(US2);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isHitmarker && hitmarkerAlready)
				{
					foreach (UnlockableScript US in OptionsMainMenu.instance.AMUS)
					{
						if (US.isHitmarker && US != this)
						{
							ReplaceAMUS(US);
							OptionsMainMenu.instance.CheckCustomHitmarkers();
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else
				{
					Play2DClipAtPoint(SuccesSound);
					AccountMaster.instance.PDO.ActivatedAM.Add(ULID);
					OptionsMainMenu.instance.AMUS.Add(this);
					AccountMaster.instance.SaveCloudData(4, ULID, 0, bounceKill: false);
					if (isHitmarker)
					{
						OptionsMainMenu.instance.CheckCustomHitmarkers();
					}
				}
			}
			else
			{
				Play2DClipAtPoint(SuccesSound);
				if (OptionsMainMenu.instance.AMselected.Contains(ULID))
				{
					OptionsMainMenu.instance.AMselected.Remove(ULID);
				}
				if (AccountMaster.instance.PDO.ActivatedAM.Contains(ULID))
				{
					AccountMaster.instance.PDO.ActivatedAM.Remove(ULID);
				}
				OptionsMainMenu.instance.AMUS.Remove(this);
				AccountMaster.instance.SaveCloudData(4, ULID, 0, bounceKill: false);
				if (isHitmarker)
				{
					OptionsMainMenu.instance.CheckCustomHitmarkers();
				}
			}
		}
		else
		{
			Play2DClipAtPoint(ErrorSound);
		}
	}

	private void ReplaceAMUS(UnlockableScript otherUS)
	{
		AccountMaster.instance.PDO.ActivatedAM.Remove(otherUS.ULID);
		OptionsMainMenu.instance.AMUS.Remove(otherUS);
		AccountMaster.instance.PDO.ActivatedAM.Add(ULID);
		OptionsMainMenu.instance.AMUS.Add(this);
		AccountMaster.instance.SaveCloudData(4, ULID, otherUS.ULID, bounceKill: false);
		Play2DClipAtPoint(SuccesSound);
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.ignoreListenerVolume = true;
		audioSource.volume = 1f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
