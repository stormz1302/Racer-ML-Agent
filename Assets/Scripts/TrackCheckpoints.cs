using System;
using System.Collections.Generic;
using UnityEngine;
using ALIyerEdon;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler<CarCheckpointEventArgs> OnCarWrongCheckpoint; // Sự kiện khi đi sai checkpoint
    public event EventHandler<CarCheckpointEventArgs> OnCarCorrectCheckpoint; // Sự kiện khi đi đúng checkpoint

    public class CarCheckpointEventArgs : EventArgs
    {
        public Transform carTransform;
    }

    [SerializeField] private List<Transform> checkpoints; // Danh sách các checkpoint theo thứ tự
    private Checkpoint_Manager checkpointManager; // Quản lý các checkpoint
    private Dictionary<Transform, int> carCheckpointIndex; // Lưu checkpoint hiện tại của mỗi agent
    

    private void Start()
    {
        checkpointManager = GetComponent<Checkpoint_Manager>();
        carCheckpointIndex = new Dictionary<Transform, int>();
        checkpoints = checkpointManager.checkpoints;
    }

    public void AgentThroughCheckpoint(Transform carTransform, Transform checkpointTransform)
    {
        if (!carCheckpointIndex.ContainsKey(carTransform))
        {
            carCheckpointIndex[carTransform] = 0; // Khởi tạo agent ở checkpoint đầu tiên
        }

        int currentIndex = carCheckpointIndex[carTransform];

        if (checkpoints[currentIndex] == checkpointTransform) // Kiểm tra đúng checkpoint
        {

            carCheckpointIndex[carTransform] = (currentIndex + 1) % checkpoints.Count; // Chuyển sang checkpoint tiếp theo
            OnCarCorrectCheckpoint?.Invoke(this, new CarCheckpointEventArgs { carTransform = carTransform });
        }
        else // Nếu agent đi sai checkpoint
        {
            //carCheckpointIndex[carTransform] = (currentIndex - 1) % checkpoints.Count;
            OnCarWrongCheckpoint?.Invoke(this, new CarCheckpointEventArgs { carTransform = carTransform });
        }
    }

    public Transform GetNextCheckpoint(Transform carTransform)
    {
        if (!carCheckpointIndex.ContainsKey(carTransform))
        {
            return checkpoints[0]; // Nếu chưa có dữ liệu, trả về checkpoint đầu tiên
        }

        int nextIndex = carCheckpointIndex[carTransform];
        return checkpoints[nextIndex];
    }

    public void ResetAgent(Transform carTransform)
    {
        if (carCheckpointIndex.ContainsKey(carTransform))
        {
            carCheckpointIndex[carTransform] = 0; // Đưa agent về checkpoint đầu tiên khi reset
        }
    }
}
