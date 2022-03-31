using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[Serializable]
public class ThemeSettings : ScriptableObject
{
	[Serializable]
	private abstract class SelectableSettings_Base
	{
		[SerializeField]
		protected Selectable.Transition _transition;

		[SerializeField]
		protected CustomColorBlock _colors;

		[SerializeField]
		protected CustomSpriteState _spriteState;

		[SerializeField]
		protected CustomAnimationTriggers _animationTriggers;

		public Selectable.Transition transition => _transition;

		public CustomColorBlock selectableColors => _colors;

		public CustomSpriteState spriteState => _spriteState;

		public CustomAnimationTriggers animationTriggers => _animationTriggers;

		public virtual void Apply(Selectable item)
		{
			Selectable.Transition transition = _transition;
			bool transitionChanged = item.transition != transition;
			item.transition = transition;
			ICustomSelectable customSel = item as ICustomSelectable;
			switch (transition)
			{
			case Selectable.Transition.ColorTint:
			{
				CustomColorBlock cb = _colors;
				cb.fadeDuration = 0f;
				item.colors = cb;
				cb.fadeDuration = _colors.fadeDuration;
				item.colors = cb;
				if (customSel != null)
				{
					customSel.disabledHighlightedColor = cb.disabledHighlightedColor;
				}
				break;
			}
			case Selectable.Transition.SpriteSwap:
				item.spriteState = _spriteState;
				if (customSel != null)
				{
					customSel.disabledHighlightedSprite = _spriteState.disabledHighlightedSprite;
				}
				break;
			case Selectable.Transition.Animation:
				item.animationTriggers.disabledTrigger = _animationTriggers.disabledTrigger;
				item.animationTriggers.highlightedTrigger = _animationTriggers.highlightedTrigger;
				item.animationTriggers.normalTrigger = _animationTriggers.normalTrigger;
				item.animationTriggers.pressedTrigger = _animationTriggers.pressedTrigger;
				if (customSel != null)
				{
					customSel.disabledHighlightedTrigger = _animationTriggers.disabledHighlightedTrigger;
				}
				break;
			}
			if (transitionChanged)
			{
				item.targetGraphic.CrossFadeColor(item.targetGraphic.color, 0f, ignoreTimeScale: true, useAlpha: true);
			}
		}
	}

	[Serializable]
	private class SelectableSettings : SelectableSettings_Base
	{
		[SerializeField]
		private ImageSettings _imageSettings;

		public ImageSettings imageSettings => _imageSettings;

		public override void Apply(Selectable item)
		{
			if (!(item == null))
			{
				base.Apply(item);
				if (_imageSettings != null)
				{
					_imageSettings.CopyTo(item.targetGraphic as Image);
				}
			}
		}
	}

	[Serializable]
	private class SliderSettings : SelectableSettings_Base
	{
		[SerializeField]
		private ImageSettings _handleImageSettings;

		[SerializeField]
		private ImageSettings _fillImageSettings;

		[SerializeField]
		private ImageSettings _backgroundImageSettings;

		public ImageSettings handleImageSettings => _handleImageSettings;

		public ImageSettings fillImageSettings => _fillImageSettings;

		public ImageSettings backgroundImageSettings => _backgroundImageSettings;

		private void Apply(Slider item)
		{
			if (item == null)
			{
				return;
			}
			if (_handleImageSettings != null)
			{
				_handleImageSettings.CopyTo(item.targetGraphic as Image);
			}
			if (_fillImageSettings != null)
			{
				RectTransform rt = item.fillRect;
				if (rt != null)
				{
					_fillImageSettings.CopyTo(rt.GetComponent<Image>());
				}
			}
			if (_backgroundImageSettings != null)
			{
				Transform t = item.transform.Find("Background");
				if (t != null)
				{
					_backgroundImageSettings.CopyTo(t.GetComponent<Image>());
				}
			}
		}

		public override void Apply(Selectable item)
		{
			base.Apply(item);
			Apply(item as Slider);
		}
	}

