using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity;
using Unity.VisualScripting;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public CarBehaviour carBehaviour;
    int checkFinish = 0;
    //Rewards
    RewardStructure rewardStructure;
    private float hitCheck;
    private float hitWall;
    private float hitVehicle;
    TrackCheckpoints trackCheckpoints;
    private float checkpointTimer = 0f; // Bộ đếm thời gian
    //[SerializeField] private float maxTimeWithoutCheckpoint = 10f; // Thời gian tối đa (giây)

    private void Start()
    {
        carBehaviour = GetComponent<CarBehaviour>();
        //Debug.Log("seces");
        trackCheckpoints = FindObjectOfType<TrackCheckpoints>();
        //Rewards
        rewardStructure = GetComponent<RewardStructure>();
        hitCheck = rewardStructure.hit_check;
        hitWall = rewardStructure.hit_wall;
        hitVehicle = rewardStructure.hitVehicle;
    }

    private void Update()
    {
        checkpointTimer += Time.deltaTime; // 🔹 Cập nhật thời gian
        //if (checkpointTimer > maxTimeWithoutCheckpoint)
        //{
        //    // Nếu thời gian vượt quá giới hạn cho phép mà không đi qua checkpoint
        //    carBehaviour.AddReward(-0.01f); // Phạt điểm
        //    Debug.Log("Time out of checkpoint: " + checkpointTimer);
        //    checkpointTimer = 0f; // 🔹 Reset lại bộ đếm thời gian
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Checkpoint"))
        //{
        //    Debug.Log("Hit CheckPoint!");
        //    carBehaviour.AddReward(hitCheck);
        //    TrackCheckpoints trackCheckpoints = FindObjectOfType<TrackCheckpoints>();
        //    trackCheckpoints.AgentThroughCheckpoint(transform, other.transform);
        //    Debug.Log("Hit CheckPoint: " + hitCheck);
        //    checkpointTimer = 0f; // 🔹 Reset lại bộ đếm thời gian
        //}
        if (other.gameObject.CompareTag("Finish"))
        {
            if (checkpointTimer < 20f)
            {
                carBehaviour.AddReward(-100f);
            }
            else
            {
                carBehaviour.AddReward(100f);
            }
            carBehaviour.EndEpisode();
            checkpointTimer = 0f; // 🔹 Reset lại bộ đếm thời gian
            trackCheckpoints.ResetAgent(transform);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit Wall");
            carBehaviour.AddReward(hitWall);
            //carBehaviour.EndEpisode();
        }

        if (collision.gameObject.CompareTag("RaceCar"))
        {
            // Phạt điểm nếu va chạm với xe khác
            carBehaviour.AddReward(hitVehicle);
            Debug.Log("Va chạm với xe khác! Phạt điểm");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Stay Wall");
            carBehaviour.AddReward(-0.001f);
        }
    }

}
