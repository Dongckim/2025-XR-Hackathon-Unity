# Architecture Documentation

## System Overview

Three-Osity adopts a modular architecture where each system operates independently while being organically connected to each other.

## Core Systems

### 1. Vehicle Control System (Car Control System)

#### AutoDriveController
**Responsibility**: Waypoint-based autonomous driving control

**Key Components**:
- `WaypointManager`: Path data management
- `Rigidbody`: Physics-based movement
- Obstacle detection raycast

**Algorithm**:
```csharp
1. Calculate distance to current waypoint
2. Determine reverse condition by comparing X-axis positions
3. Rotate and move toward target direction
4. Switch to next waypoint upon reaching target point
```

**Reverse Logic**:
- X-axis based: `targetPosition.x >= previousWaypointPosition.x` → reverse
- Apply speed multiplier when reversing (`reverseSpeedMultiplier`)
- Invert rotation direction

#### SteeringWheelController
**Responsibility**: Steering wheel rotation based on vehicle movement

**Calculation Methods**:
1. **Rigidbody Angular Velocity Based** (Recommended)
   ```csharp
   steeringInput = angularVelocityY * angularSensitivity * speedFactor
   ```

2. **Transform Delta Based** (Alternative)
   ```csharp
   angleDifference = Vector3.SignedAngle(lastForward, currentForward, Vector3.up)
   angularVelocity = angleDifference / Time.deltaTime
   ```

**Animation**:
- Lerp-based smooth rotation
- Separate return speed and rotation speed
- Apply deadzone to ignore minor movements

### 2. Collision and Quiz System

#### CollisionTriggerObject
**Responsibility**: Collision detection and quiz triggering

**Event Flow**:
```
Collision Detection → Tag Validation → Trigger Execution → Quiz Activation → Effect Playback → Object Removal
```

**Effect System**:
- Particle effects (prefab or built-in)
- Fade-out animation (coroutine)
- Sound playback (AudioSource)

#### QuizManager
**Responsibility**: Sequential management of multiple quizzes

**State Machine**:
```
Quiz 1 → Correct/Incorrect → Result Display → Quiz 2 → ... → Complete
```

**Special Features**:
- Vehicle movement animation on first quiz correct answer
- 2-second delay before switching to next quiz
- Dynamic result text update

### 3. Waypoint Management System

#### WaypointManager
**Responsibility**: Path data management and visualization

**Data Structure**:
```csharp
[Serializable]
public class Waypoint {
    public Vector3 position;  // Position
    public float speed;        // Target speed
    public float waitTime;     // Wait time
}
```

**Visualization**:
- Scene view display using Gizmos
- Waypoint number labels
- Path line connections
- Loop path display

## Data Flow

### Autonomous Driving Flow
```
WaypointManager (Path Data)
    ↓
AutoDriveController (Driving Control)
    ↓
Rigidbody (Physics Movement)
    ↓
SteeringWheelController (Steering Wheel Rotation)
```

### Quiz Trigger Flow
```
Vehicle Movement → CollisionTriggerObject (Collision Detection)
    ↓
QuizManager (Quiz Activation)
    ↓
User Input → ButtonEvent (Correct/Incorrect Answer)
    ↓
QuizManager (Result Processing and Next Quiz)
```

## Physics System

### Vehicle Physics
- **WheelCollider**: 4 wheels (2 front, 2 rear)
- **Motor Torque**: Rear-wheel drive
- **Steering Angle**: Front-wheel steering
- **Brake Torque**: 4-wheel brake
- **Downforce**: Speed-based downward force

### Collision Detection
- **LayerMask**: Obstacle layer filtering
- **Raycast**: Forward/rear obstacle detection
- **Collider**: Trigger and collision detection

## Performance Optimization

### Optimization Techniques
1. **Object Pooling**: Reuse particle effects
2. **Minimize Update**: Process physics in FixedUpdate
3. **Conditional Gizmos**: Visualize only in editor
4. **Coroutine Usage**: Handle asynchronous tasks

### Memory Management
- Immediately remove unnecessary GameObjects
- Destroy objects after sound playback completes
- Automatic particle system cleanup

## Extensibility

### Adding New Features
1. **New Vehicle Type**: Inherit from `AutoDriveController`
2. **New Quiz Type**: Extend `QuizManager`
3. **New Trigger**: Inherit from `CollisionTriggerObject`

### Configurability
- All parameters adjustable in Inspector
- Connect custom events using UnityEvent
- Flexible collision handling with tag-based filtering

## Debugging Tools

### Gizmos Visualization
- Waypoint positions and paths
- Obstacle detection rays
- Current velocity vector
- Reverse status display

### Debug Logs
- Waypoint reached notifications
- Collision event logs
- Speed and angle information
- Quiz state changes

## Security Considerations

### Input Validation
- Tag-based collision filtering
- Waypoint index range checking
- Null reference prevention

### Error Handling
- Warning when components are missing
- Safe handling when waypoints are absent
- Error log when Rigidbody is missing
