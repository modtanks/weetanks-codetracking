using System;

[Serializable]
public class CustomTankData
{
	public SerializableColor CustomTankColor = new SerializableColor();

	public string CustomTankName;

	public int CustomTankSpeed = 50;

	public float CustomFireSpeed;

	public int CustomBounces;

	public int CustomBullets = 1;

	public float CustomMineSpeed;

	public int CustomTurnHead = 3;

	public int CustomAccuracy;

	public bool CustomLayMines;

	public int CustomBulletType;

	public int CustomMusic;

	public bool CustomInvisibility;

	public bool CustomCalculateShots;

	public bool CustomArmoured;

	public int CustomArmourPoints;

	public float CustomTankScale = 1f;

	public bool CustomCanBeAirdropped;

	public bool CustomCanTeleport;

	public bool CustomShowHealthbar;

	public int CustomBulletsPerShot;

	public int CustomTankHealth;

	public string UniqueTankID;

	public int CustomBulletSpeed = 7;

	public bool CanShootAirMissiles = false;

	public int CustomMissileCapacity = 1;

	public float CustomMissileReloadSpeed;
}
