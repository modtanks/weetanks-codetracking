using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWhite : MonoBehaviour
{
	public List<Renderer> MyRenderers = new List<Renderer>();

	public bool GetRenderersFromParent = true;

	private void Start()
	{
		MeshRenderer[] rs = ((!GetRenderersFromParent) ? base.gameObject.GetComponentsInChildren<MeshRenderer>() : base.transform.parent.gameObject.GetComponentsInChildren<MeshRenderer>());
		MeshRenderer[] array = rs;
		foreach (MeshRenderer r in array)
		{
			if (r != null && r.gameObject.tag == "Untagged" && r.material.HasProperty("_Color"))
			{
				MyRenderers.Add(r);
			}
		}
	}

	public void GotHit()
	{
		StartCoroutine(DoHitWhite());
	}

	private IEnumerator DoHitWhite()
	{
		foreach (Renderer r in MyRenderers)
		{
			if (!(r != null) || !(r.gameObject.tag == "Untagged") || !r.material.HasProperty("_Color"))
			{
				continue;
			}
			if (r.materials.Length > 1)
			{
				Material[] ms2 = r.materials;
				for (int j = 0; j < r.materials.Length; j++)
				{
					ms2[j].EnableKeyword("_EMISSION");
					ms2[j].SetColor("_EmissionColor", Color.white);
				}
				r.materials = ms2;
			}
			else if (!r.material.IsKeywordEnabled("_EMISSION"))
			{
				r.material.EnableKeyword("_EMISSION");
				r.material.SetColor("_EmissionColor", Color.white);
			}
			else
			{
				r.material.DisableKeyword("_EMISSION");
			}
		}
		yield return new WaitForSeconds(0.05f);
		for (int i = 0; i < MyRenderers.Count; i++)
		{
			if (!(MyRenderers[i] != null) || !(MyRenderers[i].gameObject.tag == "Untagged") || !MyRenderers[i].material.HasProperty("_Color"))
			{
				continue;
			}
			if (MyRenderers[i].materials.Length > 1)
			{
				Material[] ms = MyRenderers[i].materials;
				for (int k = 0; k < MyRenderers[i].materials.Length; k++)
				{
					ms[k].DisableKeyword("_EMISSION");
					ms[k].SetColor("_EmissionColor", Color.black);
				}
				MyRenderers[i].materials = ms;
			}
			else if (MyRenderers[i].material.IsKeywordEnabled("_EMISSION"))
			{
				MyRenderers[i].material.DisableKeyword("_EMISSION");
				MyRenderers[i].material.SetColor("_EmissionColor", Color.black);
			}
			else
			{
				MyRenderers[i].material.EnableKeyword("_EMISSION");
			}
		}
	}
}
