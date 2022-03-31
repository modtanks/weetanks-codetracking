using System.Collections;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
	[Range(0.01f, 1f)]
	public float fadeSpeed = 1f;

	public int ShieldHealth = 0;

	public Material m_Material;

	public Color m_Color;

	public float startAlpha;

	public float GoToAlpha;

	private float OriginalGoToAlpha;

	public AudioClip HittedSound;

	private SphereCollider SC;

	public bool isPeachShield = false;

	private int prevHealth = 0;

	private bool GettingHit = false;

	private Color prevEmission;

	private void Start()
	{
		OriginalGoToAlpha = GoToAlpha;
		GetComponent<MeshRenderer>().enabled = true;
		m_Material = GetComponent<MeshRenderer>().material;
		prevEmission = m_Material.GetColor("_EmissionColor");
		SC = GetComponent<SphereCollider>();
		m_Color = m_Material.color;
		startAlpha = m_Color.a;
		prevHealth = ShieldHealth;
		InvokeRepeating("ReduceShield", 4f, 4f);
	}

	private void ReduceShield()
	{
		if (prevHealth == ShieldHealth && ShieldHealth > 0)
		{
			ShieldHealth--;
		}
		prevHealth = ShieldHealth;
	}

	public void StartFade()
	{
		Debug.Log("HIT!");
		GettingHit = true;
		m_Material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
		m_Material.SetColor("_EmissionColor", new Color(1f, 1f, 1f, 1f) * 1.5f);
		StartCoroutine(ResetColor());
	}

	private IEnumerator ResetColor()
	{
		yield return new WaitForSeconds(0.03f);
		m_Material.SetColor("_EmissionColor", new Color(1f, 1f, 1f, 1f) * 1.25f);
		yield return new WaitForSeconds(0.03f);
		m_Material.SetColor("_EmissionColor", new Color(1f, 1f, 1f, 1f) * 1f);
		yield return new WaitForSeconds(0.03f);
		GettingHit = false;
		m_Material.SetColor("_EmissionColor", prevEmission);
	}

	private void Update()
	{
		if (!GettingHit)
		{
			if (!GameMaster.instance.GameHasStarted)
			{
				ShieldHealth = 0;
			}
			if (ShieldHealth > 0)
			{
				SC.enabled = true;
				m_Material.SetColor("_Color", new Color(m_Color.r, m_Color.g, m_Color.b, (float)ShieldHealth / 10f));
			}
			else
			{
				SC.enabled = false;
				m_Material.SetColor("_Color", new Color(m_Color.r, m_Color.g, m_Color.b, 0f));
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (ShieldHealth > 0 && other.tag == "Bullet")
		{
			PlayerBulletScript PBS = other.GetComponent<PlayerBulletScript>();
			if ((bool)PBS && (!(PBS.EnemyTankScript != null) || PBS.TimesBounced != 0 || !(PBS.EnemyTankScript.AIscript == base.transform.parent.GetComponent<EnemyAI>())) && (!(PBS.TankScript != null) || PBS.TimesBounced != 0 || !(PBS.TankScript.tankMovingScript == base.transform.parent.GetComponent<MoveTankScript>())) && (!(PBS.EnemyTankScript != null) || PBS.TimesBounced != 0 || !(PBS.EnemyTankScript.AIscript == base.transform.parent.GetComponent<EnemyAI>())))
			{
				PBS.TimesBounced = 99999;
				StartFade();
				SFXManager.instance.PlaySFX(HittedSound, 1f, null);
				ShieldHealth--;
			}
		}
	}
}
