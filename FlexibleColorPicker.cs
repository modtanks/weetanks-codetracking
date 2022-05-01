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
			Vector3 hsv = RGBToHSV(color);
			if (hsv.y == 0f || hsv.z == 0f)
			{
				this.bufferedHue = bufferedHue;
			}
			else
			{
				this.bufferedHue = hsv.x;
			}
			if (hsv.z == 0f)
			{
				this.bufferedSaturation = bufferedSaturation;
			}
			else
			{
				this.bufferedSaturation = hsv.y;
			}
		}

		public BufferedColor PickR(float value)
		{
			Color toReturn = color;
			toReturn.r = value;
			return new BufferedColor(toReturn, this);
		}

		public BufferedColor PickG(float value)
		{
			Color toReturn = color;
			toReturn.g = value;
			return new BufferedColor(toReturn, this);
		}

		public BufferedColor PickB(float value)
		{
			Color toReturn = color;
			toReturn.b = value;
			return new BufferedColor(toReturn, this);
		}

		public BufferedColor PickA(float value)
		{
			Color toReturn = color;
			toReturn.a = value;
			return new BufferedColor(toReturn, this);
		}

		public BufferedColor PickH(float value)
		{
			Vector3 hsv = RGBToHSV(color);
			Color toReturn = HSVToRGB(value, hsv.y, hsv.z);
			toReturn.a = color.a;
			return new BufferedColor(toReturn, value, bufferedSaturation);
		}

		public BufferedColor PickS(float value)
		{
			Color toReturn = HSVToRGB(v: RGBToHSV(color).z, h: bufferedHue, s: value);
			toReturn.a = color.a;
			return new BufferedColor(toReturn, bufferedHue, value);
		}

		public BufferedColor PickV(float value)
		{
			Color toReturn = HSVToRGB(bufferedHue, bufferedSaturation, value);
			toReturn.a = color.a;
			return new BufferedColor(toReturn, bufferedHue, bufferedSaturation);
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

	public bool staticMode = false;

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
		Vector2 v = GetNormalizedPointerPosition(canvas, focusedPicker.rectTransform, e);
		bufferedColor = PickColor(bufferedColor, focusedPickerType, v);
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
		foreach (Image im in array)
		{
			Material original = im.material;
			Material seperate = (im.material = new Material(original));
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
				Vector2 v = GetValue(type);
				UpdateMarker(image, type, v);
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
		case PickerType.Preview:
		case PickerType.PreviewAlpha:
			break;
		default:
		{
			bool horizontal = IsHorizontal(picker);
			SetMarker(picker, v, horizontal, !horizontal);
			break;
		}
		}
	}

	private void SetMarker(Image picker, Vector2 v, bool setX, bool setY)
	{
		RectTransform marker = null;
		RectTransform offMarker = null;
		if (setX && setY)
		{
			marker = GetMarker(picker, null);
		}
		else if (setX)
		{
			marker = GetMarker(picker, "hor");
			offMarker = GetMarker(picker, "ver");
		}
		else if (setY)
		{
			marker = GetMarker(picker, "ver");
			offMarker = GetMarker(picker, "hor");
		}
		if (offMarker != null)
		{
			offMarker.gameObject.SetActive(value: false);
		}
		if (!(marker == null))
		{
			marker.gameObject.SetActive(value: true);
			RectTransform parent = picker.rectTransform;
			Vector2 parentSize = parent.rect.size;
			Vector2 localPos = marker.localPosition;
			if (setX)
			{
				localPos.x = (v.x - parent.pivot.x) * parentSize.x;
			}
			if (setY)
			{
				localPos.y = (v.y - parent.pivot.y) * parentSize.y;
			}
			marker.localPosition = localPos;
		}
	}

	private RectTransform GetMarker(Image picker, string search)
	{
		for (int i = 0; i < picker.transform.childCount; i++)
		{
			RectTransform candidate = picker.transform.GetChild(i).GetComponent<RectTransform>();
			string candidateName = candidate.name.ToLower();
			bool match = candidateName.Contains("marker");
			if (match & (string.IsNullOrEmpty(search) || candidateName.Contains(search)))
			{
				return candidate;
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
			float value = GetValue1D(type);
			return new Vector2(value, value);
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
		bool skipPickers = !forceUpdate && staticMode;
		foreach (PickerType type in Enum.GetValues(typeof(PickerType)))
		{
			if (!skipPickers || IsPreviewType(type))
			{
				UpdateTexture(type, staticMode);
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
		Material i = image.materialForRendering;
		BufferedColor bc = bufferedColor;
		if (standardized)
		{
			switch (type)
			{
			case PickerType.S:
				bc = new BufferedColor(Color.red);
				break;
			default:
				bc = new BufferedColor(Color.black);
				break;
			case PickerType.Preview:
			case PickerType.PreviewAlpha:
				break;
			}
		}
		bool alpha = IsAlphaType(type);
		i.SetInt("_Mode", GetGradientMode(type));
		Color c1 = PickColor(bc, type, Vector2.zero).color;
		Color c2 = PickColor(bc, type, Vector2.one).color;
		if (!alpha)
		{
			c1 = new Color(c1.r, c1.g, c1.b);
			c2 = new Color(c2.r, c2.g, c2.b);
		}
		i.SetColor("_Color1", c1);
		i.SetColor("_Color2", c2);
		if (type == PickerType.Main)
		{
			i.SetInt("_DoubleMode", (int)mode);
		}
		if (standardized)
		{
			i.SetVector("_HSV", new Vector4(0f, 1f, 1f, 1f));
		}
		else
		{
			i.SetVector("_HSV", new Vector4(bc.h / 5.9999f, bc.s, bc.v, alpha ? bc.a : 1f));
		}
	}

	private int GetGradientMode(PickerType type)
	{
		int o = ((!IsHorizontal(pickerImages[(int)type])) ? 1 : 0);
		return type switch
		{
			PickerType.Main => 2, 
			PickerType.H => 3 + o, 
			_ => o, 
		};
	}

	private Image GetImage(int index)
	{
		if (index < 0 || index >= pickerImages.Length)
		{
			return null;
		}
		Image toReturn = pickerImages[index];
		if (!toReturn || !toReturn.gameObject.activeInHierarchy)
		{
			return null;
		}
		return toReturn;
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
			string newText = GetSanitizedHex(input, finish);
			string parseText = GetSanitizedHex(input, full: true);
			int cp = hexInput.caretPosition;
			hexInput.text = newText;
			if (hexInput.caretPosition == 0)
			{
				hexInput.caretPosition = 1;
			}
			else if (newText.Length == 2)
			{
				hexInput.caretPosition = 2;
			}
			else if (input.Length > newText.Length && cp < input.Length)
			{
				hexInput.caretPosition = cp - input.Length + newText.Length;
			}
			ColorUtility.TryParseHtmlString(parseText, out var newColor);
			bufferedColor.Set(newColor);
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
		List<string> options = new List<string>();
		foreach (MainPickingMode value in Enum.GetValues(typeof(MainPickingMode)))
		{
			options.Add(value.ToString());
		}
		modeDropdown.AddOptions(options);
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
		List<char> toReturn = new List<char>();
		toReturn.Add('#');
		int i = 0;
		char[] chars = input.ToCharArray();
		while (toReturn.Count < 7 && i < input.Length)
		{
			char nextChar = char.ToUpper(chars[i++]);
			bool validChar = char.IsNumber(nextChar);
			if (validChar || (nextChar >= 'A' && nextChar <= 'F'))
			{
				toReturn.Add(nextChar);
			}
		}
		while (full && toReturn.Count < 7)
		{
			toReturn.Insert(1, '0');
		}
		return new string(toReturn.ToArray());
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
		Vector2 screenPoint = ((PointerEventData)e).position;
		Vector2 localPos = rect.worldToLocalMatrix.MultiplyPoint(screenPoint);
		float x = Mathf.Clamp01(localPos.x / rect.rect.size.x + rect.pivot.x);
		float y = Mathf.Clamp01(localPos.y / rect.rect.size.y + rect.pivot.y);
		return new Vector2(x, y);
	}

	private static Vector2 GetNormWorldSpace(Canvas canvas, RectTransform rect, BaseEventData e)
	{
		Vector2 screenPoint = ((PointerEventData)e).position;
		Ray pointerRay = canvas.worldCamera.ScreenPointToRay(screenPoint);
		new Plane(canvas.transform.forward, canvas.transform.position).Raycast(pointerRay, out var enter);
		Vector3 worldPoint = pointerRay.origin + enter * pointerRay.direction;
		Vector2 localPoint = rect.worldToLocalMatrix.MultiplyPoint(worldPoint);
		float x = Mathf.Clamp01(localPoint.x / rect.rect.size.x + rect.pivot.x);
		float y = Mathf.Clamp01(localPoint.y / rect.rect.size.y + rect.pivot.y);
		return new Vector2(x, y);
	}

	public static Color HSVToRGB(Vector3 hsv)
	{
		return HSVToRGB(hsv.x, hsv.y, hsv.z);
	}

	public static Color HSVToRGB(float h, float s, float v)
	{
		float c = s * v;
		float i = v - c;
		float x = c * (1f - Mathf.Abs(h % 2f - 1f)) + i;
		c += i;
		return Mathf.FloorToInt(h % 6f) switch
		{
			0 => new Color(c, x, i), 
			1 => new Color(x, c, i), 
			2 => new Color(i, c, x), 
			3 => new Color(i, x, c), 
			4 => new Color(x, i, c), 
			5 => new Color(c, i, x), 
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
		float cMax = Mathf.Max(r, g, b);
		float cMin = Mathf.Min(r, g, b);
		float delta = cMax - cMin;
		float h = 0f;
		if (delta > 0f)
		{
			if (r >= b && r >= g)
			{
				h = Mathf.Repeat((g - b) / delta, 6f);
			}
			else if (g >= r && g >= b)
			{
				h = (b - r) / delta + 2f;
			}
			else if (b >= r && b >= g)
			{
				h = (r - g) / delta + 4f;
			}
		}
		float s = ((cMax == 0f) ? 0f : (delta / cMax));
		float v = cMax;
		return new Vector3(h, s, v);
	}
}
