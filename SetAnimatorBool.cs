using UnityEngine;

public class SetAnimatorBool : MonoBehaviour
{
	private Animator Anim;

	public string boolName;

	private void Start()
	{
		Anim = GetComponent<Animator>();
	}

	public void EnableBool()
	{
		Anim.SetBool(boolName, value: true);
	}

	public void DisableBool()
	{
		Anim.SetBool(boolName, value: false);
	}
}
