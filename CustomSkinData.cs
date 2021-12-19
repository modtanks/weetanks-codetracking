using UnityEngine;

[CreateAssetMenu(fileName = "Skin", menuName = "Skin")]
public class CustomSkinData : ScriptableObject
{
	public int AMselectedID;

	public Material HeadMaterial;

	public Material BarrelMaterial;

	public Material WheelsMaterial;

	public Material InnerBodyMaterial;

	public Material OuterBodyMaterial;
}
