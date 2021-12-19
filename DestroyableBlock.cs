using UnityEngine;

public class DestroyableBlock : MonoBehaviour
{
	public int blockHealth = 2;

	public int maxBlockHealth = 5;

	private Material mymat;

	public GameObject BrokenParticlePrefab;

	public bool destroyed;

	public Texture2D[] WoodTextures;

	private bool Resetted;

	private bool Resetted2;

	private void Start()
	{
		blockHealth = maxBlockHealth;
		MeshRenderer component = GetComponent<MeshRenderer>();
		mymat = component.material;
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
				GameObject obj = Object.Instantiate(BrokenParticlePrefab, base.transform.position + new Vector3(0f, 0.8f, 0f), Quaternion.identity);
				obj.transform.Rotate(new Vector3(-90f, 0f, base.transform.eulerAngles.z));
				obj.transform.parent = null;
				obj.GetComponent<ParticleSystem>();
				obj.GetComponentInChildren<ParticleSystem>();
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