	[Serializable]
	private class ScrollbarSettings : SelectableSettings_Base
	{
		[SerializeField]
		private ImageSettings _handleImageSettings;

		[SerializeField]
		private ImageSettings _backgroundImageSettings;

		public ImageSettings handle => _handleImageSettings;

		public ImageSettings background => _backgroundImageSettings;

		private void Apply(Scrollbar item)
		{
			if (!(item == null))
			{
				if (_handleImageSettings != null)
				{
					_handleImageSettings.CopyTo(item.targetGraphic as Image);
				}
				if (_backgroundImageSettings != null)
				{
					_backgroundImageSettings.CopyTo(item.GetComponent<Image>());
				}
			}
		}

		public override void Apply(Selectable item)
		{
			base.Apply(item);
			Apply(item as Scrollbar);
		}
	}

	[Serializable]
	private class ImageSettings
	{
		[SerializeField]
		private Color _color = Color.white;

		[SerializeField]
		private Sprite _sprite;

		[SerializeField]
		private Material _materal;

		[SerializeField]
		private Image.Type _type;

		[SerializeField]
		private bool _preserveAspect;

		[SerializeField]
		private bool _fillCenter;

		[SerializeField]
		private Image.FillMethod _fillMethod;

		[SerializeField]
		private float _fillAmout;

		[SerializeField]
		private bool _fillClockwise;

		[SerializeField]
		private int _fillOrigin;

		public Color color => _color;

		public Sprite sprite => _sprite;

		public Material materal => _materal;

		public Image.Type type => _type;

		public bool preserveAspect => _preserveAspect;

		public bool fillCenter => _fillCenter;

		public Image.FillMethod fillMethod => _fillMethod;

		public float fillAmout => _fillAmout;

		public bool fillClockwise => _fillClockwise;

		public int fillOrigin => _fillOrigin;

		public virtual void CopyTo(Image image)
		{
			if (!(image == null))
			{
				image.color = _color;
				image.sprite = _sprite;
				image.material = _materal;
				image.type = _type;
				image.preserveAspect = _preserveAspect;
				image.fillCenter = _fillCenter;
				image.fillMethod = _fillMethod;
				image.fillAmount = _fillAmout;
				image.fillClockwise = _fillClockwise;
				image.fillOrigin = _fillOrigin;
			}
		}
	}

	[Serializable]
	private struct CustomColorBlock
	{
		[SerializeField]
		private float m_ColorMultiplier;

		[SerializeField]
		private Color m_DisabledColor;

		[SerializeField]
		private float m_FadeDuration;

		[SerializeField]
		private Color m_HighlightedColor;

		[SerializeField]
		private Color m_NormalColor;

		[SerializeField]
		private Color m_PressedColor;

		[SerializeField]
		private Color m_SelectedColor;

		[SerializeField]
		private Color m_DisabledHighlightedColor;

		public float colorMultiplier
		{
			get
			{
				return m_ColorMultiplier;
			}
			set
			{
				m_ColorMultiplier = value;
			}
		}

		public Color disabledColor
		{
			get
			{
				return m_DisabledColor;
			}
			set
			{
				m_DisabledColor = value;
			}
		}

		public float fadeDuration
		{
			get
			{
				return m_FadeDuration;
			}
			set
			{
				m_FadeDuration = value;
			}
		}

		public Color highlightedColor
		{
			get
			{
				return m_HighlightedColor;
			}
			set
			{
				m_HighlightedColor = value;
			}
		}

		public Color normalColor
		{
			get
			{
				return m_NormalColor;
			}
			set
			{
				m_NormalColor = value;
			}
		}

		public Color pressedColor
		{
			get
			{
				return m_PressedColor;
			}
			set
			{
				m_PressedColor = value;
			}
		}

		public Color selectedColor
		{
			get
			{
				return m_SelectedColor;
			}
			set
			{
				m_SelectedColor = value;
			}
		}

		public Color disabledHighlightedColor
		{
			get
			{
				return m_DisabledHighlightedColor;
			}
			set
			{
				m_DisabledHighlightedColor = value;
			}
		}

