using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationsScript : MonoBehaviour
{
	public GameMaster headscript;

	public MusicHandler musicScript;

	private Animator myanimator;

	public CountDownScript countscript;

	public TMP_Text[] defeatText;

	public TextMeshProUGUI BonusTankCheckpointText;

	private void Start()
	{
		myanimator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (headscript.inDemoMode)
		{
			myanimator.SetBool("Demo", value: true);
		}
	}

	private void ReadyGame()
	{
		countscript.start = true;
		myanimator.SetBool("Restart", value: false);
		myanimator.SetBool("Defeat", value: false);
		musicScript.StartMusic();
		headscript.DisableGame();
	}

	private void SetCheckPointText()
	{
		BonusTankCheckpointText.text = "You have reached checkpoint " + GameMaster.instance.CurrentMission + "!";
	}

	private void NextLevel()
	{
		if (GameMaster.instance != null && GameMaster.instance.CurrentMission == 50)
		{
			SceneManager.LoadScene(0);
		}
		myanimator.SetBool("Restart", value: false);
		myanimator.SetBool("Defeat", value: false);
		headscript.nextLevel();
	}

	private void RestartLevel()
	{
		headscript.ResetLevel();
	}

	private void SwitchSound()
	{
	}

	private void BonusTank()
	{
		GameMaster.instance.AddBonusTank();
	}

	public IEnumerator LostGame()
	{
		if (!GameMaster.instance.isZombieMode)
		{
			if (headscript.Lives <= 1)
			{
				TMP_Text[] array = defeatText;
				foreach (TMP_Text dftext in array)
				{
					dftext.text = "Game over!";
				}
				myanimator.SetBool("Restart", value: false);
				yield return new WaitForSeconds(2f);
				SceneManager.LoadScene(0);
			}
			else
			{
				yield return new WaitForSeconds(2f);
				myanimator.SetBool("Restart", value: true);
				myanimator.SetBool("Defeat", value: false);
				yield return new WaitForSeconds(2f);
				headscript.Lives--;
				headscript.playLostLive();
			}
		}
		else
		{
			yield return new WaitForSeconds(3f);
			SceneManager.LoadScene(0);
		}
	}
}
