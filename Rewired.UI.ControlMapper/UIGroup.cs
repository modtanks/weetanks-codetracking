using TMPro;
using UnityEngine;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
public class UIGroup : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _label;

	[SerializeField]
	private Transform _content;

	public string labelText
	{
		get
		{
			return (_label != null) ? _label.text : string.Empty;
		}
		set
		{
			if (!(_label == null))
			{
				_label.text = value;
			}
		}
	}

	public Transform content => _content;

	public void SetLabelActive(bool state)
	{
		if (!(_label == null))
		{
			_label.gameObject.SetActive(state);
		}
	}
}
