using System.Collections;
using UnityEngine;

public class SkidmarkController : MonoBehaviour
{
	[Header("Colors")]
	public Color startingColor;

	public Color otherColor;

	public Color originalColor;

	[Header("Extras")]
	public float lasting;

	private ParticleSystem PS;

	private bool startLerping = false;

	private float LerpSpeed = 0f;

	private float passedTime = 0f;

	public Rigidbody rbParent;

	private void Awake()
	{
	}

	private void Start()
	{
		PS = GetComponent<ParticleSystem>();
		PS.Stop();
		rbParent = base.transform.parent.GetComponent<Rigidbody>();
	}

	public IEnumerator SetTrackColor()
	{
		if (!PS)
		{
			PS = GetComponent<ParticleSystem>();
		}
		ParticleSystem.MainModule main = PS.main;
		main.startColor = startingColor;
		yield return new WaitForSeconds(2f);
		StartCoroutine(SetTrackColor());
	}

	public void ActivateBlood(Color othercolor)
	{
		ParticleSystem.MainModule main = PS.main;
		main.startColor = othercolor;
		otherColor = othercolor;
		startLerping = true;
		LerpSpeed = 0.08f;
		passedTime = 0f;
	}

	private void Update()
	{
		if ((bool)rbParent && rbParent.velocity.magnitude >= 0.5f)
		{
			if (passedTime < 1f)
			{
				passedTime += Time.deltaTime * LerpSpeed;
			}
			else
			{
				startLerping = false;
			}
			if (startLerping)
			{
				ParticleSystem.MainModule main = PS.main;
				main.startColor = Color.Lerp(otherColor, startingColor, passedTime);
			}
		}
	}
}
