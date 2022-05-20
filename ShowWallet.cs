using UnityEngine;

public class ShowWallet : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			TankeyTownMaster.instance.WalletAnimator.SetBool("ShowWallet", value: true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			TankeyTownMaster.instance.WalletAnimator.SetBool("ShowWallet", value: false);
		}
	}
}
