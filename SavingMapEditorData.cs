using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavingMapEditorData
{
	public static bool SaveMap(GameMaster GM, MapEditorMaster MEM, string filename, bool overwrite)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string text = Application.persistentDataPath + "/" + filename + ".campaign";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text = text + "/My Games/Wee Tanks/" + filename + ".campaign";
		new FileInfo(text).Directory.Create();
		if (File.Exists(text) && !overwrite)
		{
			Debug.LogError("File already exists!");
			return false;
		}
		FileStream fileStream = new FileStream(text, FileMode.Create);
		MapEditorData graph = new MapEditorData(GM, MEM);
		binaryFormatter.Serialize(fileStream, graph);
		Debug.LogError("File saved at " + text);
		fileStream.Close();
		return true;
	}

	public static bool SaveCampaignMap(GameMaster GM, MapEditorMaster MEM, string filename, bool overwrite)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string text = Application.persistentDataPath + "/" + filename + ".txt";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text = text + "/My Games/Wee Tanks/" + filename + ".txt";
		new FileInfo(text).Directory.Create();
		if (File.Exists(text) && !overwrite)
		{
			Debug.LogError("File already exists!");
			return false;
		}
		FileStream fileStream = new FileStream(text, FileMode.Create);
		SingleMapEditorData graph = new SingleMapEditorData(GM, MEM);
		binaryFormatter.Serialize(fileStream, graph);
		Debug.LogError("File saved at " + text);
		fileStream.Close();
		return true;
	}

	public static MapEditorData LoadData(string filename)
	{
		string text = Application.persistentDataPath + "/" + filename + ".campaign";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text = text + "/My Games/Wee Tanks/" + filename + ".campaign";
		if (File.Exists(text))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(text, FileMode.Open);
			fileStream.Seek(0L, SeekOrigin.Begin);
			MapEditorData result = binaryFormatter.Deserialize(fileStream) as MapEditorData;
			fileStream.Close();
			return result;
		}
		return null;
	}

	public static bool ReSaveMap(MapEditorData data, string path)
	{
		File.Delete(path);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Create(path);
		binaryFormatter.Serialize(fileStream, data);
		Debug.LogError("File saved at " + path);
		fileStream.Close();
		return true;
	}
}
