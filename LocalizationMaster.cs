using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationMaster : MonoBehaviour
{
	[Serializable]
	public class Locale
	{
		public string MM_options;

		public string MM_exit;

		public string MM_back;

		public string MM_extr;

		public string HUD_mission;

		public string CE_Error;

		public string Difficulty_setting_1;

		public string Difficulty_setting_2;

		public string Difficulty_setting_3;

		public string Difficulty_setting_4;

		public string MM_Checkpoint;

		public string MM_Campaign;

		public string MM_Survival;

		public string MM_Editor;

		public string MM_continue;

		public string MM_Record;

		public string MM_final_Checkpoint;

		public string MM_New_Game;

		public string SV_Boxmap;

		public string SV_Maze;

		public string SV_Fortress;

		public string SV_Field;

		public string SV_Camp;

		public string SV_mansion;

		public string SV_Castle;

		public string CM_Play;

		public string CM_Edit;

		public string Menu_Resume;

		public string Menu_Save;

		public string Menu_stop;

		public string Error_No_Block;

		public string CM_setting;

		public string CE_Border;

		public string CE_Mission_name;

		public string CE_Weather;

		public string CE_Weather_Clear;

		public string CE_Weather_Foggy;

		public string CE_Weather_Rain;

		public string CE_Weather_Thunder;

		public string CE_Floor;

		public string CE_Floor_NW;

		public string CE_Floor_HW;

		public string CE_Floor_Stone;

		public string CE_Floor_Tiles;

		public string CE_Floor_Carpet;

		public string CE_MM;

		public string CE_Lives;

		public string CE_Player_Bullets;

		public string CE_Player_Mines;

		public string CE_Player_Speed;

		public string CE_Player_Bounces;

		public string CE_Player_Armor;

		public string CE_Player_Type;

		public string CE_Bullet_Type_Normal;

		public string CE_Bullet_Type_Rocket;

		public string CE_Bullet_Type_Explosive;

		public string CE_Bullet_Type_Electric;

		public string CE_Show_Blue_Team;

		public string CE_Show_Red_Team;

		public string CE_Show_Green_Team;

		public string CE_Show_Purple_Team;

		public string CE_Custom_Tank_Header;

		public string CE_Custom_Tank_Appearance;

		public string CE_Custom_Tank_Invis;

		public string CE_Custom_Tank_Speed;

		public string CE_Custom_Tank_Turret_Speed;

		public string CE_Custom_Tank_Firing;

		public string CE_Custom_Tank_Accuracy;

		public string CE_Custom_Tank_BPS;

		public string CE_Custom_Tank_Bounces;

		public string CE_Custom_Tank_Max_Bullets;

		public string CE_Custom_Tank_BPS_shotgun;

		public string CE_Custom_Tank_Calculate;

		public string CE_Custom_Tank_Mines;

		public string CE_Custom_Tank_Mine_speed;

		public string CE_Custom_Tank_Health;

		public string CE_Custom_Tank_HP;

		public string CE_Custom_Tank_armor;

		public string CE_Custom_Tank_Healthbar;

		public string CE_Custom_Tank_Armor_Points;

		public string CE_Custom_Tank_Extras;

		public string CE_Custom_Tank_Name;

		public string CE_Custom_Tank_Music;

		public string CE_Custom_Tank_Scale;

		public string CE_Custom_Tank_Teleport;

		public string CE_Custom_Tank_Airdrop;

		public string CE_Custom_Tank_Music1;

		public string CE_Custom_Tank_Music2;

		public string CE_Custom_Tank_Music3;

		public string CE_Custom_Tank_Music4;

		public string CE_Custom_Tank_Music5;

		public string CE_Custom_Tank_Music6;

		public string CE_Custom_Tank_Music7;

		public string CE_Custom_Tank_Music8;

		public string CE_Custom_Tank_Music9;

		public string CE_Custom_Tank_Music10;

		public string CE_Custom_Tank_Music11;

		public string CE_Custom_Tank_Music12;

		public string CE_Custom_Tank_Music13;

		public string CE_Custom_Tank_Music14;

		public string CE_Custom_Tank_Music15;

		public string CE_Custom_Tank_Music16;

		public string CE_Custom_Tank_Music17;

		public string CE_Custom_Tank_Load;

		public string CE_Custom_Tank_Save;

		public string CE_Custom_Tank_Delete;

		public string CE_Add;

		public string Error_Max_enemies;

		public string Error_Max_Missions;

		public string Error_1_Mission;

		public string CE_Campaign_Name;

		public string CE_Save_Text;

		public string CE_Sign;

		public string CE_your_name;

		public string CE_Not_be_undone;

		public string CE_Name_Error;

		public string CE_File_Saved;

		public string CE_replace;

		public string CE_Yes;

		public string CE_No;

		public string CE_Save;

		public string CE_Perspective;

		public string CE_Test;

		public string CE_Stop_Testing;

		public string CE_Help;

		public string CE_Controls;

		public string CE_Add_Remove;

		public string CE_Rotate;

		public string CE_Properties;

		public string CE_Straight;

		public string CE_Layerup;

		public string CE_Layerdown;

		public string CE_Select_Layers;

		public string CE_Toggle;

		public string CE_Team;

		public string CE_Spawn;

		public string CE_Toddler;

		public string CE_Kid;

		public string CE_Adult;

		public string CE_Grandpa;

		public string CE_duplicate;

		public string CE_Remove;

		public string CE_No_name;

		public string CE_Load_Tank;

		public string CE_Loaded;

		public string CE_tank_locked;

		public string MM_MyAccount;

		public string MM_meta_maps;

		public string MM_meta_completed;

		public string MM_meta_highestwave;

		public string Difficulty;

		public string MM_signed_in;

		public string MM_not_signed_in;

		public string MM_individual_missions;

		public string SV_note;

		public string Settings_video;

		public string Settings_gameplay;

		public string Settings_audio;

		public string Settings_controls;

		public string MM_Options_video;

		public string Options_graphics;

		public string Options_resolution;

		public string Options_maxFPS;

		public string Options_fullscreen;

		public string MM_Options_audio;

		public string Options_master_volume;

		public string Options_music_volume;

		public string Options_sfx_volume;

		public string MM_Options_gameplay;

		public string Options_friendly_fire;

		public string Options_marked_tanks;

		public string Options_xray_bullets;

		public string Options_enable_snow;

		public string Extras_unlockables;

		public string Extras_achievements;

		public string Extras_statistics;

		public string Extras_credits;

		public string CM_New;

		public string MM_Completed;

		public string CM_Browse;

		public string CM_Play_campaign;

		public string MM_Highest_Wave;

		public string CM_No_maps;

		public string CM_Refresh;

		public string CE_Browse;

		public string CM_small;

		public string CM_normal;

		public string CM_big;

		public string CE_My_Campaign;

		public string CM_large;

		public string MM_Map_Editor_create;

		public string CE_All_maps;

		public string CE_Download;

		public string Unlockables_description;

		public string CE_Favorite;

		public string Achievements_description;

		public string Achievements_easy;

		public string Achievements_medium;

		public string CE_Date_High_low;

		public string Achievements_hard;

		public string Achievements_extreme;

		public string CE_Low_high;

		public string CE_Most_downloaded;

		public string CE_Least_download;

		public string Statistics_description;

		public string Statistics_campaign;

		public string Statistics_tanks;

		public string Statistics_survival;

		public string SV_header;

		public string Map_editor;

		public string Mission_1;

		public string Mission_2;

		public string Mission_3;

		public string Mission_4;

		public string Mission_5;

		public string Mission_6;

		public string Mission_7;

		public string Mission_8;

		public string Mission_9;

		public string Mission_10;

		public string Mission_11;

		public string Mission_12;

		public string Mission_13;

		public string Mission_14;

		public string Mission_15;

		public string Mission_16;

		public string Mission_17;

		public string Mission_18;

		public string Mission_19;

		public string Mission_20;

		public string Mission_21;

		public string Mission_22;

		public string Mission_23;

		public string Mission_24;

		public string Mission_25;

		public string Mission_26;

		public string Mission_27;

		public string Mission_28;

		public string Mission_29;

		public string Mission_30;

		public string Mission_31;

		public string Mission_32;

		public string Mission_33;

		public string Mission_34;

		public string Mission_35;

		public string Mission_36;

		public string Mission_37;

		public string Mission_38;

		public string Mission_39;

		public string Mission_40;

		public string Mission_41;

		public string Mission_42;

		public string Mission_43;

		public string Mission_44;

		public string Mission_45;

		public string Mission_46;

		public string Mission_47;

		public string Mission_48;

		public string Mission_49;

		public string Mission_50;

		public string Mission_51;

		public string Mission_52;

		public string Mission_53;

		public string Mission_54;

		public string Mission_55;

		public string Mission_56;

		public string Mission_57;

		public string Mission_58;

		public string Mission_59;

		public string Mission_60;

		public string Mission_61;

		public string Mission_62;

		public string Mission_63;

		public string Mission_64;

		public string Mission_65;

		public string Mission_66;

		public string Mission_67;

		public string Mission_68;

		public string Mission_69;

		public string Mission_70;

		public string Mission_71;

		public string Mission_72;

		public string Mission_73;

		public string Mission_74;

		public string Mission_75;

		public string Mission_76;

		public string Mission_77;

		public string Mission_78;

		public string Mission_79;

		public string Mission_80;

		public string Mission_81;

		public string Mission_82;

		public string Mission_83;

		public string Mission_84;

		public string Mission_85;

		public string Mission_86;

		public string Mission_87;

		public string Mission_88;

		public string Mission_89;

		public string Mission_90;

		public string Mission_91;

		public string Mission_92;

		public string Mission_93;

		public string Mission_94;

		public string Mission_95;

		public string Mission_96;

		public string Mission_97;

		public string Mission_98;

		public string Mission_99;

		public string Mission_100;

		public string Account_Title;

		public string Account_signed_in_status;

		public string Account_sign_out;

		public string Account_sign_in;

		public string Account_inventory;

		public string Account_create;

		public string Account_Tankey_town;

		public string Account_create_title;

		public string Account_name;

		public string Account_password;

		public string Account_password_again;

		public string Account_name_placeholder;

		public string Account_create_password_placeholder;

		public string Account_create_password_again_placeholder;

		public string Account_create_button_text;

		public string Account_sign_in_title;

		public string Account_sign_in_button_text;

		public string Pre_game_title;

		public string Pre_game_player;

		public string Pre_game_start;

		public string Account_status_signing_in;

		public string Account_status_sign_in_success;

		public string Account_status_sign_in_fail;

		public string bug_report_note;

		public string bug_report_title;

		public string bug_report_description_title;

		public string bug_report_description_placeholder;

		public string bug_report_reproduce_title;

		public string bug_report_send_button;

		public string HUD_tanks_left;

		public string clapperboard_dead;

		public string clapperboard_checkpoint_reached;

		public string clapperboard_bonus_tank;

		public string clapperboard_kills;

		public string HUD_enemies_left;

		public string Pause_Menu_Title;

		public string Pause_Menu_Jump;

		public string Map_name;

		public string Map_name_placeholder;

		public string Author_name;

		public string Quality_0;

		public string Quality_1;

		public string Quality_2;

		public string Quality_3;

		public string Quality_4;

		public string AM_0;

		public string AM_1;

		public string AM_2;

		public string AM_3;

		public string AM_4;

		public string AM_5;

		public string AM_6;

		public string AM_7;

		public string AM_8;

		public string AM_9;

		public string AM_10;

		public string AM_11;

		public string AM_12;

		public string AM_13;

		public string AM_14;

		public string AM_15;

		public string AM_16;

		public string AM_17;

		public string AM_18;

		public string AM_19;

		public string AM_20;

		public string AM_21;

		public string AM_22;

		public string AM_23;

		public string AM_24;

		public string AM_25;

		public string AM_26;

		public string AM_27;

		public string AM_28;

		public string AM_29;

		public string AM_30;

		public string AM_31;

		public string AM_32;

		public string AM_33;

		public string AM_34;

		public string AM_35;

		public string AM_desc_0;

		public string AM_desc_1;

		public string AM_desc_2;

		public string AM_desc_3;

		public string AM_desc_4;

		public string AM_desc_5;

		public string AM_desc_6;

		public string AM_desc_7;

		public string AM_desc_8;

		public string AM_desc_9;

		public string AM_desc_10;

		public string AM_desc_11;

		public string AM_desc_12;

		public string AM_desc_13;

		public string AM_desc_14;

		public string AM_desc_15;

		public string AM_desc_16;

		public string AM_desc_17;

		public string AM_desc_18;

		public string AM_desc_19;

		public string AM_desc_20;

		public string AM_desc_21;

		public string AM_desc_22;

		public string AM_desc_23;

		public string AM_desc_24;

		public string AM_desc_25;

		public string AM_desc_26;

		public string AM_desc_27;

		public string AM_desc_28;

		public string AM_desc_29;

		public string AM_desc_30;

		public string AM_desc_31;

		public string AM_desc_32;

		public string AM_desc_33;

		public string AM_desc_34;

		public string ToolTip_0;

		public string ToolTip_1;

		public string ToolTip_2;

		public string ToolTip_3;

		public string ToolTip_4;

		public string ToolTip_5;

		public string ToolTip_6;

		public string ToolTip_7;

		public string ToolTip_8;

		public string ToolTip_9;

		public string ToolTip_10;

		public string ToolTip_11;

		public string ToolTip_12;

		public string ToolTip_13;

		public string ToolTip_14;

		public string ToolTip_15;

		public string ToolTip_16;

		public string ToolTip_17;

		public string ToolTip_18;

		public string ToolTip_19;

		public string ToolTip_20;

		public string ToolTip_21;

		public string ToolTip_22;

		public string ToolTip_23;

		public string ToolTip_24;

		public string ToolTip_25;

		public string ToolTip_26;

		public string ToolTip_27;

		public string ToolTip_28;

		public string ToolTip_29;

		public string ToolTip_30;

		public string ToolTip_31;

		public string ToolTip_32;

		public string ToolTip_33;

		public string ToolTip_34;

		public string ToolTip_35;

		public string ToolTip_36;

		public string Credits_0;

		public string Credits_1;

		public string Credits_2;

		public string Credits_3;

		public string Credits_4;

		public string Credits_5;

		public string Credits_6;

		public string Credits_7;

		public string Credits_8;

		public string Credits_9;

		public string Credits_10;

		public string Stats_0;

		public string Stats_1;

		public string Stats_2;

		public string Stats_3;

		public string Stats_4;

		public string Stats_5;

		public string Stats_6;

		public string Unlockable_0;

		public string Unlockable_1;

		public string Unlockable_2;

		public string Unlockable_3;

		public string Unlockable_4;

		public string Unlockable_5;

		public string Unlockable_6;

		public string Unlockable_7;

		public string Unlockable_8;

		public string CE_Custom_Tank_Music18;

		public string CE_Custom_Tank_Music19;

		public string CE_Custom_Tank_Music20;

		public string CE_Custom_Tank_Music21;

		public string CE_Custom_Tank_Music22;
	}

	private static LocalizationMaster _instance;

	public int CurrentLang;

	[Header("JSONs")]
	public TextAsset[] jsons;

	[Header("Locales")]
	public List<Locale> langs = new List<Locale>();

	public static LocalizationMaster instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		TextAsset[] array = jsons;
		foreach (TextAsset textAsset in array)
		{
			langs.Add(JsonUtility.FromJson<Locale>(textAsset.ToString()));
		}
	}

	public string GetText(string VarName)
	{
		return (string)langs[CurrentLang].GetType().GetField(VarName).GetValue(langs[CurrentLang]);
	}

	private void Update()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
