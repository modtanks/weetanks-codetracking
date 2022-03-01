using System;

[Serializable]
public class CustomTankData
{
	public SerializableColor CustomTankColor = new SerializableColor();

	public string CustomTankName;

	public int CustomTankSpeed;

	public float CustomFireSpeed;

	public int CustomBounces;

	public int CustomBullets;

	public float CustomMineSpeed;

	public int CustomTurnHead;

	public int CustomAccuracy;

	public bool CustomLayMines;

	public int CustomBulletType;

	public int CustomMusic;

	public bool CustomInvisibility;

	public bool CustomCalculateShots;

	public bool CustomArmoured;

	public int CustomArmourPoints;

	public float CustomTankScale;

	public bool CustomCanBeAirdropped;

	public bool CustomCanTeleport;

	public bool CustomShowHealthbar;

	public int CustomBulletsPerShot;

	public int CustomTankHealth;

	public string UniqueTankID;

	public int CustomBulletSpeed;
}
