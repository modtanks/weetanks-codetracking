using System.Collections;
using UnityEngine;

public class DeathCrossScript : MonoBehaviour
{
	public Texture2D BlueCross;

	public Texture2D RedCross;

	public Texture2D PurpleCross;

	public Texture2D GreenCross;

	private Renderer m_Renderer;

	private bool startfade;

	private Color alphaColor;

	private void Start()
	{
		m_Renderer = GetComponent<Renderer>();
		startfade = false;
		StartCoroutine(startFade());
		alphaColor = m_Renderer.material.color;
		alphaColor.a = 0f;
	}

	private IEnumerator startFade()
	{
		yield return new WaitForSeconds(20f);
		startfade = true;
	}

	public void IsRed()
	{
		m_Renderer = GetComponent<Renderer>();
		m_Renderer.material.SetTexture("_MainTex", RedCross);
	}

	public void IsBlue()
	{
		m_Renderer = GetComponent<Renderer>();
		m_Renderer.material.SetTexture("_MainTex", BlueCross);
	}

	public void IsGreen()
	{
		m_Renderer = GetComponent<Renderer>();
		m_Renderer.material.SetTexture("_MainTex", GreenCross);
	}

	public void IsPurple()
	{
		m_Renderer = GetComponent<Renderer>();
		m_Renderer.material.SetTexture("_MainTex", PurpleCross);
	}

	private void Update()
	{
		if (startfade)
		{
			m_Renderer.material.color = Color.Lerp(m_Renderer.material.color, alphaColor, Time.deltaTime / 10f);
			if (m_Renderer.material.color.a < 0.1f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
