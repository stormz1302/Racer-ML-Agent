using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ALIyerEdon;

public class RacePositionManager : MonoBehaviour
{
    private Car_Position[] allCars;

    void Start()
    {
        allCars = FindObjectsOfType<Car_Position>();
    }

    void FixedUpdate()
    {
        // Sắp xếp theo totalPoints giảm dần (đi xa hơn đứng trước)
        var rankedCars = allCars.OrderByDescending(car => car.totalPoints).ToList();

        for (int i = 0; i < rankedCars.Count; i++)
        {
            WhatPosition whatPos = rankedCars[i].GetComponent<WhatPosition>();
            if (whatPos != null)
            {
                whatPos.pos = i; // Vị trí (0 là đầu)
            }
        }
    }
}
