using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossExitTrigger : MonoBehaviour
{
	public TextMeshProUGUI ExitText;

	public bool PlayerInMe;

	public List<MoveTankScript> MTSinMe = new List<MoveTankScript>();

	public bool ExitTriggered = false;

	public Animator ExitPlaneAnimator;

	public AudioClip PlaneTakeOffSound;

	public ContinuousRotating[] CRS;

	public bool ExitOverride = false;

	private void Start()
	{
		ExitText.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (PlayerInMe && MTSinMe.Count > 0 && !ExitTriggered)
		{
			foreach (MoveTankScript MTS in MTSinMe)
			{
				if (MTS.player.GetButtonUp("Use"))
				{
					DoExit();
				}
			}
			return;
		}
		if (ExitOverride && !ExitTriggered)
		{
			DoExit();
		}
	}

	private IEnumerator DoFlyAwayAnimation()
	{
		yield return new WaitForSeconds(1f);
		ExitPlaneAnimator.SetBool("ExitLevel", value: true);
		yield return new WaitForSeconds(7f);
		SceneManager.LoadScene(5);
	}

	private void DoExit()
	{
		if ((bool)AchievementsTracker.instance)
		{
			AchievementsTracker.instance.completeAchievementWithAI(0);
		}
		ExitText.gameObject.SetActive(value: false);
		ExitTriggered = true;
		CameraFollowPlayer CFP = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
		if ((bool)CFP)
		{
			CFP.enabled = false;
		}
		foreach (GameObject P in GameMaster.instance.Players)
		{
			P.transform.parent.gameObject.SetActive(value: false);
		}
		ContinuousRotating[] cRS = CRS;
		foreach (ContinuousRotating CR in cRS)
		{
			CR.enabled = false;
		}
		StartCoroutine(DoFlyAwayAnimation());
		SFXManager.instance.PlaySFX(PlaneTakeOffSound, 1f, null);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag == "Player") || ExitTriggered)
		{
			return;
		}
		PlayerInMe = true;
		ExitText.gameObject.SetActive(value: true);
		string button = "<sprite=2>";
		MoveTankScript MTS = other.GetComponent<MoveTankScript>();
		if ((bool)MTS)
		{
			if (!MTSinMe.Contains(MTS))
			{
				MTSinMe.Add(MTS);
			}
			if (GameMaster.instance.isPlayingWithController || MTS.isPlayer2)
			{
				ExitText.text = "Press " + button + " board plane";
			}
			else
			{
				ExitText.text = "Press " + MTS.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to board plane";
			}
		}
		else
		{
			ExitText.text = "Press USE KEY board plane";
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			PlayerInMe = false;
			ExitText.gameObject.SetActive(value: false);
			MoveTankScript MTS = other.GetComponent<MoveTankScript>();
			if ((bool)MTS && MTSinMe.Contains(MTS))
			{
				MTSinMe.Remove(MTS);
			}
		}
	}
}
