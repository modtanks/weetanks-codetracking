using System.Collections;
using UnityEngine;

public class DestroyableWall : MonoBehaviour
{
	public GameObject ExplosionPrefab;

	public GameObject underMe;

	public bool hasChecked = false;

	public bool isHalfSlab = false;

	public bool IsStone = false;

	public bool IsSpawnedIn = false;

	private bool PreventDouble = false;

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
		LayerMask LM = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("FLOOR"));
		if (Physics.Raycast(base.transform.position, -Vector3.up, out var hit, 9f, LM))
		{
			if (hit.transform.tag == "Solid" || hit.collider.tag == "Solid" || hit.collider.tag == "Floor" || hit.transform.tag == "Floor")
			{
				underMe = hit.collider.gameObject;
			}
			if (underMe != null)
			{
				hasChecked = true;
			}
			else
			{
				Debug.LogWarning("Checked, but not the right tag!");
				Debug.LogWarning(hit.transform.tag + hit.transform.name);
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
			GameObject explosion = Object.Instantiate(ExplosionPrefab, base.transform.position, base.transform.rotation);
			Object.Destroy(explosion, 5f);
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
		GameObject explosion = Object.Instantiate(ExplosionPrefab, base.transform.position, base.transform.rotation);
		Object.Destroy(explosion, 5f);
		if (GameMaster.instance.isZombieMode)
		{
			GenerateNavMeshSurface.instance.InitiateRecalc();
		}
		Object.Destroy(base.gameObject);
	}
}
