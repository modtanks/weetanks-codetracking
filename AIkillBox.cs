using UnityEngine;

public class AIkillBox : MonoBehaviour
{
	private RectTransform myRect;

	public Vector3 oldpos;

	public Vector3 newpos;

	public float speed = 1f;

	public float startTime;

	private float journeyLength;

	public bool Show = false;

	public Transform followObject;

	public float posYoffset = 0f;

	public bool Changing = false;

	private void Start()
	{
		myRect = GetComponent<RectTransform>();
		oldpos = new Vector3(0f, 0f, 0f);
		newpos = myRect.localScale;
		myRect.localScale = oldpos;
		startTime = Time.time;
		journeyLength = Vector3.Distance(newpos, oldpos);
	}

	private void Update()
	{
		if ((bool)MapEditorMaster.instance && (GameMaster.instance.PlayerTeamColor[0] != GameMaster.instance.PlayerTeamColor[1] || GameMaster.instance.PlayerTeamColor[1] == 0))
		{
			return;
		}
		myRect.position = followObject.position + new Vector3(0f, posYoffset, 0f);
		if (!Changing)
		{
			if (GameMaster.instance.OnlyCompanionLeft && !Show)
			{
				Show = true;
				Changing = true;
				startTime = Time.time;
			}
			else if (!GameMaster.instance.OnlyCompanionLeft && Show)
			{
				Show = false;
				Changing = true;
				startTime = Time.time;
			}
		}
		if (Show)
		{
			float distanceNow2 = Vector3.Distance(myRect.localScale, newpos);
			if (distanceNow2 > 0.01f)
			{
				float distCovered2 = (Time.time - startTime) * speed;
				float fractionOfJourney2 = distCovered2 / journeyLength;
				myRect.localScale = Vector3.Lerp(myRect.localScale, newpos, fractionOfJourney2);
			}
			else
			{
				Changing = false;
			}
			return;
		}
		float distanceNow = Vector3.Distance(myRect.localScale, oldpos);
		if (distanceNow > 0.01f)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fractionOfJourney = distCovered / journeyLength;
			myRect.localScale = Vector3.Lerp(myRect.localScale, oldpos, fractionOfJourney);
		}
		else
		{
			Changing = false;
			myRect.localScale = Vector3.zero;
		}
	}
}
