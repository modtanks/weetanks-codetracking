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
		bool existsAlready = false;
		int counter = 0;
		if (rendererInfo == null || rendererInfo.Count == 0)
		{
			return;
		}
		LightmapData[] settingslightmaps = LightmapSettings.lightmaps;
		List<LightmapData> combinedLightmaps = new List<LightmapData>();
		int[] lightmapArrayOffsetIndex = new int[lightmaps.Length];
		for (int j = 0; j < lightmaps.Length; j++)
		{
			existsAlready = false;
			for (int k = 0; k < settingslightmaps.Length; k++)
			{
				if (lightmaps[j] == settingslightmaps[k].lightmapColor)
				{
					lightmapArrayOffsetIndex[j] = k;
					existsAlready = true;
				}
			}
			if (!existsAlready)
			{
				lightmapArrayOffsetIndex[j] = counter + settingslightmaps.Length;
				LightmapData newLightmapData = new LightmapData();
				newLightmapData.lightmapColor = lightmaps[j];
				newLightmapData.lightmapDir = lightmaps2[j];
				combinedLightmaps.Add(newLightmapData);
				counter++;
			}
		}
		LightmapData[] combinedLightmaps2 = new LightmapData[settingslightmaps.Length + combinedLightmaps.Count];
		settingslightmaps.CopyTo(combinedLightmaps2, 0);
		if (counter > 0)
		{
			for (int i = 0; i < combinedLightmaps.Count; i++)
			{
				combinedLightmaps2[i + settingslightmaps.Length] = new LightmapData();
				combinedLightmaps2[i + settingslightmaps.Length].lightmapColor = combinedLightmaps[i].lightmapColor;
				combinedLightmaps2[i + settingslightmaps.Length].lightmapDir = combinedLightmaps[i].lightmapDir;
			}
		}
		ApplyRendererInfo(rendererInfo, lightmapArrayOffsetIndex);
		LightmapSettings.lightmaps = combinedLightmaps2;
	}

	private static void ApplyRendererInfo(List<RendererInfo> infos, int[] arrayOffsetIndex)
	{
		for (int i = 0; i < infos.Count; i++)
		{
			RendererInfo info = infos[i];
			foreach (Renderer rer in info.renderer)
			{
				rer.lightmapIndex = arrayOffsetIndex[info.lightmapIndex];
				rer.lightmapScaleOffset = info.lightmapOffsetScale;
			}
		}
	}
}
