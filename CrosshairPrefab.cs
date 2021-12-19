using UnityEngine;

[CreateAssetMenu(fileName = "Crosshair", menuName = "Crosshair")]
public class CrosshairPrefab : ScriptableObject
{
	public int AMID;

	public int FPS;

	public int CrosshairSize;

	public Texture[] CrosshairFrames;
}
