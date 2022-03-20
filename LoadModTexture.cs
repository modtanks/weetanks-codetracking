using System;
using System.IO;
using UnityEngine;

public class LoadModTexture : MonoBehaviour
{
	public string filename;

	private void Start()
	{
		UpdateTexture();
	}

	private void UpdateTexture()
	{
		string text = "";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text = text + "/My Games/Wee Tanks/mods/" + filename + ".png";
		Debug.Log(text);
		Texture2D texture2D = new Texture2D(2, 2);
		if (File.Exists(text))
		{
			byte[] data = File.ReadAllBytes(text);
			texture2D.LoadImage(data);
			if ((bool)texture2D)
			{
				GetComponent<MeshRenderer>().material.mainTexture = texture2D;
			}
			return;
		}
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text = text + "/My Games/Wee Tanks/mods/" + filename + ".jpeg";
		if (File.Exists(text))
		{
			byte[] data2 = File.ReadAllBytes(text);
			texture2D.LoadImage(data2);
			if ((bool)texture2D)
			{
				GetComponent<MeshRenderer>().material.mainTexture = texture2D;
			}
		}
	}
}
