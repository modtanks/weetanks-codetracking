using System.Collections.Generic;
using System.Text;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos;

[AddComponentMenu("")]
public sealed class PlayerPointerEventHandlerExample : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public Text text;

	private const int logLength = 10;

	private List<string> log = new List<string>();

	private void Log(string o)
	{
		log.Add(o);
		if (log.Count > 10)
		{
			log.RemoveAt(0);
		}
	}

	private void Update()
	{
		if (!(text != null))
		{
			return;
		}
		StringBuilder sb = new StringBuilder();
		foreach (string s in log)
		{
			sb.AppendLine(s);
		}
		text.text = sb.ToString();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnPointerEnter:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData));
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnPointerExit:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData));
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnPointerUp:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData) + ", Button Index = " + playerEventData.buttonIndex);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnPointerDown:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData) + ", Button Index = " + playerEventData.buttonIndex);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnPointerClick:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData) + ", Button Index = " + playerEventData.buttonIndex);
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnScroll:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData));
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnBeginDrag:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData) + ", Button Index = " + playerEventData.buttonIndex);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnDrag:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData) + ", Button Index = " + playerEventData.buttonIndex);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData is PlayerPointerEventData)
		{
			PlayerPointerEventData playerEventData = (PlayerPointerEventData)eventData;
			Log("OnEndDrag:  Player = " + playerEventData.playerId + ", Pointer Index = " + playerEventData.inputSourceIndex + ", Source = " + GetSourceName(playerEventData) + ", Button Index = " + playerEventData.buttonIndex);
		}
	}

	private static string GetSourceName(PlayerPointerEventData playerEventData)
	{
		if (playerEventData.sourceType == PointerEventType.Mouse)
		{
			if (playerEventData.mouseSource is Behaviour)
			{
				return (playerEventData.mouseSource as Behaviour).name;
			}
		}
		else if (playerEventData.sourceType == PointerEventType.Touch && playerEventData.touchSource is Behaviour)
		{
			return (playerEventData.touchSource as Behaviour).name;
		}
		return null;
	}
}
