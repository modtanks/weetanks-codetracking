using System.Collections;
using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
	public Transform[] Spawnpoints;

	public GameObject TrainPrefab;

	public GameObject myTrain;

	public int SpawnSpeed = 18;

	public int SpawnSpeedOffsetRandom = 5;

	public AudioClip ChooChoo;

	private bool isTrainRunning = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Solid" && !GameMaster.instance.GameHasStarted)
		{
			MeshRenderer MR = other.GetComponent<MeshRenderer>();
			if ((bool)MR)
			{
				MR.enabled = false;
			}
		}
	}

	private void Update()
	{
		if ((GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode) && !isTrainRunning)
		{
			StartCoroutine("TrainSystem");
		}
		if (!GameMaster.instance.GameHasStarted && !GameMaster.instance.inMenuMode && isTrainRunning)
		{
			StopCoroutine("TrainSystem");
			isTrainRunning = false;
		}
	}

	private IEnumerator TrainSystem()
	{
		isTrainRunning = true;
		Debug.LogWarning("train is gonna wait");
		int waitTime = SpawnSpeed + Random.Range(-SpawnSpeedOffsetRandom, SpawnSpeedOffsetRandom);
		yield return new WaitForSeconds(waitTime);
		if ((bool)myTrain)
		{
			StartCoroutine("TrainSystem");
			yield break;
		}
		Play2DClipAtPoint(ChooChoo);
		yield return new WaitForSeconds(0.9f);
		int pick = Random.Range(0, Spawnpoints.Length);
		myTrain = Object.Instantiate(TrainPrefab, Spawnpoints[pick]);
		Debug.LogWarning("train spawned");
		StartCoroutine("TrainSystem");
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
