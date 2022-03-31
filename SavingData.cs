using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SavingData
{
	public static void SaveData(GameMaster GM, string filename)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = Application.persistentDataPath + "/" + filename + ".tnk";
		FileStream stream = new FileStream(savePath, FileMode.Create);
		ProgressDataNew data = new ProgressDataNew(GM, null);
		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static void OverwriteData(ProgressData olddata)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = Application.persistentDataPath + "/tank_progress.tnk";
		FileStream stream = new FileStream(savePath, FileMode.Create);
		ProgressDataNew data = new ProgressDataNew(null, olddata);
		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static void SaveSettingsData(OptionsMainMenu OMM)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = Application.persistentDataPath + "/tank_settings.tnk";
		FileStream stream = new FileStream(savePath, FileMode.Create);
		SettingsData data = new SettingsData(OMM);
		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static ProgressDataNew LoadData()
	{
		string savePath = Application.persistentDataPath + "/tank_progress.tnk";
		if (File.Exists(savePath) && new FileInfo(savePath).Length != 0)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(savePath, FileMode.Open);
			if (formatter.Deserialize(stream) is ProgressData olddata)
			{
				if (olddata.totalTankKills > 0)
				{
					stream.Close();
					OverwriteData(olddata);
					stream = new FileStream(savePath, FileMode.Open);
				}
				else
				{
					stream.Close();
					stream = new FileStream(savePath, FileMode.Open);
				}
			}
			else
			{
				stream.Close();
				stream = new FileStream(savePath, FileMode.Open);
			}
			ProgressDataNew data = formatter.Deserialize(stream) as ProgressDataNew;
			stream.Close();
			return data;
		}
		Debug.LogError("Save file not found!!" + savePath);
		return null;
	}

	public static SettingsData LoadSettingsData()
	{
		string savePath = Application.persistentDataPath + "/tank_settings.tnk";
		if (File.Exists(savePath))
		{
			FileStream stream = new FileStream(savePath, FileMode.Open);
			Debug.Log(Application.persistentDataPath);
			if (stream.Length > 0)
			{
				BinaryFormatter formatter = new BinaryFormatter();
				SettingsData data = formatter.Deserialize(stream) as SettingsData;
				stream.Close();
				return data;
			}
			Debug.LogError("Save file corrupted!!");
			return null;
		}
		Debug.LogError("Settings file not found!!" + savePath);
		return null;
	}

	public static bool ExistData()
	{
		string savePath = Application.persistentDataPath + "/tank_progress.tnk";
		if (File.Exists(savePath) && new FileInfo(savePath).Length != 0)
		{
			return true;
		}
		return false;
	}

	public static bool ExistSettingsData()
	{
		string savePath = Application.persistentDataPath + "/tank_settings.tnk";
		if (File.Exists(savePath))
		{
			Debug.LogWarning("Save settings file found!!" + savePath);
			return true;
		}
		Debug.LogError("Save settings file not found!!" + savePath);
		return false;
	}
}
