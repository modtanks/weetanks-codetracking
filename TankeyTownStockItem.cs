using UnityEngine;

[CreateAssetMenu(fileName = "StockItem", menuName = "StockItem")]
public class TankeyTownStockItem : ScriptableObject
{
	public int ItemID;

	public string ItemName;

	public GameObject ItemObject;

	public float ItemYoffset;

	public Texture ItemTexture;

	public bool IsMapEditorObject;

	public bool IsMapEditorFloor;

	public bool isSkin;

	public bool isMine;

	public bool isSkidmarks;

	public bool isBullet;

	public bool isBoost;

	public bool isHitmarker;

	public GameObject MapEditorPrefab;

	public int MapEditorPropID;

	public float MapEditorYoffset;
}
