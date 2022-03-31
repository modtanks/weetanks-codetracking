using UnityEngine;

public class SetTrackSoundZone : MonoBehaviour
{
	public enum TrackSound
	{
		Grass,
		Wood,
		Carpet
	}

	public TrackSound Sound = TrackSound.Grass;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			MoveTankScript MTS = other.GetComponent<MoveTankScript>();
			if ((bool)MTS && Sound == TrackSound.Grass)
			{
				MTS.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
		else if (other.tag == "Enemy")
		{
			NewAIagent NAI = other.GetComponent<NewAIagent>();
			if ((bool)NAI && Sound == TrackSound.Grass)
			{
				NAI.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			MoveTankScript MTS = other.GetComponent<MoveTankScript>();
			if ((bool)MTS && Sound == TrackSound.Grass && MTS.TankTracks != GameMaster.instance.TankTracksGrassSound)
			{
				MTS.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
		else if (other.tag == "Enemy")
		{
			NewAIagent NAI = other.GetComponent<NewAIagent>();
			if ((bool)NAI && Sound == TrackSound.Grass && NAI.TankTracks != GameMaster.instance.TankTracksGrassSound)
			{
				NAI.TankTracks = GameMaster.instance.TankTracksGrassSound;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			MoveTankScript MTS = other.GetComponent<MoveTankScript>();
			if ((bool)MTS && Sound == TrackSound.Grass)
			{
				MTS.TankTracks = GameMaster.instance.TankTracksNormalSound;
			}
		}
		else if (other.tag == "Enemy")
		{
			NewAIagent NAI = other.GetComponent<NewAIagent>();
			if ((bool)NAI && Sound == TrackSound.Grass)
			{
				NAI.TankTracks = GameMaster.instance.TankTracksNormalSound;
			}
		}
	}
}
