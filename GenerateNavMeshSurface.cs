using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GenerateNavMeshSurface : MonoBehaviour
{
	private static GenerateNavMeshSurface _instance;

	private NavMeshSurface theFloor;

	private bool isRecalcing;

	public static GenerateNavMeshSurface instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
		theFloor = GetComponent<NavMeshSurface>();
		GenerateSurface();
	}

	private void Start()
	{
	}

	public void InitiateRecalc()
	{
		if (!isRecalcing)
		{
			isRecalcing = true;
			StartCoroutine(RecalcSurface());
		}
	}

	private IEnumerator RecalcSurface()
	{
		yield return new WaitForSeconds(0.5f);
		isRecalcing = false;
		GenerateSurface();
	}

	public void GenerateSurface()
	{
		theFloor.BuildNavMesh();
	}
}
