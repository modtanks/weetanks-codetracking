using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorUIprop : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public RawImage myImage;

	public int PropID;

	public int minimumMission = -1;

	public Color normalColor;

	public Color hoverColor;

	public Color selectedColor;

	public Color unavailableColor;

	private bool selected;

	public bool mouseOnMe;

	public bool canSelectMe = true;

	public bool BypassBobBuilderAchievement;

	public int[] NeedCustomMissions;

	private void Start()
	{
		myImage = GetComponent<RawImage>();
		normalColor = myImage.color;
		if (NeedCustomMissions.Length != 0)
		{
			int num = 0;
			int[] needCustomMissions = NeedCustomMissions;
			foreach (int item in needCustomMissions)
			{
				if (GameMaster.instance.FoundSecretMissions.Contains(item))
				{
					num++;
				}
			}
			if ((!OptionsMainMenu.instance.AMselected.Contains(60) || PropID != 49) && (!OptionsMainMenu.instance.AMselected.Contains(33) || PropID != 26) && num != NeedCustomMissions.Length)
			{
				myImage.color = unavailableColor;
				canSelectMe = false;
			}
		}
		else
		{
			if (minimumMission > GameMaster.instance.maxMissionReached && minimumMission > GameMaster.instance.maxMissionReachedHard && minimumMission > GameMaster.instance.maxMissionReachedKid && !OptionsMainMenu.instance.AMselected.Contains(60))
			{
				myImage.color = unavailableColor;
				canSelectMe = false;
			}
			if (minimumMission > GameMaster.instance.maxMissionReached && minimumMission > GameMaster.instance.maxMissionReachedHard && minimumMission > GameMaster.instance.maxMissionReachedKid && BypassBobBuilderAchievement && (PropID != 26 || (!OptionsMainMenu.instance.AMselected.Contains(33) && GameMaster.instance.FoundSecretMissions.Count <= 9)) && (PropID != 27 || !OptionsMainMenu.instance.AMselected.Contains(34)))
			{
				myImage.color = unavailableColor;
				canSelectMe = false;
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseOnMe = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseOnMe = false;
	}

	private void Update()
	{
		if (MapEditorMaster.instance.MenuCurrent == 2 && (PropID == 19 || PropID == 20 || PropID == 21))
		{
			MapEditorMaster.instance.SelectedPropUITextureMenu = MapEditorMaster.instance.MenuCurrent;
			MapEditorMaster.instance.SelectedProp = PropID;
			MapEditorMaster.instance.SelectedPropUITexture.transform.position = new Vector3(base.transform.position.x, MapEditorMaster.instance.SelectedPropUITexture.transform.position.y, MapEditorMaster.instance.SelectedPropUITexture.transform.position.z);
			selected = true;
		}
		if (MapEditorMaster.instance.SelectedProp == PropID && canSelectMe)
		{
			myImage.color = normalColor;
			MapEditorMaster.instance.SelectedPropUITexture.transform.position = new Vector3(base.transform.position.x, MapEditorMaster.instance.SelectedPropUITexture.transform.position.y, MapEditorMaster.instance.SelectedPropUITexture.transform.position.z);
			selected = true;
		}
		else if (!mouseOnMe)
		{
			if (canSelectMe)
			{
				myImage.color = normalColor;
			}
			else
			{
				myImage.color = unavailableColor;
			}
			selected = false;
		}
		if (!mouseOnMe)
		{
			return;
		}
		myImage.color = (canSelectMe ? hoverColor : unavailableColor);
		Color color = myImage.color;
		color.a = (canSelectMe ? 1f : 0.8f);
		myImage.color = color;
		if (Input.GetMouseButtonDown(0) && !selected && canSelectMe)
		{
			MapEditorMaster.instance.Play2DClipAtPoint(MapEditorMaster.instance.MenuSelect);
			MapEditorMaster.instance.SelectedPropUITextureMenu = MapEditorMaster.instance.MenuCurrent;
			MapEditorMaster.instance.SelectedProp = PropID;
			MapEditorMaster.instance.SelectedPropUITexture.transform.position = new Vector3(base.transform.position.x, MapEditorMaster.instance.SelectedPropUITexture.transform.position.y, MapEditorMaster.instance.SelectedPropUITexture.transform.position.z);
			selected = true;
		}
		else if (Input.GetMouseButtonDown(0) && selected)
		{
			MapEditorMaster.instance.Play2DClipAtPoint(MapEditorMaster.instance.MenuSelect);
			MapEditorMaster.instance.SelectedProp = -1;
			MapEditorMaster.instance.SelectedPropUITexture.transform.position = new Vector3(-5000f, MapEditorMaster.instance.SelectedPropUITexture.transform.position.y, 0f);
			selected = false;
		}
		else if (Input.GetMouseButtonDown(0) && !selected && !canSelectMe)
		{
			if (BypassBobBuilderAchievement)
			{
				MapEditorMaster.instance.ShowErrorMessage("ERROR: You have not unlocked this tank yet!");
			}
			else if (NeedCustomMissions.Length != 0)
			{
				MapEditorMaster.instance.ShowErrorMessage("ERROR: You have not unlocked this block yet!");
			}
			else
			{
				MapEditorMaster.instance.ShowErrorMessage("ERROR: You need to reach mission " + minimumMission + " to unlock!");
			}
		}
	}
}
