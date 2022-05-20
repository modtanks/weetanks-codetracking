using UnityEngine;

public class ShowMessageOnTriggerEnter : MonoBehaviour
{
	public string message;

	public bool HasShown;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !HasShown)
		{
			HasShown = true;
			TutorialMaster.instance.ShowTutorial(message);
		}
	}
}
