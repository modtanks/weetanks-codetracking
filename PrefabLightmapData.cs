using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLightmapData : MonoBehaviour
{
	[Serializable]
	private struct RendererInfo
	{
		public List<Renderer> renderer;

		public int lightmapIndex;

		public Vector4 lightmapOffsetScale;
	}

	[Serializable]
	private struct Texture2D_Remap
	{
		public int originalLightmapIndex;

		public Texture2D originalLightmap;

		public Texture2D lightmap;

		public Texture2D lightmap2;
	}

	[SerializeField]
	private List<RendererInfo> m_RendererInfo;

	[SerializeField]
	private Texture2D[] m_Lightmaps;

	[SerializeField]
	private Texture2D[] m_Lightmaps2;

	[SerializeField]
	public static string LIGHTMAP_RESOURCE_PATH = "Assets/Resources/Lightmaps/";

	private static List<Texture2D_Remap> sceneLightmaps = new List<Texture2D_Remap>();

	private void Awake()
	{
		ApplyLightmaps(m_RendererInfo, m_Lightmaps, m_Lightmaps2);
	}

	private void Start()
	{
		StaticBatchingUtility.Combine(base.gameObject);
	}

	private static void ApplyLightmaps(List<RendererInfo> rendererInfo, Texture2D[] lightmaps, Texture2D[] lightmaps2)
	{
		bool flag = false;
		int num = 0;
		if (rendererInfo == null || rendererInfo.Count == 0)
		{
			return;
		}
		LightmapData[] lightmaps3 = LightmapSettings.lightmaps;
		List<LightmapData> list = new List<LightmapData>();
		int[] array = new int[lightmaps.Length];
		for (int i = 0; i < lightmaps.Length; i++)
		{
			flag = false;
			for (int j = 0; j < lightmaps3.Length; j++)
			{
				if (lightmaps[i] == lightmaps3[j].lightmapColor)
				{
					array[i] = j;
					flag = true;
				}
			}
			if (!flag)
			{
				array[i] = num + lightmaps3.Length;
				LightmapData lightmapData = new LightmapData();
				lightmapData.lightmapColor = lightmaps[i];
				lightmapData.lightmapDir = lightmaps2[i];
				list.Add(lightmapData);
				num++;
			}
		}
		LightmapData[] array2 = new LightmapData[lightmaps3.Length + list.Count];
		lightmaps3.CopyTo(array2, 0);
		if (num > 0)
		{
			for (int k = 0; k < list.Count; k++)
			{
				array2[k + lightmaps3.Length] = new LightmapData();
				array2[k + lightmaps3.Length].lightmapColor = list[k].lightmapColor;
				array2[k + lightmaps3.Length].lightmapDir = list[k].lightmapDir;
			}
		}
		ApplyRendererInfo(rendererInfo, array);
		LightmapSettings.lightmaps = array2;
	}

	private static void ApplyRendererInfo(List<RendererInfo> infos, int[] arrayOffsetIndex)
	{
		for (int i = 0; i < infos.Count; i++)
		{
			RendererInfo rendererInfo = infos[i];
			foreach (Renderer item in rendererInfo.renderer)
			{
				item.lightmapIndex = arrayOffsetIndex[rendererInfo.lightmapIndex];
				item.lightmapScaleOffset = rendererInfo.lightmapOffsetScale;
			}
		}
	}
}
