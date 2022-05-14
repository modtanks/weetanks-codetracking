using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlexibleColorPicker : MonoBehaviour
{
	private enum PickerType
	{
		Main,
		R,
		G,
		B,
		H,
		S,
		V,
		A,
		Preview,
		PreviewAlpha
	}

	public enum MainPickingMode
	{
		HS,
		HV,
		SH,
		SV,
		VH,
		VS
	}

	[Serializable]
	private class BufferedColor
	{
		public Color color;

		private float bufferedHue;

		private float bufferedSaturation;

		public float r => color.r;

		public float g => color.g;

		public float b => color.b;

		public float a => color.a;

		public float h => bufferedHue;

		public float s => bufferedSaturation;

		public float v => RGBToHSV(color).z;

		public BufferedColor()
		{
			bufferedHue = 0f;
			bufferedSaturation = 0f;
			color = Color.black;
		}

		public BufferedColor(Color color)
			: this()
		{
			Set(color);
		}

		public BufferedColor(Color color, float hue, float sat)
			: this(color)
		{
			bufferedHue = hue;
			bufferedSaturation = sat;
		}

		public BufferedColor(Color color, BufferedColor source)
			: this(color, source.bufferedHue, source.bufferedSaturation)
		{
			Set(color);
		}

		public void Set(Color color)
		{
			Set(color, bufferedHue, bufferedSaturation);
		}

		public void Set(Color color, float bufferedHue, float bufferedSaturation)
		{
			this.color = color;
			Vector3 vector = RGBToHSV(color);
			if (vector.y == 0f || vector.z == 0f)
			{
				this.bufferedHue = bufferedHue;
			}
			else
			{
				this.bufferedHue = vector.x;
			}
			if (vector.z == 0f)
			{
				this.bufferedSaturation = bufferedSaturation;
			}
			else
			{
				this.bufferedSaturation = vector.y;
			}
		}

		public BufferedColor PickR(float value)
		{
			Color color = this.color;
			color.r = value;
			return new BufferedColor(color, this);
		}

		public BufferedColor PickG(float value)
		{
			Color color = this.color;
			color.g = value;
			return new BufferedColor(color, this);
		}

		public BufferedColor PickB(float value)
		{
			Color color = this.color;
			color.b = value;
			return new BufferedColor(color, this);
		}

		public BufferedColor PickA(float value)
		{
			Color color = this.color;
			color.a = value;
			return new BufferedColor(color, this);
		}

		public BufferedColor PickH(float value)
		{
			Vector3 vector = RGBToHSV(this.color);
			Color color = HSVToRGB(value, vector.y, vector.z);
			color.a = this.color.a;
			return new BufferedColor(color, value, bufferedSaturation);
		}

		public BufferedColor PickS(float value)
		{
			Color color = HSVToRGB(v: RGBToHSV(this.color).z, h: bufferedHue, s: value);
			color.a = this.color.a;
			return new BufferedColor(color, bufferedHue, value);
		}

		public BufferedColor PickV(float value)
		{
			Color color = HSVToRGB(bufferedHue, bufferedSaturation, value);
			color.a = this.color.a;
			return new BufferedColor(color, bufferedHue, bufferedSaturation);
		}
	}

	public Image[] pickerImages;

	public InputField hexInput;

	public Dropdown modeDropdown;

	private Canvas canvas;

	private BufferedColor bufferedColor;

	private Image focusedPicker;

	private PickerType focusedPickerType;

	private MainPickingMode lastUpdatedMode;

	private bool typeUpdate;

	private bool pickerTexturesStandardized;

	private bool materialsSeperated;

	public MainPickingMode mode;

	public Color startingColor = Color.white;

	private const float HUE_LOOP = 5.9999f;

	public bool staticMode;

	public bool multiInstance = true;

	public Color color
	{
		get
		{
			return bufferedColor.color;
		}
		set
		{
			if (bufferedColor != null)
			{
				bufferedColor.Set(value);
			}
			UpdateMarkers();
			UpdateTextures();
			UpdateHex();
			typeUpdate = true;
		}
	}

	private void Start()
	{
		bufferedColor = new BufferedColor(startingColor);
		canvas = GetComponentInParent<Canvas>();
	}

	private void OnEnable()
	{
		if (bufferedColor == null)
		{
			bufferedColor = new BufferedColor(startingColor);
		}
		if (multiInstance && !materialsSeperated)
		{
			SeperateMaterials();
			materialsSeperated = true;
		}
		pickerTexturesStandardized = staticMode;
		UpdateTextures(forceUpdate: true);
		MakeModeOptions();
		UpdateMarkers();
	}

	private void Update()
	{
		if (bufferedColor == null)
		{
			Debug.Log("NOO");
			bufferedColor = new BufferedColor(startingColor);
		}
		typeUpdate = false;
		if (lastUpdatedMode != mode)
		{
			ChangeMode(mode);
		}
		if (staticMode && !pickerTexturesStandardized)
		{
			UpdateTextures(forceUpdate: true);
			pickerTexturesStandardized = true;
		}
		else if (!staticMode && pickerTexturesStandardized)
		{
			UpdateTextures();
			pickerTexturesStandardized = false;
		}
		if (multiInstance && !materialsSeperated)
		{
			SeperateMaterials();
			materialsSeperated = true;
		}
	}

	public void SetPointerFocus(int i)
	{
		if (i < 0 || i >= pickerImages.Length)
		{
			PickerType pickerType = (PickerType)i;
			Debug.LogWarning("No picker image available of type " + pickerType.ToString() + ". Did you assign all the picker images in the editor?");
		}
		else
		{
			focusedPicker = pickerImages[i];
		}
		focusedPickerType = (PickerType)i;
	}

	public void PointerUpdate(BaseEventData e)
	{
		Vector2 normalizedPointerPosition = GetNormalizedPointerPosition(canvas, focusedPicker.rectTransform, e);
		bufferedColor = PickColor(bufferedColor, focusedPickerType, normalizedPointerPosition);
		UpdateMarkers();
		UpdateTextures();
		typeUpdate = true;
		UpdateHex();
	}

	public void TypeHex(string input)
	{
		TypeHex(input, finish: false);
		UpdateTextures();
		UpdateMarkers();
	}

	public void FinishTypeHex(string input)
	{
		TypeHex(input, finish: true);
		UpdateTextures();
		UpdateMarkers();
	}

	public void ChangeMode(int newMode)
	{
		ChangeMode((MainPickingMode)newMode);
	}

	public void ChangeMode(MainPickingMode mode)
	{
		this.mode = mode;
		pickerTexturesStandardized = false;
		UpdateTextures();
		UpdateMarkers();
		UpdateMode(mode);
	}

	private void SeperateMaterials()
	{
		Image[] array = pickerImages;
		foreach (Image obj in array)
		{
			Material material2 = (obj.material = new Material(obj.material));
		}
	}

	private BufferedColor PickColor(BufferedColor color, PickerType type, Vector2 v)
	{
		switch (type)
		{
		case PickerType.Main:
			return PickColorMain(color, v);
		case PickerType.Preview:
		case PickerType.PreviewAlpha:
			return color;
		default:
			return PickColor1D(color, type, v);
		}
	}

	private BufferedColor PickColorMain(BufferedColor color, Vector2 v)
	{
		return PickColorMain(color, mode, v);
	}

	private BufferedColor PickColor1D(BufferedColor color, PickerType type, Vector2 v)
	{
		float value = (IsHorizontal(pickerImages[(int)type]) ? v.x : v.y);
		return PickColor1D(color, type, value);
	}

	private BufferedColor PickColorMain(BufferedColor color, MainPickingMode mode, Vector2 v)
	{
		return mode switch
		{
			MainPickingMode.HS => PickColor2D(color, PickerType.H, v.x, PickerType.S, v.y), 
			MainPickingMode.HV => PickColor2D(color, PickerType.H, v.x, PickerType.V, v.y), 
			MainPickingMode.SH => PickColor2D(color, PickerType.S, v.x, PickerType.H, v.y), 
			MainPickingMode.SV => PickColor2D(color, PickerType.S, v.x, PickerType.V, v.y), 
			MainPickingMode.VH => PickColor2D(color, PickerType.V, v.x, PickerType.H, v.y), 
			MainPickingMode.VS => PickColor2D(color, PickerType.V, v.x, PickerType.S, v.y), 
			_ => bufferedColor, 
		};
	}

	private BufferedColor PickColor2D(BufferedColor color, PickerType type1, float value1, PickerType type2, float value2)
	{
		color = PickColor1D(color, type1, value1);
		color = PickColor1D(color, type2, value2);
		return color;
	}

	private BufferedColor PickColor1D(BufferedColor color, PickerType type, float value)
	{
		return type switch
		{
			PickerType.R => color.PickR(value), 
			PickerType.G => color.PickG(value), 
			PickerType.B => color.PickB(value), 
			PickerType.H => color.PickH(value * 5.9999f), 
			PickerType.S => color.PickS(value), 
			PickerType.V => color.PickV(value), 
			PickerType.A => color.PickA(value), 
			_ => throw new Exception("Picker type " + type.ToString() + " is not associated with a single color value."), 
		};
	}

	private BufferedColor GetDefaultColor(PickerType type)
	{
		return type switch
		{
			PickerType.Main => GetDefaultColor(mode), 
			PickerType.H => new BufferedColor(Color.red), 
			PickerType.S => new BufferedColor(Color.red), 
			PickerType.A => new BufferedColor(Color.white), 
			_ => new BufferedColor(), 
		};
	}

	private BufferedColor GetDefaultColor(MainPickingMode mode)
	{
		return new BufferedColor(Color.red);
	}

	private void UpdateMarkers()
	{
		for (int i = 0; i < pickerImages.Length; i++)
		{
			Image image = GetImage(i);
			if ((bool)image && image.isActiveAndEnabled)
			{
				PickerType type = (PickerType)i;
				Vector2 value = GetValue(type);
				UpdateMarker(image, type, value);
			}
		}
	}

	private void UpdateMarker(Image picker, PickerType type, Vector2 v)
	{
		switch (type)
		{
		case PickerType.Main:
			SetMarker(picker, v, setX: true, setY: true);
			break;
		default:
		{
			bool flag = IsHorizontal(picker);
			SetMarker(picker, v, flag, !flag);
			break;
		}
		case PickerType.Preview:
		case PickerType.PreviewAlpha:
			break;
		}
	}

	private void SetMarker(Image picker, Vector2 v, bool setX, bool setY)
	{
		RectTransform rectTransform = null;
		RectTransform rectTransform2 = null;
		if (setX && setY)
		{
			rectTransform = GetMarker(picker, null);
		}
		else if (setX)
		{
			rectTransform = GetMarker(picker, "hor");
			rectTransform2 = GetMarker(picker, "ver");
		}
		else if (setY)
		{
			rectTransform = GetMarker(picker, "ver");
			rectTransform2 = GetMarker(picker, "hor");
		}
		if (rectTransform2 != null)
		{
			rectTransform2.gameObject.SetActive(value: false);
		}
		if (!(rectTransform == null))
		{
			rectTransform.gameObject.SetActive(value: true);
			RectTransform rectTransform3 = picker.rectTransform;
			Vector2 size = rectTransform3.rect.size;
			Vector2 vector = rectTransform.localPosition;
			if (setX)
			{
				vector.x = (v.x - rectTransform3.pivot.x) * size.x;
			}
			if (setY)
			{
				vector.y = (v.y - rectTransform3.pivot.y) * size.y;
			}
			rectTransform.localPosition = vector;
		}
	}

	private RectTransform GetMarker(Image picker, string search)
	{
		for (int i = 0; i < picker.transform.childCount; i++)
		{
			RectTransform component = picker.transform.GetChild(i).GetComponent<RectTransform>();
			string text = component.name.ToLower();
			if (text.Contains("marker") & (string.IsNullOrEmpty(search) || text.Contains(search)))
			{
				return component;
			}
		}
		return null;
	}

	private Vector2 GetValue(PickerType type)
	{
		switch (type)
		{
		case PickerType.Main:
			return GetValue(mode);
		case PickerType.Preview:
		case PickerType.PreviewAlpha:
			return Vector2.zero;
		default:
		{
			float value1D = GetValue1D(type);
			return new Vector2(value1D, value1D);
		}
		}
	}

	private float GetValue1D(PickerType type)
	{
		return type switch
		{
			PickerType.R => bufferedColor.r, 
			PickerType.G => bufferedColor.g, 
			PickerType.B => bufferedColor.b, 
			PickerType.H => bufferedColor.h / 5.9999f, 
			PickerType.S => bufferedColor.s, 
			PickerType.V => bufferedColor.v, 
			PickerType.A => bufferedColor.a, 
			_ => throw new Exception("Picker type " + type.ToString() + " is not associated with a single color value."), 
		};
	}

	private Vector2 GetValue(MainPickingMode mode)
	{
		return mode switch
		{
			MainPickingMode.HS => new Vector2(GetValue1D(PickerType.H), GetValue1D(PickerType.S)), 
			MainPickingMode.HV => new Vector2(GetValue1D(PickerType.H), GetValue1D(PickerType.V)), 
			MainPickingMode.SH => new Vector2(GetValue1D(PickerType.S), GetValue1D(PickerType.H)), 
			MainPickingMode.SV => new Vector2(GetValue1D(PickerType.S), GetValue1D(PickerType.V)), 
			MainPickingMode.VH => new Vector2(GetValue1D(PickerType.V), GetValue1D(PickerType.H)), 
			MainPickingMode.VS => new Vector2(GetValue1D(PickerType.V), GetValue1D(PickerType.S)), 
			_ => throw new Exception("Unkown main picking mode: " + mode), 
		};
	}

	private void UpdateTextures(bool forceUpdate = false)
	{
		bool flag = !forceUpdate && staticMode;
		foreach (PickerType value in Enum.GetValues(typeof(PickerType)))
		{
			if (!flag || IsPreviewType(value))
			{
				UpdateTexture(value, staticMode);
			}
		}
	}

	private void UpdateTexture(PickerType type, bool standardized)
	{
		Image image = GetImage((int)type);
		if (!image || !image.gameObject.activeInHierarchy)
		{
			return;
		}
		Material materialForRendering = image.materialForRendering;
		BufferedColor bufferedColor = this.bufferedColor;
		if (standardized)
		{
			switch (type)
			{
			case PickerType.S:
				bufferedColor = new BufferedColor(Color.red);
				break;
			default:
				bufferedColor = new BufferedColor(Color.black);
				break;
			case PickerType.Preview:
			case PickerType.PreviewAlpha:
				break;
			}
		}
		bool flag = IsAlphaType(type);
		materialForRendering.SetInt("_Mode", GetGradientMode(type));
		Color value = PickColor(bufferedColor, type, Vector2.zero).color;
		Color value2 = PickColor(bufferedColor, type, Vector2.one).color;
		if (!flag)
		{
			value = new Color(value.r, value.g, value.b);
			value2 = new Color(value2.r, value2.g, value2.b);
		}
		materialForRendering.SetColor("_Color1", value);
		materialForRendering.SetColor("_Color2", value2);
		if (type == PickerType.Main)
		{
			materialForRendering.SetInt("_DoubleMode", (int)mode);
		}
		if (standardized)
		{
			materialForRendering.SetVector("_HSV", new Vector4(0f, 1f, 1f, 1f));
		}
		else
		{
			materialForRendering.SetVector("_HSV", new Vector4(bufferedColor.h / 5.9999f, bufferedColor.s, bufferedColor.v, flag ? bufferedColor.a : 1f));
		}
	}

	private int GetGradientMode(PickerType type)
	{
		int num = ((!IsHorizontal(pickerImages[(int)type])) ? 1 : 0);
		return type switch
		{
			PickerType.Main => 2, 
			PickerType.H => 3 + num, 
			_ => num, 
		};
	}

	private Image GetImage(int index)
	{
		if (index < 0 || index >= pickerImages.Length)
		{
			return null;
		}
		Image image = pickerImages[index];
		if (!image || !image.gameObject.activeInHierarchy)
		{
			return null;
		}
		return image;
	}

	private void UpdateHex()
	{
		if (!(hexInput == null) && hexInput.gameObject.activeInHierarchy)
		{
			hexInput.text = "#" + ColorUtility.ToHtmlStringRGB(color);
		}
	}

	private void TypeHex(string input, bool finish)
	{
		if (!typeUpdate)
		{
			typeUpdate = true;
			string sanitizedHex = GetSanitizedHex(input, finish);
			string sanitizedHex2 = GetSanitizedHex(input, full: true);
			int caretPosition = hexInput.caretPosition;
			hexInput.text = sanitizedHex;
			if (hexInput.caretPosition == 0)
			{
				hexInput.caretPosition = 1;
			}
			else if (sanitizedHex.Length == 2)
			{
				hexInput.caretPosition = 2;
			}
			else if (input.Length > sanitizedHex.Length && caretPosition < input.Length)
			{
				hexInput.caretPosition = caretPosition - input.Length + sanitizedHex.Length;
			}
			ColorUtility.TryParseHtmlString(sanitizedHex2, out var color);
			bufferedColor.Set(color);
			UpdateMarkers();
			UpdateTextures();
		}
	}

	private void MakeModeOptions()
	{
		if (modeDropdown == null || !modeDropdown.gameObject.activeInHierarchy)
		{
			return;
		}
		modeDropdown.ClearOptions();
		List<string> list = new List<string>();
		foreach (MainPickingMode value in Enum.GetValues(typeof(MainPickingMode)))
		{
			list.Add(value.ToString());
		}
		modeDropdown.AddOptions(list);
		UpdateMode(mode);
	}

	private void UpdateMode(MainPickingMode mode)
	{
		lastUpdatedMode = mode;
		if (!(modeDropdown == null) && modeDropdown.gameObject.activeInHierarchy)
		{
			modeDropdown.value = (int)mode;
		}
	}

	private static bool IsPreviewType(PickerType type)
	{
		return type switch
		{
			PickerType.Preview => true, 
			PickerType.PreviewAlpha => true, 
			_ => false, 
		};
	}

	private static bool IsAlphaType(PickerType type)
	{
		return type switch
		{
			PickerType.A => true, 
			PickerType.PreviewAlpha => true, 
			_ => false, 
		};
	}

	private static bool IsHorizontal(Image image)
	{
		Vector2 size = image.rectTransform.rect.size;
		return size.x >= size.y;
	}

	public static string GetSanitizedHex(string input, bool full)
	{
		if (string.IsNullOrEmpty(input))
		{
			return "#";
		}
		List<char> list = new List<char>();
		list.Add('#');
		int num = 0;
		char[] array = input.ToCharArray();
		while (list.Count < 7 && num < input.Length)
		{
			char c = char.ToUpper(array[num++]);
			if (char.IsNumber(c) || (c >= 'A' && c <= 'F'))
			{
				list.Add(c);
			}
		}
		while (full && list.Count < 7)
		{
			list.Insert(1, '0');
		}
		return new string(list.ToArray());
	}

	private static Vector2 GetNormalizedPointerPosition(Canvas canvas, RectTransform rect, BaseEventData e)
	{
		switch (canvas.renderMode)
		{
		case RenderMode.ScreenSpaceCamera:
			if (canvas.worldCamera == null)
			{
				return GetNormScreenSpace(rect, e);
			}
			return GetNormWorldSpace(canvas, rect, e);
		case RenderMode.ScreenSpaceOverlay:
			return GetNormScreenSpace(rect, e);
		case RenderMode.WorldSpace:
			if (canvas.worldCamera == null)
			{
				Debug.LogError("FCP in world space render mode requires an event camera to be set up on the parent canvas!");
				return Vector2.zero;
			}
			return GetNormWorldSpace(canvas, rect, e);
		default:
			return Vector2.zero;
		}
	}

	private static Vector2 GetNormScreenSpace(RectTransform rect, BaseEventData e)
	{
		Vector2 position = ((PointerEventData)e).position;
		Vector2 vector = rect.worldToLocalMatrix.MultiplyPoint(position);
		float x = Mathf.Clamp01(vector.x / rect.rect.size.x + rect.pivot.x);
		float y = Mathf.Clamp01(vector.y / rect.rect.size.y + rect.pivot.y);
		return new Vector2(x, y);
	}

	private static Vector2 GetNormWorldSpace(Canvas canvas, RectTransform rect, BaseEventData e)
	{
		Vector2 position = ((PointerEventData)e).position;
		Ray ray = canvas.worldCamera.ScreenPointToRay(position);
		new Plane(canvas.transform.forward, canvas.transform.position).Raycast(ray, out var enter);
		Vector3 point = ray.origin + enter * ray.direction;
		Vector2 vector = rect.worldToLocalMatrix.MultiplyPoint(point);
		float x = Mathf.Clamp01(vector.x / rect.rect.size.x + rect.pivot.x);
		float y = Mathf.Clamp01(vector.y / rect.rect.size.y + rect.pivot.y);
		return new Vector2(x, y);
	}

	public static Color HSVToRGB(Vector3 hsv)
	{
		return HSVToRGB(hsv.x, hsv.y, hsv.z);
	}

	public static Color HSVToRGB(float h, float s, float v)
	{
		float num = s * v;
		float num2 = v - num;
		float num3 = num * (1f - Mathf.Abs(h % 2f - 1f)) + num2;
		num += num2;
		return Mathf.FloorToInt(h % 6f) switch
		{
			0 => new Color(num, num3, num2), 
			1 => new Color(num3, num, num2), 
			2 => new Color(num2, num, num3), 
			3 => new Color(num2, num3, num), 
			4 => new Color(num3, num2, num), 
			5 => new Color(num, num2, num3), 
			_ => Color.black, 
		};
	}

	public static Vector3 RGBToHSV(Color color)
	{
		float r = color.r;
		float g = color.g;
		float b = color.b;
		return RGBToHSV(r, g, b);
	}

	public static Vector3 RGBToHSV(float r, float g, float b)
	{
		float num = Mathf.Max(r, g, b);
		float num2 = Mathf.Min(r, g, b);
		float num3 = num - num2;
		float x = 0f;
		if (num3 > 0f)
		{
			if (r >= b && r >= g)
			{
				x = Mathf.Repeat((g - b) / num3, 6f);
			}
			else if (g >= r && g >= b)
			{
				x = (b - r) / num3 + 2f;
			}
			else if (b >= r && b >= g)
			{
				x = (r - g) / num3 + 4f;
			}
		}
		float y = ((num == 0f) ? 0f : (num3 / num));
		float z = num;
		return new Vector3(x, y, z);
	}
}
