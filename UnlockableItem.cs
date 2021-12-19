using UnityEngine;

[CreateAssetMenu(fileName = "Unlockable", menuName = "Unlockable")]
public class UnlockableItem : ScriptableObject
{
	public string UnlockableName = "";

	public string UnlockableRequirement = "";

	public string code;

	public bool codeNeededToUnlock;

	public int ULID;

	public bool isSkin;

	public bool isMine;

	public bool isSkidmarks;

	public bool isBullet;

	public bool isBoost;

	public bool isHitmarker;
}
