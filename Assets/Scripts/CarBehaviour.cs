using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using UnityEditor;
using System.Reflection;
using EVP;
using ALIyerEdon;
using UnityEngine.AI;
using EdyCommonTools;

public class CarBehaviour : Agent
{
    public Rigidbody rBody;
    public EasyCarController m_input;
    public Nitro nitro;

    //public CarController controller;
    public GameObject spawn;

    private bool isBreaking;
    private bool nitroState;

    private float maxTime = 20.0f, currentTime;

    private float speed;
    private float timeSpeedZero;
    float hitCheck;
    //Rewards
    RewardStructure rewardStructure;
    private float timePass;
    private float speedCoeff;

    private NavMeshPath navPath;
    private int currentTargetIndex = 0;
    public float targetReachThreshold = 4f;

    //RayPerceptionSensors
    public RayPerceptionSensorComponent3D wall;
    public RayPerceptionSensorComponent3D checkpoint;
    public RayPerceptionSensorComponent3D vehicleSensor;

    public TrackCheckpoints trackCheckpoints;
    public Transform[] targetCheckpoints;

    public float minSpeed = 30f;

    private int wrongCheckpointCount = 0; // Biến đếm số lần đi sai checkpoint
    private const int maxWrongCheckpoints = 10; // Giới hạn sai checkpoint tối đa
    private bool isStopped = false; // Biến kiểm tra xem xe đã dừng chưa
    private LineRenderer lineRenderer;
    private Vector3 lastValidLocalDir = Vector3.zero;

    float alignmentTimer;

    void Awake()
    {
        // Gán hoặc tạo LineRenderer nếu chưa có
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
        }

