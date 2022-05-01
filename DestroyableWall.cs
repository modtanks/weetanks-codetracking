using System.Collections;
using UnityEngine;

public class DestroyableWall : MonoBehaviour
{
	public GameObject ExplosionPrefab;

	public GameObject underMe;

	public bool hasChecked;

	public bool isHalfSlab;

	public bool IsStone;

	public bool IsSpawnedIn;

	private bool PreventDouble;

	private void Start()
	{
		StartCoroutine(LateCheck());
	}

	private void OnEnable()
	{
		StartCoroutine(LateCheck());
	}

	private IEnumerator LateCheck()
	{
		yield return new WaitForSeconds(0.5f);
		Check();
	}

	private void Check()
	{
		if (IsStone)
		{
			return;
		}
		LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("FLOOR"));
		if (Physics.Raycast(base.transform.position, -Vector3.up, out var hitInfo, 9f, layerMask))
		{
			if (hitInfo.transform.tag == "Solid" || hitInfo.collider.tag == "Solid" || hitInfo.collider.tag == "Floor" || hitInfo.transform.tag == "Floor")
			{
				underMe = hitInfo.collider.gameObject;
			}
			if (underMe != null)
			{
				hasChecked = true;
			}
			else
			{
				Debug.LogWarning("Checked, but not the right tag!");
				Debug.LogWarning(hitInfo.transform.tag + hitInfo.transform.name);
				hasChecked = true;
			}
		}
		else
		{
			Debug.LogWarning("NO HITS!");
			hasChecked = true;
		}
		hasChecked = true;
	}

	private void Update()
	{
		if (!PreventDouble)
		{
			if (underMe == null && hasChecked)
			{
				PreventDouble = true;
				StartCoroutine(destroy());
			}
			else if (IsSpawnedIn && !GameMaster.instance.GameHasStarted)
			{
				PreventDouble = true;
				StartCoroutine(destroy());
			}
		}
	}

	public IEnumerator destroy()
	{
		if (!IsStone)
		{
			yield return new WaitForSeconds(0.1f);
			Object.Destroy(Object.Instantiate(ExplosionPrefab, base.transform.position, base.transform.rotation), 5f);
			if (GameMaster.instance.isZombieMode)
			{
				GenerateNavMeshSurface.instance.InitiateRecalc();
			}
			Object.Destroy(base.gameObject);
		}
	}

	public IEnumerator stonedestroy()
	{
		yield return new WaitForSeconds(0.1f);
		Object.Destroy(Object.Instantiate(ExplosionPrefab, base.transform.position, base.transform.rotation), 5f);
		if (GameMaster.instance.isZombieMode)
		{
			GenerateNavMeshSurface.instance.InitiateRecalc();
		}
		Object.Destroy(base.gameObject);
	}
}
