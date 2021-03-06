using System;
using System.IO;
using System.Linq;
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
			Debug.LogError(text);
			return false;
		}
		FileStream fileStream = new FileStream(text, FileMode.Create);
		MapEditorData graph = new MapEditorData(GM, MEM);
		binaryFormatter.Serialize(fileStream, graph);
		Debug.LogError("File saved at " + text);
		fileStream.Close();
		return true;
	}

	public static bool AutoSaveMap(GameMaster GM, MapEditorMaster MEM)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string text = "autosave_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
		string text2 = Application.persistentDataPath + "/autosaves/" + text + ".campaign";
		text2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		string path = text2 + "/My Games/Wee Tanks/autosaves";
		text2 = text2 + "/My Games/Wee Tanks/autosaves/" + text + ".campaign";
		new FileInfo(text2).Directory.Create();
		foreach (FileInfo item in (from x in new DirectoryInfo(path).GetFiles()
			orderby x.LastWriteTime descending
			select x).Skip(49))
		{
			item.Delete();
		}
		FileStream fileStream = new FileStream(text2, FileMode.Create);
		MapEditorData graph = new MapEditorData(GM, MEM);
		binaryFormatter.Serialize(fileStream, graph);
		Debug.LogError("File saved at " + text2);
		fileStream.Close();
		return true;
	}

	public static bool SaveClassicCampaignMap(GameMaster GM, MapEditorMaster MEM, string filename, TextAsset CampaignMap)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		string text = null;
		text = text.Replace("Assets", "");
		Debug.Log("save path now is: " + text);
		Debug.Log("dataPath : " + Application.dataPath);
		text = Application.dataPath + text;
		FileStream fileStream = new FileStream(text, FileMode.Create);
		SingleMapEditorData graph = new SingleMapEditorData(GM, MEM);
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
			Debug.LogError(text);
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

	public static SingleMapEditorData LoadDataFromTXT(TextAsset mapObject)
	{
		Stream stream = new MemoryStream(mapObject.bytes);
		SingleMapEditorData result = new BinaryFormatter().Deserialize(stream) as SingleMapEditorData;
		stream.Close();
		return result;
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
