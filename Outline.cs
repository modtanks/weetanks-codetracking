using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
	public enum Mode
	{
		OutlineAll,
		OutlineVisible,
		OutlineHidden,
		OutlineAndSilhouette,
		SilhouetteOnly
	}

	[Serializable]
	private class ListVector3
	{
		public List<Vector3> data;
	}

	private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

	[SerializeField]
	private Mode outlineMode;

	[SerializeField]
	private Color outlineColor = Color.white;

	[SerializeField]
	[Range(0f, 10f)]
	private float outlineWidth = 2f;

	[Header("Optional")]
	[SerializeField]
	[Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
	private bool precomputeOutline;

	[SerializeField]
	[HideInInspector]
	private List<Mesh> bakeKeys = new List<Mesh>();

	[SerializeField]
	[HideInInspector]
	private List<ListVector3> bakeValues = new List<ListVector3>();

	private List<Renderer> renderers = new List<Renderer>();

	private Material outlineMaskMaterial;

	private Material outlineFillMaterial;

	private bool needsUpdate;

	public Mode OutlineMode
	{
		get
		{
			return outlineMode;
		}
		set
		{
			outlineMode = value;
			needsUpdate = true;
		}
	}

	public Color OutlineColor
	{
		get
		{
			return outlineColor;
		}
		set
		{
			outlineColor = value;
			needsUpdate = true;
		}
	}

	public float OutlineWidth
	{
		get
		{
			return outlineWidth;
		}
		set
		{
			outlineWidth = value;
			needsUpdate = true;
		}
	}

	private void Awake()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in componentsInChildren)
		{
			renderers.Add(rend);
		}
		List<Renderer> RendToRemove = new List<Renderer>();
		foreach (Renderer renderer2 in renderers)
		{
			if (renderer2.gameObject.GetComponent<ParticleSystem>() != null)
			{
				RendToRemove.Add(renderer2);
			}
		}
		foreach (Renderer renderer in RendToRemove)
		{
			renderers.Remove(renderer);
		}
		outlineMaskMaterial = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/OutlineMask"));
		outlineFillMaterial = UnityEngine.Object.Instantiate(Resources.Load<Material>("Materials/OutlineFill"));
		outlineMaskMaterial.name = "OutlineMask (Instance)";
		outlineFillMaterial.name = "OutlineFill (Instance)";
		LoadSmoothNormals();
		needsUpdate = true;
	}

	private void OnEnable()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Material> materials = renderer.sharedMaterials.ToList();
			materials.Add(outlineMaskMaterial);
			materials.Add(outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}

	private void OnValidate()
	{
		needsUpdate = true;
		if ((!precomputeOutline && bakeKeys.Count != 0) || bakeKeys.Count != bakeValues.Count)
		{
			bakeKeys.Clear();
			bakeValues.Clear();
		}
		if (precomputeOutline && bakeKeys.Count == 0)
		{
			Bake();
		}
	}

	private void Update()
	{
		if (needsUpdate)
		{
			needsUpdate = false;
			UpdateMaterialProperties();
		}
	}

	private void OnDisable()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Material> materials = renderer.sharedMaterials.ToList();
			materials.Remove(outlineMaskMaterial);
			materials.Remove(outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(outlineMaskMaterial);
		UnityEngine.Object.Destroy(outlineFillMaterial);
	}

	private void Bake()
	{
		HashSet<Mesh> bakedMeshes = new HashSet<Mesh>();
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			if (bakedMeshes.Add(meshFilter.sharedMesh))
			{
				List<Vector3> smoothNormals = SmoothNormals(meshFilter.sharedMesh);
				bakeKeys.Add(meshFilter.sharedMesh);
				bakeValues.Add(new ListVector3
				{
					data = smoothNormals
				});
			}
		}
	}

	private void LoadSmoothNormals()
	{
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			if (registeredMeshes.Add(meshFilter.sharedMesh))
			{
				int index = bakeKeys.IndexOf(meshFilter.sharedMesh);
				List<Vector3> smoothNormals = ((index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh));
				meshFilter.sharedMesh.SetUVs(3, smoothNormals);
			}
		}
		SkinnedMeshRenderer[] componentsInChildren2 = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
		{
			if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
			{
				skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
			}
		}
	}

	private List<Vector3> SmoothNormals(Mesh mesh)
	{
		IEnumerable<IGrouping<Vector3, KeyValuePair<Vector3, int>>> groups = from pair in mesh.vertices.Select((Vector3 vertex, int index) => new KeyValuePair<Vector3, int>(vertex, index))
			group pair by pair.Key;
		List<Vector3> smoothNormals = new List<Vector3>(mesh.normals);
		foreach (IGrouping<Vector3, KeyValuePair<Vector3, int>> group in groups)
		{
			if (group.Count() == 1)
			{
				continue;
			}
			Vector3 smoothNormal = Vector3.zero;
			foreach (KeyValuePair<Vector3, int> pair2 in group)
			{
				smoothNormal += mesh.normals[pair2.Value];
			}
			smoothNormal.Normalize();
			foreach (KeyValuePair<Vector3, int> item in group)
			{
				smoothNormals[item.Value] = smoothNormal;
			}
		}
		return smoothNormals;
	}

	private void UpdateMaterialProperties()
	{
		outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
		switch (outlineMode)
		{
		case Mode.OutlineAll:
			outlineMaskMaterial.SetFloat("_ZTest", 8f);
			outlineFillMaterial.SetFloat("_ZTest", 8f);
			outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
			break;
		case Mode.OutlineVisible:
			outlineMaskMaterial.SetFloat("_ZTest", 8f);
			outlineFillMaterial.SetFloat("_ZTest", 4f);
			outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
			break;
		case Mode.OutlineHidden:
			outlineMaskMaterial.SetFloat("_ZTest", 8f);
			outlineFillMaterial.SetFloat("_ZTest", 5f);
			outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
			break;
		case Mode.OutlineAndSilhouette:
			outlineMaskMaterial.SetFloat("_ZTest", 4f);
			outlineFillMaterial.SetFloat("_ZTest", 8f);
			outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
			break;
		case Mode.SilhouetteOnly:
			outlineMaskMaterial.SetFloat("_ZTest", 4f);
			outlineFillMaterial.SetFloat("_ZTest", 5f);
			outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
			break;
		}
	}
}
