using System.Text;
using Rewired.UI;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UnityUI;

public class PlayerPointerEventData : PointerEventData
{
	public int playerId { get; set; }

	public int inputSourceIndex { get; set; }

	public IMouseInputSource mouseSource { get; set; }

	public ITouchInputSource touchSource { get; set; }

	public PointerEventType sourceType { get; set; }

	public int buttonIndex { get; set; }

	public PlayerPointerEventData(EventSystem eventSystem)
		: base(eventSystem)
	{
		playerId = -1;
		inputSourceIndex = -1;
		buttonIndex = -1;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("<b>Player Id</b>: " + playerId);
		sb.AppendLine("<b>Mouse Source</b>: " + mouseSource);
		sb.AppendLine("<b>Input Source Index</b>: " + inputSourceIndex);
		sb.AppendLine("<b>Touch Source/b>: " + touchSource);
		sb.AppendLine("<b>Source Type</b>: " + sourceType);
		sb.AppendLine("<b>Button Index</b>: " + buttonIndex);
		sb.Append(base.ToString());
		return sb.ToString();
	}
}