		public static implicit operator ColorBlock(CustomColorBlock item)
		{
			ColorBlock result = default(ColorBlock);
			result.selectedColor = item.m_SelectedColor;
			result.colorMultiplier = item.m_ColorMultiplier;
			result.disabledColor = item.m_DisabledColor;
			result.fadeDuration = item.m_FadeDuration;
			result.highlightedColor = item.m_HighlightedColor;
			result.normalColor = item.m_NormalColor;
			result.pressedColor = item.m_PressedColor;
			return result;
		}
	}

	[Serializable]
	private struct CustomSpriteState
	{
		[SerializeField]
		private Sprite m_DisabledSprite;

		[SerializeField]
		private Sprite m_HighlightedSprite;

		[SerializeField]
		private Sprite m_PressedSprite;

		[SerializeField]
		private Sprite m_SelectedSprite;

		[SerializeField]
		private Sprite m_DisabledHighlightedSprite;

		public Sprite disabledSprite
		{
			get
			{
				return m_DisabledSprite;
			}
			set
			{
				m_DisabledSprite = value;
			}
		}

		public Sprite highlightedSprite
		{
			get
			{
				return m_HighlightedSprite;
			}
			set
			{
				m_HighlightedSprite = value;
			}
		}

		public Sprite pressedSprite
		{
			get
			{
				return m_PressedSprite;
			}
			set
			{
				m_PressedSprite = value;
			}
		}

		public Sprite selectedSprite
		{
			get
			{
				return m_SelectedSprite;
			}
			set
			{
				m_SelectedSprite = value;
			}
		}

		public Sprite disabledHighlightedSprite
		{
			get
			{
				return m_DisabledHighlightedSprite;
			}
			set
			{
				m_DisabledHighlightedSprite = value;
			}
		}

		public static implicit operator SpriteState(CustomSpriteState item)
		{
			SpriteState result = default(SpriteState);
			result.selectedSprite = item.m_SelectedSprite;
			result.disabledSprite = item.m_DisabledSprite;
			result.highlightedSprite = item.m_HighlightedSprite;
			result.pressedSprite = item.m_PressedSprite;
			return result;
		}
	}

	[Serializable]
	private class CustomAnimationTriggers
	{
		[SerializeField]
		private string m_DisabledTrigger;

		[SerializeField]
		private string m_HighlightedTrigger;

		[SerializeField]
		private string m_NormalTrigger;

		[SerializeField]
		private string m_PressedTrigger;

		[SerializeField]
		private string m_SelectedTrigger;

		[SerializeField]
		private string m_DisabledHighlightedTrigger;

		public string disabledTrigger
		{
			get
			{
				return m_DisabledTrigger;
			}
			set
			{
				m_DisabledTrigger = value;
			}
		}

		public string highlightedTrigger
		{
			get
			{
				return m_HighlightedTrigger;
			}
			set
			{
				m_HighlightedTrigger = value;
			}
		}

		public string normalTrigger
		{
			get
			{
				return m_NormalTrigger;
			}
			set
			{
				m_NormalTrigger = value;
			}
		}

		public string pressedTrigger
		{
			get
			{
				return m_PressedTrigger;
			}
			set
			{
				m_PressedTrigger = value;
			}
		}

		public string selectedTrigger
		{
			get
			{
				return m_SelectedTrigger;
			}
			set
			{
				m_SelectedTrigger = value;
			}
		}

		public string disabledHighlightedTrigger
		{
			get
			{
				return m_DisabledHighlightedTrigger;
			}
			set
			{
				m_DisabledHighlightedTrigger = value;
			}
		}

		public CustomAnimationTriggers()
		{
			m_DisabledTrigger = string.Empty;
			m_HighlightedTrigger = string.Empty;
			m_NormalTrigger = string.Empty;
			m_PressedTrigger = string.Empty;
			m_SelectedTrigger = string.Empty;
			m_DisabledHighlightedTrigger = string.Empty;
		}

