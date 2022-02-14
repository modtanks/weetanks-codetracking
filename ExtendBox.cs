using UnityEngine;

public class ExtendBox : MonoBehaviour
{
	public TutorialMaster TM;

	public bool IsExtended;

	public void DoBox()
	{
		if (IsExtended)
		{
			IsExtended = false;
			TM.StartCoroutine(TM.Inscend());
		}
		else
		{
			IsExtended = true;
			TM.StartCoroutine(TM.ExtendBox(new Vector2(TM.TextSize.x + TM.Padding, TM.Box.sizeDelta.y)));
		}
	}
}
