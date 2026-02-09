# Features Documentation

## 📋 Table of Contents
1. [Autonomous Driving System](#autonomous-driving-system)
2. [Steering Wheel Controller](#steering-wheel-controller)
3. [Collision Trigger System](#collision-trigger-system)
4. [Quiz Management System](#quiz-management-system)
5. [Waypoint Management](#waypoint-management)
6. [Timer System](#timer-system)

---

## Autonomous Driving System

### AutoDriveController

#### Key Features
- ✅ Waypoint-based autonomous pathfinding
- ✅ X-axis based reverse mode
- ✅ Obstacle detection and avoidance
- ✅ Smooth acceleration/deceleration
- ✅ Loop path support
- ✅ Waypoint wait time

#### Configuration Parameters

| Parameter | Type | Default | Description |
|---------|------|--------|-------------|
| `acceleration` | float | 10f | Acceleration rate |
| `deceleration` | float | 15f | Deceleration rate |
| `turnSpeed` | float | 5f | Turn speed |
| `stoppingDistance` | float | 2f | Stopping distance |
| `maxSpeed` | float | 20f | Maximum speed |
| `reverseSpeedMultiplier` | float | 0.8f | Reverse speed multiplier |
| `lookAheadDistance` | float | 5f | Obstacle detection distance |

#### Usage Example
```csharp
// Start autonomous driving
autoDriveController.StartAutoDrive();

// Stop autonomous driving
autoDriveController.StopAutoDrive();

// Automatically moves to next waypoint when reached
// Returns to first waypoint if in loop mode
```

#### Reverse Logic
```csharp
// X-axis position comparison
isReversing = targetPosition.x >= previousWaypointPosition.x;

// When reversing:
- Invert rotation direction
- Invert movement direction (vehicle rear)
- Apply speed multiplier
```

---

## Steering Wheel Controller

### SteeringWheelController

#### Key Features
- ✅ Real-time steering wheel rotation
- ✅ Rigidbody angular velocity-based calculation
- ✅ Transform delta-based calculation (alternative)
- ✅ Smooth animation
- ✅ Automatic return
- ✅ Deadzone application

#### Calculation Methods

**Method 1: Rigidbody Angular Velocity (Recommended)**
```csharp
float angularVelocityY = carRigidbody.angularVelocity.y;
float speedFactor = carRigidbody.linearVelocity.magnitude / 10f;
steeringInput = angularVelocityY * angularSensitivity * speedFactor;
```

**Method 2: Transform Delta**
```csharp
float angleDifference = Vector3.SignedAngle(lastForward, currentForward, Vector3.up);
float angularVelocity = angleDifference / Time.deltaTime;
steeringInput = (angularVelocity * angularSensitivity * speedFactor) / 100f;
```

#### Configuration Parameters

| Parameter | Type | Default | Description |
|---------|------|---------|-------------|
| `maxSteeringAngle` | float | 540f | Maximum steering angle (1.5 turns) |
| `steeringSpeed` | float | 5f | Steering rotation speed |
| `returnSpeed` | float | 3f | Steering return speed |
| `velocitySensitivity` | float | 1f | Velocity sensitivity |
| `angularSensitivity` | float | 2f | Angular velocity sensitivity |
| `deadZone` | float | 0.1f | Deadzone |

#### Usage Example
```csharp
// Manually set steering angle
steeringWheelController.SetSteeringAngle(180f);

// Reset steering to center
steeringWheelController.ResetSteering();

// Get current steering angle
float angle = steeringWheelController.GetCurrentSteeringAngle();

// Get normalized steering input (-1 ~ 1)
float input = steeringWheelController.GetSteeringInput();
```

---

## Collision Trigger System

### CollisionTriggerObject

#### Key Features
- ✅ Collision detection (Trigger & Collision)
- ✅ Tag-based filtering
- ✅ Particle effects
- ✅ Fade-out animation
- ✅ Sound playback
- ✅ UnityEvent-based extension
- ✅ Single/multiple trigger modes

#### Configuration Parameters

| Parameter | Type | Default | Description |
|---------|------|---------|-------------|
| `triggerName` | string | "ItemCollected" | Trigger name |
| `triggerOnce` | bool | true | Trigger only once |
| `fadeOut` | bool | true | Use fade-out |
| `fadeOutDuration` | float | 0.5f | Fade-out duration |
| `playSound` | bool | true | Play sound |
| `createParticles` | bool | true | Create particles |
| `validTags` | string[] | ["Player", "Car"] | Valid tags |

#### Events
- `onTriggerActivated`: When trigger occurs
- `onTriggerWithName`: Trigger with name

#### Usage Example
```csharp
// Manually trigger
collisionTrigger.ManualTrigger();

// Reset trigger (reuse)
collisionTrigger.ResetTrigger();

// Connect UnityEvent
collisionTrigger.onTriggerActivated.AddListener(() => {
    Debug.Log("Item collected!");
});
```

#### Effect System
1. **Particle Effects**
   - Prefab-based particle generation
   - Built-in ParticleSystem playback
   - Automatic cleanup (removed after duration)

2. **Fade-out**
   - Alpha value decrease
   - Scale decrease
   - Collider disable

3. **Sound**
   - Automatic AudioSource creation
   - Object destruction after playback completes

---

## Quiz Management System

### QuizManager

#### Key Features
- ✅ Sequential management of multiple quiz canvases
- ✅ Correct/incorrect answer handling
- ✅ Result display
- ✅ Vehicle movement animation integration
- ✅ Automatic next quiz transition

#### Configuration Parameters

| Parameter | Type | Description |
|---------|------|-------------|
| `questionCanvases` | GameObject[] | Quiz canvas array |
| `resultCanvas` | GameObject | Result canvas |
| `resultText` | Text | Result text |
| `targetObject` | Transform | Object to move |
| `moveOffset` | Vector3 | Movement offset |
| `moveDuration` | float | Movement duration |

#### Methods

**Correct Answer Handling**
```csharp
quizManager.OnCorrect("Correct!");
// Executes vehicle movement animation for first quiz
// Otherwise shows result then moves to next quiz
```

**Incorrect Answer Handling**
```csharp
quizManager.OnWrong("Incorrect!");
// Shows result then moves to next quiz
```

#### Operation Flow
```
Quiz 1 Activated
    ↓
User Answer
    ↓
Correct/Incorrect Handling
    ↓
Result Display (2 seconds)
    ↓
Quiz 2 Activated
    ↓
... (repeat)
```

---

## Waypoint Management

### WaypointManager

#### Key Features
- ✅ Waypoint list management
- ✅ Visual editing (Gizmos)
- ✅ Runtime addition/removal
- ✅ Speed and wait time settings
- ✅ Loop path support

#### Data Structure
```csharp
[Serializable]
public class Waypoint {
    public Vector3 position;  // Position
    public float speed;        // Target speed
    public float waitTime;     // Wait time (seconds)
}
```

#### Methods

**Add Waypoint**
```csharp
waypointManager.AddWaypoint(
    new Vector3(10, 0, 5),  // Position
    speed: 15f,              // Speed
    waitTime: 2f             // Wait time
);
```

**Remove Waypoint**
```csharp
waypointManager.RemoveWaypoint(0); // Remove index 0
```

#### Visualization
- **Waypoints**: Red spheres
- **Path**: Yellow lines
- **Numbers**: Scene view labels
- **Loop**: Connection between last and first

#### Configuration Parameters

| Parameter | Type | Default | Description |
|---------|------|---------|-------------|
| `waypoints` | List<Waypoint> | - | Waypoint list |
| `loopPath` | bool | true | Loop path |
| `waypointColor` | Color | Red | Waypoint color |
| `pathColor` | Color | Yellow | Path color |
| `waypointSize` | float | 1f | Waypoint size |

---

## Timer System

### Timer

#### Key Features
- ✅ Elapsed time measurement
- ✅ Delay countdown integration
- ✅ Time save and display

#### Configuration Parameters

| Parameter | Type | Description |
|---------|------|-------------|
| `time` | float | Elapsed time |
| `text` | Text | Display text |
| `DelayCount` | DelayTimeMain | Delay countdown |
| `endtext` | Text | End text |

#### Usage Example
```csharp
// Save time
timer.saveTime();

// Automatically starts time measurement when DelayCount reaches 0
// Automatically updates text in Update
```

### DelayTimeMain

#### Key Features
- ✅ Countdown timer
- ✅ Decreases every second

#### Configuration Parameters

| Parameter | Type | Default | Description |
|---------|------|---------|-------------|
| `DelayCount` | int | 3 | Start count |

#### Operation
- Decreases `DelayCount` every second
- Timer can start when it reaches 0