		public static implicit operator AnimationTriggers(CustomAnimationTriggers item)
		{
			return new AnimationTriggers
			{
				selectedTrigger = item.m_SelectedTrigger,
				disabledTrigger = item.m_DisabledTrigger,
				highlightedTrigger = item.m_HighlightedTrigger,
				normalTrigger = item.m_NormalTrigger,
				pressedTrigger = item.m_PressedTrigger
			};
		}
	}

	[Serializable]
	private class TextSettings
	{
		[SerializeField]
		private Color _color = Color.white;

		[SerializeField]
		private TMP_FontAsset _font;

		[SerializeField]
		private FontStyleOverride _style = FontStyleOverride.Default;

		[SerializeField]
		private float _sizeMultiplier = 1f;

		[SerializeField]
		private float _lineSpacing = 1f;

		[SerializeField]
		private float _characterSpacing = 1f;

		[SerializeField]
		private float _wordSpacing = 1f;

		public Color color => _color;

		public TMP_FontAsset font => _font;

		public FontStyleOverride style => _style;

		public float sizeMultiplier => _sizeMultiplier;

		public float lineSpacing => _lineSpacing;

		public float chracterSpacing => _characterSpacing;

		public float wordSpacing => _wordSpacing;
	}

	private enum FontStyleOverride
	{
		Default,
		Normal,
		Bold,
		Italic,
		BoldAndItalic
	}

	[SerializeField]
	private ImageSettings _mainWindowBackground;

	[SerializeField]
	private ImageSettings _popupWindowBackground;

	[SerializeField]
	private ImageSettings _areaBackground;

	[SerializeField]
	private SelectableSettings _selectableSettings;

	[SerializeField]
	private SelectableSettings _buttonSettings;

	[SerializeField]
	private SelectableSettings _inputGridFieldSettings;

	[SerializeField]
	private ScrollbarSettings _scrollbarSettings;

	[SerializeField]
	private SliderSettings _sliderSettings;

	[SerializeField]
	private ImageSettings _invertToggle;

	[SerializeField]
	private Color _invertToggleDisabledColor;

	[SerializeField]
	private ImageSettings _calibrationBackground;

	[SerializeField]
	private ImageSettings _calibrationValueMarker;

	[SerializeField]
	private ImageSettings _calibrationRawValueMarker;

	[SerializeField]
	private ImageSettings _calibrationZeroMarker;

	[SerializeField]
	private ImageSettings _calibrationCalibratedZeroMarker;

	[SerializeField]
	private ImageSettings _calibrationDeadzone;

	[SerializeField]
	private TextSettings _textSettings;

	[SerializeField]
	private TextSettings _buttonTextSettings;

	[SerializeField]
	private TextSettings _inputGridFieldTextSettings;

	public void Apply(ThemedElement.ElementInfo[] elementInfo)
	{
		if (elementInfo == null)
		{
			return;
		}
		for (int i = 0; i < elementInfo.Length; i++)
		{
			if (elementInfo[i] != null)
			{
				Apply(elementInfo[i].themeClass, elementInfo[i].component);
			}
		}
	}

	private void Apply(string themeClass, Component component)
	{
		if (component as Selectable != null)
		{
			Apply(themeClass, (Selectable)component);
		}
		else if (component as Image != null)
		{
			Apply(themeClass, (Image)component);
		}
		else if (component as TMP_Text != null)
		{
			Apply(themeClass, (TMP_Text)component);
		}
		else if (component as UIImageHelper != null)
		{
			Apply(themeClass, (UIImageHelper)component);
		}
	}

	private void Apply(string themeClass, Selectable item)
	{
		if (!(item == null))
		{
			SelectableSettings_Base settings;
			if (item as Button != null)
			{
				settings = ((!(themeClass == "inputGridField")) ? _buttonSettings : _inputGridFieldSettings);
			}
			else if (item as Scrollbar != null)
			{
				settings = _scrollbarSettings;
			}
			else if (item as Slider != null)
			{
				settings = _sliderSettings;
			}
			else if (item as Toggle != null)
			{
				settings = ((!(themeClass == "button")) ? _selectableSettings : _buttonSettings);
			}
			else
			{
				settings = _selectableSettings;
			}
			settings.Apply(item);
		}
	}

