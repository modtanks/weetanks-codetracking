using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BossVoiceLines : MonoBehaviour
{
	private HealthTanks HT;

	public int lastPlayedLine = 999;

	public AudioMixerGroup BossMixer;

	public AudioClip[] NormalBossLines;

	public AudioClip[] LowHealthBossLines;

	public AudioClip[] DyingBossLines;

	public AudioClip[] KilledPlayerLines;

	public AudioClip[] GettingHitLines;

	public AudioClip[] SpecialAttacks;

	public AudioClip[] SpecialAttacks2;

	private bool KilledPlayerLine;

	private void Start()
	{
		HT = GetComponent<HealthTanks>();
		StartCoroutine(DoVoiceLine());
	}

	public void PlayDeathSound()
	{
	}

	public void PlayHitSound()
	{
	}

	private IEnumerator DoVoiceLine()
	{
		yield break;
	}

	private void PlayVoiceLine(AudioClip[] AudioArray, bool ByPass)
	{
	}
}
