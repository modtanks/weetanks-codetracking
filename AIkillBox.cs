using UnityEngine;

public class AIkillBox : MonoBehaviour
{
	private RectTransform myRect;

	public Vector3 oldpos;

	public Vector3 newpos;

	public float speed = 1f;

	public float startTime;

	private float journeyLength;

	public bool Show;

	public Transform followObject;

	public float posYoffset;

	public bool Changing;

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
			if (Vector3.Distance(myRect.localScale, newpos) > 0.01f)
			{
				float t = (Time.time - startTime) * speed / journeyLength;
				myRect.localScale = Vector3.Lerp(myRect.localScale, newpos, t);
			}
			else
			{
				Changing = false;
			}
		}
		else if (Vector3.Distance(myRect.localScale, oldpos) > 0.01f)
		{
			float t2 = (Time.time - startTime) * speed / journeyLength;
			myRect.localScale = Vector3.Lerp(myRect.localScale, oldpos, t2);
		}
		else
		{
			Changing = false;
			myRect.localScale = Vector3.zero;
		}
	}
}
