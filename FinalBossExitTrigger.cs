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

	public bool ExitTriggered;

	public Animator ExitPlaneAnimator;

	public AudioClip PlaneTakeOffSound;

	public ContinuousRotating[] CRS;

	public bool ExitOverride;

	private void Start()
	{
		ExitText.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (PlayerInMe && MTSinMe.Count > 0 && !ExitTriggered)
		{
			foreach (MoveTankScript item in MTSinMe)
			{
				if (item.player.GetButtonUp("Use"))
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
		CameraFollowPlayer component = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		foreach (GameObject player in GameMaster.instance.Players)
		{
			player.transform.parent.gameObject.SetActive(value: false);
		}
		ContinuousRotating[] cRS = CRS;
		for (int i = 0; i < cRS.Length; i++)
		{
			cRS[i].enabled = false;
		}
		StartCoroutine(DoFlyAwayAnimation());
		GameMaster.instance.Play2DClipAtPoint(PlaneTakeOffSound, 1f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag == "Player") || ExitTriggered)
		{
			return;
		}
		PlayerInMe = true;
		ExitText.gameObject.SetActive(value: true);
		string text = "<sprite=2>";
		MoveTankScript component = other.GetComponent<MoveTankScript>();
		if ((bool)component)
		{
			if (!MTSinMe.Contains(component))
			{
				MTSinMe.Add(component);
			}
			if (GameMaster.instance.isPlayingWithController || component.isPlayer2)
			{
				ExitText.text = "Press " + text + " board plane";
			}
			else
			{
				ExitText.text = "Press " + component.player.controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to board plane";
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
			MoveTankScript component = other.GetComponent<MoveTankScript>();
			if ((bool)component && MTSinMe.Contains(component))
			{
				MTSinMe.Remove(component);
			}
		}
	}
}
