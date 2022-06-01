using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavingData
{
	public static void SaveData(GameMaster GM, string filename)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/" + filename + ".tnk", FileMode.Create);
		ProgressDataNew graph = new ProgressDataNew(GM, null);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
	}

	public static void OverwriteData(ProgressData olddata)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/tank_progress.tnk", FileMode.Create);
		ProgressDataNew graph = new ProgressDataNew(null, olddata);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
	}

	public static void SaveSettingsData(OptionsMainMenu OMM)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/tank_settings.tnk", FileMode.Create);
		SettingsData graph = new SettingsData(OMM);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
	}

	public static ProgressDataNew LoadData()
	{
		string text = Application.persistentDataPath + "/tank_progress.tnk";
		if (File.Exists(text) && new FileInfo(text).Length != 0L)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(text, FileMode.Open);
			if (binaryFormatter.Deserialize(fileStream) is ProgressData progressData)
			{
				if (progressData.totalTankKills > 0)
				{
					fileStream.Close();
					OverwriteData(progressData);
					fileStream = new FileStream(text, FileMode.Open);
				}
				else
				{
					fileStream.Close();
					fileStream = new FileStream(text, FileMode.Open);
				}
			}
			else
			{
				fileStream.Close();
				fileStream = new FileStream(text, FileMode.Open);
			}
			ProgressDataNew result = binaryFormatter.Deserialize(fileStream) as ProgressDataNew;
			fileStream.Close();
			return result;
		}
		Debug.LogError("Save file not found!!" + text);
		return null;
	}

	public static SettingsData LoadSettingsData()
	{
		string text = Application.persistentDataPath + "/tank_settings.tnk";
		if (File.Exists(text))
		{
			FileStream fileStream = new FileStream(text, FileMode.Open);
			Debug.Log(Application.persistentDataPath);
			if (fileStream.Length > 0)
			{
				SettingsData result = new BinaryFormatter().Deserialize(fileStream) as SettingsData;
				fileStream.Close();
				return result;
			}
			fileStream.Close();
			Debug.LogError("Save file corrupted!!");
			return null;
		}
		Debug.LogError("Settings file not found!!" + text);
		return null;
	}

	public static bool ExistData()
	{
		string text = Application.persistentDataPath + "/tank_progress.tnk";
		if (File.Exists(text) && new FileInfo(text).Length != 0L)
		{
			return true;
		}
		return false;
	}

	public static bool ExistSettingsData()
	{
		string text = Application.persistentDataPath + "/tank_settings.tnk";
		if (File.Exists(text))
		{
			Debug.LogWarning("Save settings file found!!" + text);
			return true;
		}
		Debug.LogError("Save settings file not found!!" + text);
		return false;
	}
}
