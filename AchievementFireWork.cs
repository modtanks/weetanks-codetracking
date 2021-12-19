using System.Collections;
using UnityEngine;

public class AchievementFireWork : MonoBehaviour
{
	public ParticleSystem PS;

	private void Start()
	{
		PS = GetComponent<ParticleSystem>();
	}

	public void NewAchievement()
	{
		StartCoroutine(FireWorks());
	}

	private IEnumerator FireWorks()
	{
		PS.Clear();
		PS.Play();
		yield return new WaitForSeconds(3f);
		PS.Stop();
	}
}
