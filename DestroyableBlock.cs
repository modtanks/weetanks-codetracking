using UnityEngine;

public class DestroyableBlock : MonoBehaviour
{
	public int blockHealth = 2;

	public int maxBlockHealth = 5;

	private Material mymat;

	public GameObject BrokenParticlePrefab;

	public bool destroyed = false;

	public Texture2D[] WoodTextures;

	private bool Resetted = false;

	private bool Resetted2 = false;

	private void Start()
	{
		blockHealth = maxBlockHealth;
		MeshRenderer myrend = GetComponent<MeshRenderer>();
		mymat = myrend.material;
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted && GameMaster.instance.restartGame && !GetComponent<BoxCollider>().enabled)
		{
			GetComponent<BoxCollider>().enabled = true;
			GetComponent<MeshRenderer>().enabled = true;
			destroyed = false;
			blockHealth = maxBlockHealth;
			Resetted = false;
			Resetted2 = true;
		}
		if (GameMaster.instance.GameHasStarted && !Resetted2)
		{
			Resetted2 = true;
			GetComponent<BoxCollider>().enabled = true;
		}
		if (GameMaster.instance.GameHasStarted && !Resetted)
		{
			Resetted = true;
			Resetted2 = false;
			GetComponent<BoxCollider>().enabled = false;
		}
		mymat.SetTexture("_MainTex", WoodTextures[maxBlockHealth - blockHealth]);
		if (blockHealth < 1 && !destroyed)
		{
			destroyed = true;
			if (BrokenParticlePrefab != null)
			{
				GameObject particles = Object.Instantiate(BrokenParticlePrefab, base.transform.position + new Vector3(0f, 0.8f, 0f), Quaternion.identity);
				particles.transform.Rotate(new Vector3(-90f, 0f, base.transform.eulerAngles.z));
				particles.transform.parent = null;
				ParticleSystem OneSystem = particles.GetComponent<ParticleSystem>();
				ParticleSystem SmokeSystem = particles.GetComponentInChildren<ParticleSystem>();
			}
			GetComponent<BoxCollider>().enabled = false;
			GetComponent<MeshRenderer>().enabled = false;
		}
		else if (blockHealth > 0 && GameMaster.instance.GameHasStarted && Resetted2 && !GetComponent<BoxCollider>().enabled)
		{
			GetComponent<BoxCollider>().enabled = true;
			GetComponent<MeshRenderer>().enabled = true;
			destroyed = false;
		}
	}
}
