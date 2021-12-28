using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairScript : MonoBehaviour
{
	public List<Texture> CrosshairTex = new List<Texture>();

	public CrosshairPrefab SelectedCrosshair;

	public CrosshairPrefab OriginalCrosshair;

	public int framesPerSecond = 15;

	public int CrosshairWidth = 50;

	public int CrosshairHeight = 50;

	public RawImage RI;

	public bool showCursor = true;

	private static CrosshairScript _instance;

	public static CrosshairScript instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Update()
	{
		if (ReInput.players.GetPlayer(0).controllers.GetController(ControllerType.Joystick, 0) == null)
		{
			showCursor = true;
		}
		else if (ReInput.players.GetPlayer(0) != null)
		{
			if (ReInput.players.GetPlayer(0).controllers.GetLastActiveController() != null)
			{
				if (ReInput.players.GetPlayer(0).controllers.GetLastActiveController().type == ControllerType.Joystick)
				{
					showCursor = false;
				}
				else
				{
					showCursor = true;
				}
			}
			else
			{
				showCursor = true;
			}
		}
		else
		{
			showCursor = true;
		}
		RI.enabled = showCursor;
		if (SelectedCrosshair == null)
		{
			SelectedCrosshair = OriginalCrosshair;
		}
		if (SelectedCrosshair.CrosshairFrames.Length > 1)
		{
			int num = Mathf.RoundToInt(Time.time * (float)SelectedCrosshair.FPS % (float)SelectedCrosshair.CrosshairFrames.Length);
			if (num != SelectedCrosshair.CrosshairFrames.Length)
			{
				RI.texture = SelectedCrosshair.CrosshairFrames[num];
			}
		}
		else
		{
			RI.texture = SelectedCrosshair.CrosshairFrames[0];
		}
		RI.GetComponent<RectTransform>().sizeDelta = new Vector2(SelectedCrosshair.CrosshairSize, SelectedCrosshair.CrosshairSize);
		Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		base.transform.position = vector;
	}
}
