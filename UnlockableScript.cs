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

	private void Start()
	{
		if (isTankeyTownItem)
		{
			base.transform.localScale = scale;
			base.transform.localRotation = Quaternion.Euler(Vector3.zero);
			EventTrigger component = GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate
			{
				TaskOnClick();
			});
			component.triggers.Add(entry);
			StartCoroutine(LateCheck());
			return;
		}
		UnlockableItem[] uIs = OptionsMainMenu.instance.UIs;
		foreach (UnlockableItem unlockableItem in uIs)
		{
			if (unlockableItem.ULID == ULID)
			{
				myUI = unlockableItem;
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
		EventTrigger component2 = GetComponent<EventTrigger>();
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerDown;
		entry2.callback.AddListener(delegate
		{
			TaskOnClick();
		});
		component2.triggers.Add(entry2);
		if ((bool)AccountMaster.instance)
		{
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
					UnlockableRequire.text = "( Achievement: '" + OptionsMainMenu.instance.AMnames[ULID] + "' required!)";
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
				if (myUI.code != "")
				{
					code = myUI.code;
				}
				UnlockableRequire.text = "( Achievement: '" + OptionsMainMenu.instance.AMnames[ULID] + "' required!)";
			}
			else
			{
				UnlockableTitle.text = myUI.UnlockableName;
				code = myUI.code;
			}
		}
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
			return;
		}
		if (Input.anyKeyDown)
		{
			_ = ULID;
		}
		if (!AccountMaster.instance.isSignedIn)
		{
			return;
		}
		if (AccountMaster.instance.PDO.AM[ULID] == 1)
		{
			if (myUI.codeNeededToUnlock && base.transform.parent == null)
			{
				base.transform.SetParent(origiParent);
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
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				if (OptionsMainMenu.instance.AMUS.Count > 0)
				{
					foreach (UnlockableScript aMU in OptionsMainMenu.instance.AMUS)
					{
						if (aMU.isBoost)
						{
							flag3 = true;
						}
						if (aMU.isMine)
						{
							flag4 = true;
						}
						if (aMU.isBullet)
						{
							flag5 = true;
						}
						if (aMU.isSkidmarks)
						{
							flag2 = true;
						}
						if (aMU.isSkin)
						{
							flag = true;
						}
						if (aMU.isHitmarker)
						{
							flag6 = true;
						}
					}
				}
				if (isSkin && flag)
				{
					foreach (UnlockableScript aMU2 in OptionsMainMenu.instance.AMUS)
					{
						if (aMU2.isSkin && aMU2 != this)
						{
							ReplaceAMUS(aMU2);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isMine && flag4)
				{
					foreach (UnlockableScript aMU3 in OptionsMainMenu.instance.AMUS)
					{
						if (aMU3.isMine && aMU3 != this)
						{
							ReplaceAMUS(aMU3);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isBullet && flag5)
				{
					foreach (UnlockableScript aMU4 in OptionsMainMenu.instance.AMUS)
					{
						if (aMU4.isBullet && aMU4 != this)
						{
							ReplaceAMUS(aMU4);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isSkidmarks && flag2)
				{
					foreach (UnlockableScript aMU5 in OptionsMainMenu.instance.AMUS)
					{
						if (aMU5.isSkidmarks && aMU5 != this)
						{
							ReplaceAMUS(aMU5);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isBoost && flag3)
				{
					foreach (UnlockableScript aMU6 in OptionsMainMenu.instance.AMUS)
					{
						if (aMU6.isBoost && aMU6 != this)
						{
							ReplaceAMUS(aMU6);
							return;
						}
					}
					Play2DClipAtPoint(ErrorSound);
				}
				else if (isHitmarker && flag6)
				{
					foreach (UnlockableScript aMU7 in OptionsMainMenu.instance.AMUS)
					{
						if (aMU7.isHitmarker && aMU7 != this)
						{
							ReplaceAMUS(aMU7);
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
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.ignoreListenerVolume = true;
		audioSource.volume = 1f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