        // Khởi tạo NavMeshPath
    }

    private void Start()
    {
        //trackCheckpoints = FindObjectOfType<TrackCheckpoints>();
        hitCheck = rewardStructure.hit_check;
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    private void Update()
    {
        if (transform.position.y < -20)
        {
            EndEpisode();
        }
    }

    private void FixedUpdate()
    {
        UpdateCurrentTargetIndex();
        Transform target = targetCheckpoints[currentTargetIndex];

        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, navPath))
        {
            lineRenderer.positionCount = navPath.corners.Length;
            lineRenderer.SetPositions(navPath.corners);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            
            Debug.Log("Wrong Checkpoint: " + hitCheck);
            
            AddReward(-hitCheck);
        }
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            wrongCheckpointCount = 0; // Reset lại biến đếm khi đi đúng checkpoint
            AddReward(hitCheck);
            Debug.Log("Correct Checkpoint: " + hitCheck);
        }
    }


    public override void Initialize()
    {
        //Debug.Log("Init");
        //controller = GetComponent<CarController>();
        m_input = GetComponent<EasyCarController>();
        rBody = GetComponent<Rigidbody>();
        nitro = GetComponent<Nitro>();
        navPath = new NavMeshPath();

        //Rewards
        rewardStructure = GetComponent<RewardStructure>();
        timePass = rewardStructure.time_pass;
        speedCoeff = rewardStructure.speed_coeff;
    }

    public override void OnEpisodeBegin()
    {
        ResetEnv();
        currentTime = 0.0f;
        currentTargetIndex = 0;
        //Debug.Log("OnEpis");
        trackCheckpoints.ResetAgent(transform);

    }

    private void UpdateCurrentTargetIndex()
    {
        if (targetCheckpoints == null || targetCheckpoints.Length == 0) return;

        Transform currentTarget = targetCheckpoints[currentTargetIndex];

        // Tính path từ vị trí hiện tại tới checkpoint
        if (NavMesh.CalculatePath(transform.position, currentTarget.position, NavMesh.AllAreas, navPath))
        {
            // Nếu path có ít hơn 2 điểm, coi như đã tới nơi
            if (navPath.corners.Length <= 2)
            {
                currentTargetIndex = (currentTargetIndex + 1) % targetCheckpoints.Length;
            }
            //else
            //{
            //    // Kiểm tra nếu agent đã gần corner tiếp theo trong path
            //    float distToNextCorner = Vector3.Distance(transform.position, navPath.corners[1]);
            //    if (distToNextCorner < targetReachThreshold)
            //    {
            //        currentTargetIndex = (currentTargetIndex + 1) % targetCheckpoints.Length;
            //    }
            //}
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        Transform currentTarget = targetCheckpoints[currentTargetIndex];

        bool hasValidPath = NavMesh.CalculatePath(transform.position, currentTarget.position, NavMesh.AllAreas, navPath);

        if (hasValidPath && navPath.corners.Length > 1)
        {
            Vector3 nextWaypoint = navPath.corners[1];
            lastValidLocalDir = transform.InverseTransformDirection((nextWaypoint - transform.position).normalized);
        }

        // Dù path có valid hay không, vẫn dùng hướng cuối cùng đã biết
        sensor.AddObservation(lastValidLocalDir);
        
        // Add speed of the agent as an observation
        Vector3 vel = rBody.velocity;
        speed = vel.magnitude;
        //Debug.Log((int)speed);
        sensor.AddObservation(speed);
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log("Action");
        MoveAgent(actions);
        //Debug.Log("Speed: " + speed);
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= maxTime)
        {
            //AddReward(-100f);
            //EndEpisode();
            //trackCheckpoints.ResetAgent(transform);
        }


        if (speed <= 1f)
        {
            isStopped = true;
            timeSpeedZero += Time.fixedDeltaTime;
            AddReward(-0.1f); // Phạt nhẹ khi dừng lại
            if (timeSpeedZero >= 2 && timeSpeedZero < 15f)
            {
                AddReward(-speedCoeff * timeSpeedZero);
            }
            else if (timeSpeedZero >= 15f)
            {
                Debug.Log("Speed Zero");
                AddReward(-100f);
                EndEpisode();
                trackCheckpoints.ResetAgent(transform);
                timeSpeedZero = 0f;
            }
        }
        else
        {
            if (isStopped)
            {
                //StartCoroutine(CheckSpeed());
                Debug.Log("Move againt!");
            }
            isStopped = false;
            timeSpeedZero = 0f;
            AddReward(speedCoeff * speed);
        }

        // Reward theo hướng về waypoint
        if (navPath != null && navPath.corners.Length > 1)
        {
            Vector3 dirToWaypoint = (navPath.corners[1] - transform.position).normalized;
            float alignment = Vector3.Dot(transform.forward, dirToWaypoint); // -1 đến 1
            if (alignment < 0f)
            {
                alignmentTimer += Time.fixedDeltaTime;
                if (alignmentTimer >= 15f)
                {
                    AddReward(-100f);
                    EndEpisode();
                }
            }
            else
            {
                // Reset timer nếu quay lại đúng hướng
                alignmentTimer = 0f;
            }

            float directionReward = alignment - 0.6f; // Điều chỉnh hệ số này để tăng độ nhạy
            AddReward(directionReward * 0.05f); // Có thể điều chỉnh hệ số này
        }
      
        AddReward(timePass);
        //AddReward(speed * speedCoeff);
        float penalty = speed - minSpeed;
        if ( speed < 50f)
        {
            AddReward(speedCoeff * penalty);
        }
        else
        {
            AddReward(speedCoeff*2f*penalty);
            EndEpisode();
            trackCheckpoints.ResetAgent(transform);
        }
    }

    IEnumerator CheckSpeed()
    {
        yield return new WaitForSeconds(3f);
        if (speed > 5f)
        {
            AddReward(100f);
        }
        else
        {
            AddReward(-100f);
            EndEpisode();
            trackCheckpoints.ResetAgent(transform);
        }
    }
    

    public void MoveAgent(ActionBuffers actions)
    {
        //Debug.Log("MoveAgents");
        float throttle = actions.ContinuousActions[0];
        float steer = actions.ContinuousActions[1];
        //float brake = actions.ContinuousActions[2];

        float mappedThrottle = Mathf.Clamp(throttle, -1f, 1f);
        float mappedSteer = Mathf.Clamp(steer, -1f, 1f);
        //float braking = Mathf.Clamp(brake, 0f, 1f);

        float braking = actions.DiscreteActions[0];
        float nitring = actions.DiscreteActions[1];

        if (braking == 1)
        {
            isBreaking = true;
        }
        else
        {
            isBreaking = false;
        }

        if (nitring == 1)
        {
            nitroState = true;
        }
        else
        {
            nitroState = false;
        }
        GetInput(mappedThrottle, isBreaking, mappedSteer, nitroState);
        AddReward((mappedThrottle) * 0.05f); 
    }

    public void GetInput(float throttle, bool brake, float steering, bool nitroState)
    {
        m_input.Move(throttle, steering, brake);
        if (nitroState)
        {
            nitro.Apply_Nitro();
        }
        else
        {
            nitro.Release_Nitro();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;

        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        //continuousActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[0] = 1;
        }
        else
        {
            discreteActionsOut[0] = 0;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            discreteActionsOut[1] = 1;
        }
        else
        {
            discreteActionsOut[1] = 0;
        }
    }

    public void ResetEnv()
    {
        this.transform.position = spawn.transform.position;
        this.transform.rotation = spawn.transform.rotation;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }
    
    /*
    private void OnGUI()
    {
        float labelWidth = 100f;
        float labelHeight = 20f;

        Rect labelRect = new Rect((Screen.width - labelWidth) / 2, Screen.height - labelHeight - 10, labelWidth, labelHeight);

        using (var layout = new GUILayout.AreaScope(labelRect))
        {
            GUILayout.Label("Speed: " + (int)(speed * 2.23694) + " mph");
        }
    }*/
}