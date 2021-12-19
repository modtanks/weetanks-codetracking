using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnClickDo : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public UnityEvent myUnityEvent;

	private void Awake()
	{
		if (myUnityEvent == null)
		{
			myUnityEvent = new UnityEvent();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		myUnityEvent.Invoke();
	}
}
