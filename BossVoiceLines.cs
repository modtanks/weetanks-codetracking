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

	private void Start()
	{
		HT = GetComponent<HealthTanks>();
	}
}
