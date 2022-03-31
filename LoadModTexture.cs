using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LoadModTexture : MonoBehaviour
{
	public string filename;

	public string filename_normal;

	public bool IsAudio = false;

	public bool IsModded = false;

	private AudioClip AC;

	private void Start()
	{
		if (IsAudio)
		{
			if (!UpdateAudio(".ogg", AudioType.OGGVORBIS) && !UpdateAudio(".wav", AudioType.WAV))
			{
				bool bababoey = !UpdateAudio(".mp3", AudioType.MPEG);
			}
		}
		else
		{
			UpdateTexture();
		}
	}

	private bool UpdateAudio(string extension, AudioType type)
	{
		string savePath = "";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/mods/" + filename + extension;
		if (File.Exists(savePath))
		{
			StartCoroutine(LoadClip(savePath));
			return true;
		}
		return false;
	}

	private IEnumerator LoadClip(string path)
	{
		UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, AudioType.WAV);
		yield return req.SendWebRequest();
		AC = DownloadHandlerAudioClip.GetContent(req);
		GetComponent<AudioSource>().clip = AC;
		IsModded = true;
	}

	private void UpdateTexture()
	{
		switch (filename)
		{
		case "wood_block_1":
			if ((bool)GlobalAssets.instance.WoodenBlock_1)
			{
				SetTexture(GlobalAssets.instance.WoodenBlock_1);
				return;
			}
			break;
		case "wood_block_2":
			if ((bool)GlobalAssets.instance.WoodenBlock_2)
			{
				SetTexture(GlobalAssets.instance.WoodenBlock_2);
				return;
			}
			break;
		case "wood_block_1_half":
			if ((bool)GlobalAssets.instance.WoodenBlock_1_half)
			{
				SetTexture(GlobalAssets.instance.WoodenBlock_1_half);
				return;
			}
			break;
		case "wood_block_2_half":
			if ((bool)GlobalAssets.instance.WoodenBlock_2_half)
			{
				SetTexture(GlobalAssets.instance.WoodenBlock_2_half);
				return;
			}
			break;
		case "stone_block":
			if ((bool)GlobalAssets.instance.StoneBlock)
			{
				SetTexture(GlobalAssets.instance.StoneBlock);
				return;
			}
			break;
		case "cork_block":
			if ((bool)GlobalAssets.instance.CorkBlock)
			{
				SetTexture(GlobalAssets.instance.CorkBlock);
				return;
			}
			break;
		}
		if (!ApplyTexture(filename, 0, ".jpg") && ApplyTexture(filename, 0, ".png"))
		{
		}
	}

	public bool ApplyTexture(string filename, int type, string extension)
	{
		string savePath = "";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/mods/" + filename + extension;
		if (File.Exists(savePath))
		{
			Texture2D texture = new Texture2D(2, 2);
			byte[] fileData = File.ReadAllBytes(savePath);
			texture.LoadImage(fileData);
			if ((bool)texture)
			{
				switch (filename)
				{
				case "wood_block_1":
					GlobalAssets.instance.WoodenBlock_1 = texture;
					break;
				case "wood_block_2":
					GlobalAssets.instance.WoodenBlock_2_half = texture;
					break;
				case "wood_block_1_half":
					GlobalAssets.instance.WoodenBlock_1_half = texture;
					break;
				case "wood_block_2_half":
					GlobalAssets.instance.WoodenBlock_2_half = texture;
					break;
				case "stone_block":
					GlobalAssets.instance.StoneBlock = texture;
					break;
				case "cork_block":
					GlobalAssets.instance.CorkBlock = texture;
					break;
				}
				SetTexture(texture);
				return true;
			}
		}
		return false;
	}

	private void SetTexture(Texture texture)
	{
		GetComponent<MeshRenderer>().material.SetTexture("_BumpMap", null);
		GetComponent<MeshRenderer>().material.mainTexture = texture;
		GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1f, 1f);
		IsModded = true;
	}
}
