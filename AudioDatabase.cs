using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Database", menuName = "Audio Database")]
public class AudioDatabase : ScriptableObject
{
	public AudioClip[] ArmourHits;

	public AudioClip MenuClick;

	public AudioClip success;

	public AudioClip failure;

	public AudioClip click;

	public AudioClip swoosh;

	public AudioClip unlock;

	public AudioClip extraLife;

	public AudioClip lostLife;

	public AudioClip trackCarpet;

	public AudioClip trackGrass;

	public AudioClip track;

	public List<AudioClip> NormalBulletShootSound;

	public List<AudioClip> RocketBulletShootSound;

	public List<AudioClip> ExplosiveBulletShootSound;

	public List<AudioClip> ElectricBulletShootSound;
}