	private void Apply(string themeClass, Image item)
	{
		if (item == null)
		{
			return;
		}
		switch (themeClass)
		{
		case "area":
			if (_areaBackground != null)
			{
				_areaBackground.CopyTo(item);
			}
			break;
		case "popupWindow":
			if (_popupWindowBackground != null)
			{
				_popupWindowBackground.CopyTo(item);
			}
			break;
		case "mainWindow":
			if (_mainWindowBackground != null)
			{
				_mainWindowBackground.CopyTo(item);
			}
			break;
		case "calibrationValueMarker":
			if (_calibrationValueMarker != null)
			{
				_calibrationValueMarker.CopyTo(item);
			}
			break;
		case "calibrationRawValueMarker":
			if (_calibrationRawValueMarker != null)
			{
				_calibrationRawValueMarker.CopyTo(item);
			}
			break;
		case "calibrationBackground":
			if (_calibrationBackground != null)
			{
				_calibrationBackground.CopyTo(item);
			}
			break;
		case "calibrationZeroMarker":
			if (_calibrationZeroMarker != null)
			{
				_calibrationZeroMarker.CopyTo(item);
			}
			break;
		case "calibrationCalibratedZeroMarker":
			if (_calibrationCalibratedZeroMarker != null)
			{
				_calibrationCalibratedZeroMarker.CopyTo(item);
			}
			break;
		case "calibrationDeadzone":
			if (_calibrationDeadzone != null)
			{
				_calibrationDeadzone.CopyTo(item);
			}
			break;
		case "invertToggle":
			if (_invertToggle != null)
			{
				_invertToggle.CopyTo(item);
			}
			break;
		case "invertToggleBackground":
			if (_inputGridFieldSettings != null)
			{
				_inputGridFieldSettings.imageSettings.CopyTo(item);
			}
			break;
		case "invertToggleButtonBackground":
			if (_buttonSettings != null)
			{
				_buttonSettings.imageSettings.CopyTo(item);
			}
			break;
		}
	}

	private void Apply(string themeClass, TMP_Text item)
	{
		if (!(item == null))
		{
			TextSettings settings = ((themeClass == "button") ? _buttonTextSettings : ((!(themeClass == "inputGridField")) ? _textSettings : _inputGridFieldTextSettings));
			if (settings.font != null)
			{
				item.font = settings.font;
			}
			item.color = settings.color;
			item.lineSpacing = settings.lineSpacing;
			if (settings.sizeMultiplier != 1f)
			{
				item.fontSize = (int)(item.fontSize * settings.sizeMultiplier);
				item.fontSizeMax = (int)(item.fontSizeMax * settings.sizeMultiplier);
				item.fontSizeMin = (int)(item.fontSizeMin * settings.sizeMultiplier);
			}
			item.characterSpacing = settings.chracterSpacing;
			item.wordSpacing = settings.wordSpacing;
			if (settings.style != 0)
			{
				item.fontStyle = GetFontStyle(settings.style);
			}
		}
	}

	private void Apply(string themeClass, UIImageHelper item)
	{
		if (!(item == null))
		{
			item.SetEnabledStateColor(_invertToggle.color);
			item.SetDisabledStateColor(_invertToggleDisabledColor);
			item.Refresh();
		}
	}

	private static FontStyles GetFontStyle(FontStyleOverride style)
	{
		switch (style)
		{
		case FontStyleOverride.Bold:
			return FontStyles.Bold;
		case FontStyleOverride.BoldAndItalic:
			return FontStyles.Bold | FontStyles.Italic;
		case FontStyleOverride.Italic:
			return FontStyles.Italic;
		case FontStyleOverride.Default:
		case FontStyleOverride.Normal:
			return FontStyles.Normal;
		default:
			throw new NotImplementedException();
		}
	}
}
