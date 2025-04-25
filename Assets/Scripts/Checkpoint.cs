using UnityEngine;


public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarBehaviour>(out CarBehaviour agent))
        {
            TrackCheckpoints trackCheckpoints = FindObjectOfType<TrackCheckpoints>();
            trackCheckpoints.AgentThroughCheckpoint(agent.transform, transform);
        }
    }
}