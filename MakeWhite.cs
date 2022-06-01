using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWhite : MonoBehaviour
{
	public List<Renderer> MyRenderers = new List<Renderer>();

	public bool GetRenderersFromParent = true;

	private void Start()
	{
		MeshRenderer[] array = ((!GetRenderersFromParent) ? base.gameObject.GetComponentsInChildren<MeshRenderer>() : base.transform.parent.gameObject.GetComponentsInChildren<MeshRenderer>());
		MeshRenderer[] array2 = array;
		foreach (MeshRenderer meshRenderer in array2)
		{
			if (meshRenderer != null && !meshRenderer.name.Contains("SSAO") && meshRenderer.gameObject.tag == "Untagged" && meshRenderer.material.HasProperty("_Color"))
			{
				MyRenderers.Add(meshRenderer);
			}
		}
	}

	public void GotHit()
	{
		StartCoroutine(DoHitWhite());
	}

	private IEnumerator DoHitWhite()
	{
		foreach (Renderer myRenderer in MyRenderers)
		{
			if (!(myRenderer != null) || myRenderer.name.Contains("SSAO") || !(myRenderer.gameObject.tag == "Untagged") || !myRenderer.material.HasProperty("_Color"))
			{
				continue;
			}
			if (myRenderer.materials.Length > 1)
			{
				Material[] materials = myRenderer.materials;
				for (int i = 0; i < myRenderer.materials.Length; i++)
				{
					materials[i].EnableKeyword("_EMISSION");
					materials[i].SetColor("_EmissionColor", Color.white);
				}
				myRenderer.materials = materials;
			}
			else if (!myRenderer.material.IsKeywordEnabled("_EMISSION"))
			{
				myRenderer.material.EnableKeyword("_EMISSION");
				myRenderer.material.SetColor("_EmissionColor", Color.white);
			}
			else
			{
				myRenderer.material.DisableKeyword("_EMISSION");
			}
		}
		yield return new WaitForSeconds(0.05f);
		for (int j = 0; j < MyRenderers.Count; j++)
		{
			if (!(MyRenderers[j] != null) || MyRenderers[j].name.Contains("SSAO") || !(MyRenderers[j].gameObject.tag == "Untagged") || !MyRenderers[j].material.HasProperty("_Color"))
			{
				continue;
			}
			if (MyRenderers[j].materials.Length > 1)
			{
				Material[] materials2 = MyRenderers[j].materials;
				for (int k = 0; k < MyRenderers[j].materials.Length; k++)
				{
					materials2[k].DisableKeyword("_EMISSION");
					materials2[k].SetColor("_EmissionColor", Color.black);
				}
				MyRenderers[j].materials = materials2;
			}
			else if (MyRenderers[j].material.IsKeywordEnabled("_EMISSION"))
			{
				MyRenderers[j].material.DisableKeyword("_EMISSION");
				MyRenderers[j].material.SetColor("_EmissionColor", Color.black);
			}
			else
			{
				MyRenderers[j].material.EnableKeyword("_EMISSION");
			}
		}
	}
}
