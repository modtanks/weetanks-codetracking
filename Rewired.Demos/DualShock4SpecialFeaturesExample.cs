using System.Collections.Generic;
using Rewired.ControllerExtensions;
using UnityEngine;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class DualShock4SpecialFeaturesExample : MonoBehaviour
{
	private class Touch
	{
		public GameObject go;

		public int touchId = -1;
	}

	private const int maxTouches = 2;

	public int playerId = 0;

	public Transform touchpadTransform;

	public GameObject lightObject;

	public Transform accelerometerTransform;

	private List<Touch> touches;

	private Queue<Touch> unusedTouches;

	private bool isFlashing;

	private GUIStyle textStyle;

	private Player player => ReInput.players.GetPlayer(playerId);

	private void Awake()
	{
		InitializeTouchObjects();
	}

	private void Update()
	{
		if (!ReInput.isReady)
		{
			return;
		}
		IDualShock4Extension ds4 = GetFirstDS4(player);
		if (ds4 != null)
		{
			base.transform.rotation = ds4.GetOrientation();
			HandleTouchpad(ds4);
			Vector3 accelerometerValue = ds4.GetAccelerometerValue();
			accelerometerTransform.LookAt(accelerometerTransform.position + accelerometerValue);
		}
		if (player.GetButtonDown("CycleLight"))
		{
			SetRandomLightColor();
		}
		if (player.GetButtonDown("ResetOrientation"))
		{
			ResetOrientation();
		}
		if (player.GetButtonDown("ToggleLightFlash"))
		{
			if (isFlashing)
			{
				StopLightFlash();
			}
			else
			{
				StartLightFlash();
			}
			isFlashing = !isFlashing;
		}
		if (player.GetButtonDown("VibrateLeft"))
		{
			ds4.SetVibration(0, 1f, 1f);
		}
		if (player.GetButtonDown("VibrateRight"))
		{
			ds4.SetVibration(1, 1f, 1f);
		}
	}

	private void OnGUI()
	{
		if (textStyle == null)
		{
			textStyle = new GUIStyle(GUI.skin.label);
			textStyle.fontSize = 20;
			textStyle.wordWrap = true;
		}
		if (GetFirstDS4(player) != null)
		{
			GUILayout.BeginArea(new Rect(200f, 100f, (float)Screen.width - 400f, (float)Screen.height - 200f));
			GUILayout.Label("Rotate the Dual Shock 4 to see the model rotate in sync.", textStyle);
			GUILayout.Label("Touch the touchpad to see them appear on the model.", textStyle);
			ActionElementMap aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "ResetOrientation", skipDisabledMaps: true);
			if (aem != null)
			{
				GUILayout.Label("Press " + aem.elementIdentifierName + " to reset the orientation. Hold the gamepad facing the screen with sticks pointing up and press the button.", textStyle);
			}
			aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "CycleLight", skipDisabledMaps: true);
			if (aem != null)
			{
				GUILayout.Label("Press " + aem.elementIdentifierName + " to change the light color.", textStyle);
			}
			aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "ToggleLightFlash", skipDisabledMaps: true);
			if (aem != null)
			{
				GUILayout.Label("Press " + aem.elementIdentifierName + " to start or stop the light flashing.", textStyle);
			}
			aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "VibrateLeft", skipDisabledMaps: true);
			if (aem != null)
			{
				GUILayout.Label("Press " + aem.elementIdentifierName + " vibrate the left motor.", textStyle);
			}
			aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "VibrateRight", skipDisabledMaps: true);
			if (aem != null)
			{
				GUILayout.Label("Press " + aem.elementIdentifierName + " vibrate the right motor.", textStyle);
			}
			GUILayout.EndArea();
		}
	}

	private void ResetOrientation()
	{
		GetFirstDS4(player)?.ResetOrientation();
	}

	private void SetRandomLightColor()
	{
		IDualShock4Extension ds4 = GetFirstDS4(player);
		if (ds4 != null)
		{
			Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
			ds4.SetLightColor(color);
			lightObject.GetComponent<MeshRenderer>().material.color = color;
		}
	}

	private void StartLightFlash()
	{
		if (GetFirstDS4(player) is DualShock4Extension ds4)
		{
			ds4.SetLightFlash(0.5f, 0.5f);
		}
	}

	private void StopLightFlash()
	{
		if (GetFirstDS4(player) is DualShock4Extension ds4)
		{
			ds4.StopLightFlash();
		}
	}

	private IDualShock4Extension GetFirstDS4(Player player)
	{
		foreach (Joystick i in player.controllers.Joysticks)
		{
			IDualShock4Extension ds4 = i.GetExtension<IDualShock4Extension>();
			if (ds4 == null)
			{
				continue;
			}
			return ds4;
		}
		return null;
	}

	private void InitializeTouchObjects()
	{
		touches = new List<Touch>(2);
		unusedTouches = new Queue<Touch>(2);
		for (int i = 0; i < 2; i++)
		{
			Touch touch = new Touch();
			touch.go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			touch.go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			touch.go.transform.SetParent(touchpadTransform, worldPositionStays: true);
			touch.go.GetComponent<MeshRenderer>().material.color = ((i == 0) ? Color.red : Color.green);
			touch.go.SetActive(value: false);
			unusedTouches.Enqueue(touch);
		}
	}

	private void HandleTouchpad(IDualShock4Extension ds4)
	{
		for (int j = touches.Count - 1; j >= 0; j--)
		{
			Touch touch = touches[j];
			if (!ds4.IsTouchingByTouchId(touch.touchId))
			{
				touch.go.SetActive(value: false);
				unusedTouches.Enqueue(touch);
				touches.RemoveAt(j);
			}
		}
		for (int i = 0; i < ds4.maxTouches; i++)
		{
			if (ds4.IsTouching(i))
			{
				int touchId = ds4.GetTouchId(i);
				Touch touch2 = touches.Find((Touch x) => x.touchId == touchId);
				if (touch2 == null)
				{
					touch2 = unusedTouches.Dequeue();
					touches.Add(touch2);
				}
				touch2.touchId = touchId;
				touch2.go.SetActive(value: true);
				ds4.GetTouchPosition(i, out var position);
				touch2.go.transform.localPosition = new Vector3(position.x - 0.5f, 0.5f + touch2.go.transform.localScale.y * 0.5f, position.y - 0.5f);
			}
		}
	}
}
