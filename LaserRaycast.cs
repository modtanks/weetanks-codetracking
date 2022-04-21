using Rewired;
using UnityEngine;

public class LaserRaycast : MonoBehaviour
{
	public LineRenderer myLine;

	public Gradient[] TeamColors;

	public EnemyAI EA;

	public MoveTankScript MTS;

	private LayerMask myLayerMasks;

	public Transform myParent;

	private void Start()
	{
		myLine = GetComponent<LineRenderer>();
		if ((bool)EA)
		{
			myLine.colorGradient = TeamColors[EA.MyTeam];
		}
		else if ((bool)MTS)
		{
			myLine.colorGradient = TeamColors[MTS.playerId + 1];
			if (!GameMaster.instance.isPlayingWithController && MTS.playerId != 0)
			{
			}
		}
		myParent = base.transform.parent;
		myLayerMasks = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("EnemyBorder")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
	}

	private void Update()
	{
		if ((bool)GameMaster.instance && (bool)MapEditorMaster.instance && !MapEditorMaster.instance.inPlayingMode && !GameMaster.instance.GameHasStarted)
		{
			myLine.enabled = false;
			return;
		}
		if ((bool)MTS && MTS.playerId == 0)
		{
			if (ReInput.players.GetPlayer(0).controllers.GetLastActiveController().type == ControllerType.Joystick)
			{
				myParent.gameObject.SetActive(value: true);
				base.transform.SetParent(myParent);
				myLine.enabled = true;
			}
			else
			{
				base.transform.SetParent(myParent.parent);
				myLine.enabled = false;
				myParent.gameObject.SetActive(value: false);
			}
		}
		Vector3 testdir = base.transform.forward;
		Vector3 dir = base.transform.position - base.transform.forward;
		Vector3 fwd = base.transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast(base.transform.position, base.transform.forward, out var rayhit, 50f, myLayerMasks))
		{
			float length = Vector3.Distance(base.transform.position, rayhit.point);
			Vector3 newPos = new Vector3(0f, 0f, length / 3f);
			myLine.SetPosition(1, newPos);
		}
	}
}
