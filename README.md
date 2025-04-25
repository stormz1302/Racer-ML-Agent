# Racer-ML-Agent
This project uses Unity's ML-Agents to create an AI-driven racing agent that learns to navigate a racing track. The agent learns to optimize its actions based on rewards and penalties, with the goal of completing laps faster and more efficiently.

Project Overview

The agent is a car that can navigate through a track, pass through checkpoints, and complete laps. It is trained using reinforcement learning where it gets rewards for passing checkpoints, following the correct path, and completing laps in the shortest time. The car uses a custom-built controller, which allows for acceleration, braking, and steering.

Features

Car Behaviour: The car learns to navigate the track by receiving feedback in the form of rewards and penalties based on its actions.

Checkpoint System: The track is divided into checkpoints, which the car must pass in sequence. The system keeps track of the car’s progress and ensures it follows the correct path.

Lap Completion: The agent learns to complete laps and can pass to the next lap when it completes a circuit.

AI Training with ML-Agents: The car is trained using Unity’s ML-Agents to improve its performance in the racing environment.

Key Components

1. CarBehaviour
This script controls the movement of the car and interacts with the environment.

The car receives a reward based on its position (e.g., a reward for being in the lead).

Actions include acceleration, turning, and braking.

2. CarPosition
This component tracks the car’s position in the race.

It monitors the current lap, checkpoint, and distance to the next checkpoint.

The car’s progress is updated every frame and passed to the race manager.

3. Checkpoint System
The track is divided into checkpoints.

The car must pass through these checkpoints in order, which is checked in the Checkpoint_Trigger script.

The car receives rewards for successfully navigating through checkpoints and completing laps.

4. AI Training with ML-Agents
The car is trained using reinforcement learning with the Unity ML-Agents framework.

The agent learns to maximize rewards by choosing actions that lead to faster lap times and fewer collisions.

The reward function is designed to encourage the car to stay on course, pass checkpoints in the correct order, and finish the race.

#Scripts Overview

WhatPosition

This script assigns a position to the car based on its lap and checkpoint progress.

It listens for collisions with checkpoints and updates the car’s position accordingly.

CarBehaviour

This script handles the car's movement and behavior.

It receives control inputs, applies them to the car, and calculates the reward for each action.

CarPosition

Tracks the car's position, including the current lap, checkpoint, and distance to the next checkpoint.

Checkpoint_Trigger

Detects when the car passes through a checkpoint and updates the car’s checkpoint progress.
