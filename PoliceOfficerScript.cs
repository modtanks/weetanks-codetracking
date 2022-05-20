using UnityEngine;

public class PoliceOfficerScript : MonoBehaviour
{
	private EnemyAI EA;

	private EnemyTargetingSystemNew ETSN;

	public bool IsGuarding;

	public bool IsPatrolling;

	private void Start()
	{
		EA = GetComponent<EnemyAI>();
		ETSN = EA.ETSN;
		if (IsGuarding)
		{
			ETSN.IsJustLookingAround = true;
		}
	}

	private void Update()
	{
	}
}
