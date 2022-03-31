using System.Collections;
using UnityEngine;

public class MaterialFade : MonoBehaviour
{
	[Range(0.01f, 1f)]
	public float fadeSpeed = 1f;

	public Color fadeColor = new Color(0f, 0f, 0f, 0f);

	public float startAlpha = 0.5f;

	public Material m_Material;

	public Color m_Color;

	public bool isMaterial = false;

	public bool IsShield = false;

	private void Start()
	{
		if (isMaterial)
		{
			m_Material = GetComponent<Renderer>().material;
		}
		else
		{
			m_Material = GetComponent<SpriteRenderer>().material;
		}
		if (!GameMaster.instance.Sun[0].activeSelf && !isMaterial)
		{
			Debug.LogWarning("Dark level so dark texture");
		}
		m_Color = m_Material.color;
		m_Color.a = 0.82f;
		StartCoroutine(AlphaFade());
	}

	public void StartFade()
	{
		m_Color = m_Material.color;
		m_Color.a = 0.82f;
		StartCoroutine(AlphaFade());
	}

	private IEnumerator AlphaFade()
	{
		float alpha = startAlpha;
		while (alpha > 0f)
		{
			m_Color.a = 0.82f;
			alpha -= fadeSpeed * Time.deltaTime;
			if (isMaterial)
			{
				m_Material.SetColor("_BaseColor", new Color(m_Color.r, m_Color.g, m_Color.b, alpha));
				m_Material.SetColor("_EmissiveColor", new Color(m_Color.r, m_Color.g, m_Color.b, alpha));
			}
			else
			{
				m_Material.color = new Color(m_Color.r, m_Color.g, m_Color.b, alpha);
			}
			if (alpha < 0.03f)
			{
				if (!IsShield)
				{
					Object.Destroy(base.transform.parent.gameObject);
				}
				break;
			}
			yield return null;
		}
	}
}
