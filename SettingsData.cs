using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingsData
{
	public int musicVolLevel;

	public int masterVolLevel;

	public int sfxVolLevel;

	public int graphicsSettings;

	public bool Fullscreen;

	public int resolution;

	public int fps;

	public bool friendlyFire;

	public int difficultySetting;

	public bool[] AIcompanion;

	public bool SnowyMode;

	public int CompletedCustomCampaigns;

	public int[] AM;

	public bool MarkedTanks;

	public bool vsync;

	public bool GoreMode;

	public bool xraybullets;

	public bool[] MapEditorTankMessagesReceived;

	public int UIsettings;

	public List<int> ActivatedAM = new List<int>();

	public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

	public bool[] AIactived;

	public SettingsData(OptionsMainMenu OMM)
	{
		musicVolLevel = OMM.musicVolumeLvl;
		masterVolLevel = OMM.masterVolumeLvl;
		sfxVolLevel = OMM.sfxVolumeLvl;
		graphicsSettings = OMM.currentGraphicSettings;
		Fullscreen = OMM.isFullscreen;
		resolution = OMM.currentResolutionSettings;
		friendlyFire = OMM.FriendlyFire;
		fps = OMM.currentFPSSettings;
		difficultySetting = OMM.currentDifficulty;
		AIcompanion = OMM.AIcompanion;
		SnowyMode = OMM.SnowMode;
		AM = OMM.AM;
		ActivatedAM = OMM.AMselected;
		CompletedCustomCampaigns = OMM.CompletedCustomCampaigns;
		MarkedTanks = OMM.MarkedTanks;
		keys = OMM.keys;
		vsync = OMM.vsync;
		GoreMode = OMM.BloodMode;
		MapEditorTankMessagesReceived = OMM.MapEditorTankMessagesReceived;
		UIsettings = OMM.UIsetting;
		xraybullets = OMM.showxraybullets;
		AIactived = OMM.MenuCompanion;
	}
}
