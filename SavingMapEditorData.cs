using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavingMapEditorData
{
	public static bool SaveMap(GameMaster GM, MapEditorMaster MEM, string filename, bool overwrite)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = Application.persistentDataPath + "/" + filename + ".campaign";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/" + filename + ".campaign";
		FileInfo file = new FileInfo(savePath);
		file.Directory.Create();
		if (File.Exists(savePath) && !overwrite)
		{
			Debug.LogError("File already exists!");
			Debug.LogError(savePath);
			return false;
		}
		FileStream stream = new FileStream(savePath, FileMode.Create);
		MapEditorData data = new MapEditorData(GM, MEM);
		formatter.Serialize(stream, data);
		Debug.LogError("File saved at " + savePath);
		stream.Close();
		return true;
	}

	public static bool SaveClassicCampaignMap(GameMaster GM, MapEditorMaster MEM, string filename, TextAsset CampaignMap)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = null;
		savePath = savePath.Replace("Assets", "");
		Debug.Log("save path now is: " + savePath);
		Debug.Log("dataPath : " + Application.dataPath);
		savePath = Application.dataPath + savePath;
		FileStream stream = new FileStream(savePath, FileMode.Create);
		SingleMapEditorData data = new SingleMapEditorData(GM, MEM);
		formatter.Serialize(stream, data);
		Debug.LogError("File saved at " + savePath);
		stream.Close();
		return true;
	}

	public static bool SaveCampaignMap(GameMaster GM, MapEditorMaster MEM, string filename, bool overwrite)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = Application.persistentDataPath + "/" + filename + ".txt";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/" + filename + ".txt";
		FileInfo file = new FileInfo(savePath);
		file.Directory.Create();
		if (File.Exists(savePath) && !overwrite)
		{
			Debug.LogError("File already exists!");
			Debug.LogError(savePath);
			return false;
		}
		FileStream stream = new FileStream(savePath, FileMode.Create);
		SingleMapEditorData data = new SingleMapEditorData(GM, MEM);
		formatter.Serialize(stream, data);
		Debug.LogError("File saved at " + savePath);
		stream.Close();
		return true;
	}

	public static MapEditorData LoadData(string filename)
	{
		string savePath = Application.persistentDataPath + "/" + filename + ".campaign";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/" + filename + ".campaign";
		if (File.Exists(savePath))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(savePath, FileMode.Open);
			stream.Seek(0L, SeekOrigin.Begin);
			MapEditorData data = formatter.Deserialize(stream) as MapEditorData;
			stream.Close();
			return data;
		}
		return null;
	}

	public static SingleMapEditorData LoadDataFromTXT(TextAsset mapObject)
	{
		Stream stream = new MemoryStream(mapObject.bytes);
		BinaryFormatter formatter = new BinaryFormatter();
		SingleMapEditorData data = formatter.Deserialize(stream) as SingleMapEditorData;
		stream.Close();
		return data;
	}

	public static bool ReSaveMap(MapEditorData data, string path)
	{
		File.Delete(path);
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = File.Create(path);
		formatter.Serialize(stream, data);
		Debug.LogError("File saved at " + path);
		stream.Close();
		return true;
	}
}
