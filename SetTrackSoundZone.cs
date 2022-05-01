using UnityEngine;

public class SetTrackSoundZone : MonoBehaviour
{
	public enum TrackSound
	{
		Grass,
		Wood,
		Carpet
	}

	public TrackSound Sound;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			MoveTankScript component = other.GetComponent<MoveTankScript>();
			if ((bool)component && Sound == TrackSound.Grass)
			{
				component.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
		else if (other.tag == "Enemy")
		{
			NewAIagent component2 = other.GetComponent<NewAIagent>();
			if ((bool)component2 && Sound == TrackSound.Grass)
			{
				component2.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			MoveTankScript component = other.GetComponent<MoveTankScript>();
			if ((bool)component && Sound == TrackSound.Grass && component.TankTracks != GameMaster.instance.TankTracksGrassSound)
			{
				component.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
		else if (other.tag == "Enemy")
		{
			NewAIagent component2 = other.GetComponent<NewAIagent>();
			if ((bool)component2 && Sound == TrackSound.Grass && component2.TankTracks != GameMaster.instance.TankTracksGrassSound)
			{
				component2.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			MoveTankScript component = other.GetComponent<MoveTankScript>();
			if ((bool)component && Sound == TrackSound.Grass)
			{
				component.TankTracks = GameMaster.instance.TankTracksNormalSound;
			}
		}
		else if (other.tag == "Enemy")
		{
			NewAIagent component2 = other.GetComponent<NewAIagent>();
			if ((bool)component2 && Sound == TrackSound.Grass)
			{
				component2.TankTracks = GameMaster.instance.TankTracksNormalSound;
			}
		}
	}
}
