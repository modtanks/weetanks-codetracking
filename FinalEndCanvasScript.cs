using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalEndCanvasScript : MonoBehaviour
{
	public TextMeshProUGUI text;

	public TextMeshProUGUI textShadow;

	private string originaltext;

	public float typeSpeed = 0.2f;

	public AudioClip winCampaignSound;

	private void Start()
	{
		originaltext = text.text;
		text.text = "";
		textShadow.text = "";
	}

	private IEnumerator ShowText()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Temp");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.name == "TempAudio")
			{
				Object.Destroy(gameObject);
			}
		}
		Play2DClipAtPoint(winCampaignSound, 1f);
		for (int i = 0; i <= originaltext.Length; i++)
		{
			text.text = originaltext.Substring(0, i);
			textShadow.text = originaltext.Substring(0, i);
			yield return new WaitForSeconds(typeSpeed);
		}
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(0);
	}

	public void Play2DClipAtPoint(AudioClip clip, float Vol)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = Vol;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
