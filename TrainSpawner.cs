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

	private bool isTrainRunning;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Solid" && !GameMaster.instance.GameHasStarted)
		{
			MeshRenderer component = other.GetComponent<MeshRenderer>();
			if ((bool)component)
			{
				component.enabled = false;
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
		int num = SpawnSpeed + Random.Range(-SpawnSpeedOffsetRandom, SpawnSpeedOffsetRandom);
		yield return new WaitForSeconds(num);
		if ((bool)myTrain)
		{
			StartCoroutine("TrainSystem");
			yield break;
		}
		Play2DClipAtPoint(ChooChoo);
		yield return new WaitForSeconds(0.9f);
		int num2 = Random.Range(0, Spawnpoints.Length);
		myTrain = Object.Instantiate(TrainPrefab, Spawnpoints[num2]);
		Debug.LogWarning("train spawned");
		StartCoroutine("TrainSystem");
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
