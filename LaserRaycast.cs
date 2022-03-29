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
			if (!GameMaster.instance.isPlayingWithController)
			{
				_ = MTS.playerId;
			}
		}
		myParent = base.transform.parent;
		myLayerMasks = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("EnemyBorder")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
	}

	private void Update()
	{
		if ((bool)MTS && MTS.playerId == 0)
		{
			if (ReInput.players.GetPlayer(0).controllers.GetLastActiveController().type == ControllerType.Joystick)
			{
				Debug.Log("ENABLOE LASER");
				myParent.gameObject.SetActive(value: true);
				base.transform.SetParent(myParent);
				myLine.enabled = true;
			}
			else
			{
				Debug.Log("DISABLE LASER");
				base.transform.SetParent(myParent.parent);
				myLine.enabled = false;
				myParent.gameObject.SetActive(value: false);
			}
		}
		_ = base.transform.forward;
		_ = base.transform.position - base.transform.forward;
		base.transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast(base.transform.position, base.transform.forward, out var hitInfo, 50f, myLayerMasks))
		{
			float num = Vector3.Distance(base.transform.position, hitInfo.point);
			Vector3 position = new Vector3(0f, 0f, num / 3f);
			myLine.SetPosition(1, position);
		}
	}
}
