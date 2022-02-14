using UnityEngine;

[CreateAssetMenu(fileName = "Skin", menuName = "Skin")]
public class CustomSkinData : ScriptableObject
{
	public int AMselectedID;

	public Material MainMaterial;

	public Material TurretMaterial;
}
