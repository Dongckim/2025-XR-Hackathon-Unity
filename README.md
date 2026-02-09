# Three-Osity: AR/VR Educational Driving Experience

<div align="center">

![Unity Version](https://img.shields.io/badge/Unity-6000.0.45f1-blue.svg)
![XR Interaction Toolkit](https://img.shields.io/badge/XR%20Interaction%20Toolkit-3.0.8-green.svg)
![License](https://img.shields.io/badge/License-MIT-yellow.svg)

**An immersive AR/VR educational experience combining autonomous vehicle navigation, interactive quizzes, and realistic physics simulation**

[Features](#-main-features) • [Architecture](#️-architecture) • [Getting Started](#-getting-started) • [Documentation](#-documentation)

</div>

---

## 📖 Project Overview

**Three-Osity** is a Unity-based AR/VR educational simulation project. Users drive a car through a virtual Tokyo street, collect items along the path, and solve quizzes in an interactive experience.

### Core Features
- 🚗 **Autonomous Driving System**: Waypoint-based automatic vehicle control
- 🎮 **VR/AR Support**: Immersive experience using XR Interaction Toolkit
- 🎯 **Interactive Quizzes**: Collision-based quiz trigger system
- 🎨 **Physics-based Simulation**: Realistic vehicle physics using WheelCollider
- 🎪 **Visual Feedback**: Particle effects, fade-out animations, sound effects

---

## ✨ Main Features

### 1. Autonomous Driving System (AutoDriveController)
- Waypoint-based pathfinding
- X-axis based reverse mode support
- Obstacle detection and avoidance
- Smooth acceleration/deceleration control
- Loop path support

### 2. Steering Wheel Controller (SteeringWheelController)
- Real-time steering wheel rotation based on vehicle movement
- Rigidbody angular velocity-based calculation
- Smooth animation and return effects
- Maximum 540-degree rotation support (1.5 turns)

### 3. Collision Trigger System (CollisionTriggerObject)
- Automatic quiz activation on vehicle collision
- Particle effects and sound playback
- Fade-out animation
- UnityEvent-based extensible event system

### 4. Quiz Management System (QuizManager)
- Sequential management of multiple quiz canvases
- Correct/incorrect answer feedback handling
- Vehicle movement animation integration
- Timer integration

### 5. Waypoint Management (WaypointManager)
- Visual waypoint editing
- Runtime waypoint addition/removal
- Path visualization (Gizmos)
- Speed and wait time settings

---

## 🏗️ Architecture

### Project Structure
```
Assets/
├── Scripts/
│   ├── dongchan/          # Vehicle control system
│   │   ├── AutoDriveController.cs      # Autonomous driving controller
│   │   ├── SteeringWheelController.cs  # Steering wheel rotation controller
│   │   ├── VRSimpleCarController.cs    # VR vehicle controller
│   │   ├── CollisionTriggerObject.cs   # Collision trigger object
│   │   ├── WaypointManager.cs          # Waypoint manager
│   │   └── TestScript.cs               # Test script
│   │
│   └── heeyeon/           # UI and quiz system
│       ├── QuizManager.cs              # Quiz manager
│       ├── Timer.cs                    # Timer system
│       ├── ButtonEvent.cs              # Button event handling
│       └── DelayTimeMain.cs            # Delay countdown
│
├── Scenes/                 # Unity scene files
├── Tokyo_Street/           # Tokyo street environment assets
├── Simple Car Controller/  # Vehicle controller assets
└── Samples/                # XR Interaction Toolkit samples
```

### System Diagram
```
┌─────────────────────────────────────────────────┐
│           Unity AR/VR Application               │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌──────────────┐      ┌──────────────┐        │
│  │   VR/AR      │      │   Physics    │        │
│  │  Interaction │◄────►│   Engine     │        │
│  └──────────────┘      └──────────────┘        │
│         │                       │                │
│         ▼                       ▼                │
│  ┌──────────────────────────────────────┐       │
│  │      Car Control System              │       │
│  │  ┌────────────┐  ┌──────────────┐   │       │
│  │  │ AutoDrive  │  │ SteeringWheel│   │       │
│  │  │ Controller │◄─┤  Controller  │   │       │
│  │  └────────────┘  └──────────────┘   │       │
│  │         │                │           │       │
│  │         └────────┬───────┘           │       │
│  │                  ▼                    │       │
│  │         ┌──────────────┐             │       │
│  │         │ WaypointMgr  │             │       │
│  │         └──────────────┘             │       │
│  └──────────────────────────────────────┘       │
│         │                                        │
│         ▼                                        │
│  ┌──────────────────────────────────────┐       │
│  │    Collision & Quiz System           │       │
│  │  ┌──────────────┐  ┌──────────────┐ │       │
│  │  │  Collision   │─►│    Quiz      │ │       │
│  │  │   Trigger    │  │   Manager    │ │       │
│  │  └──────────────┘  └──────────────┘ │       │
│  └──────────────────────────────────────┘       │
│                                                  │
└──────────────────────────────────────────────────┘
```

---

## 🚀 Getting Started

### Prerequisites
- **Unity Editor**: 6000.0.45f1 or higher
- **XR Interaction Toolkit**: 3.0.8
- **Platform**: Android/iOS (AR), VR Headsets (VR)

### Installation

1. **Clone the project**
   ```bash
   git clone [repository-url]
   cd three-osity
   ```

2. **Open project in Unity**
   - Add project in Unity Hub
   - Select Unity version 6000.0.45f1

3. **Verify dependencies**
   - Check if XR Interaction Toolkit is automatically installed
   - Install from Package Manager if needed

4. **Run scene**
   - Open `Assets/Day_Demo.unity` or `Assets/Scenes/testing2.unity`
   - Click Play button

### Basic Usage

#### Start Autonomous Driving
```csharp
// After connecting WaypointManager to AutoDriveController
autoDriveController.StartAutoDrive();
```

#### Add Waypoint
```csharp
waypointManager.AddWaypoint(new Vector3(10, 0, 5), speed: 15f, waitTime: 2f);
```

#### Setup Collision Trigger
- Add `CollisionTriggerObject` component to object
- Assign `firstQuiz` GameObject
- Add "Car" or "Player" to `validTags`

---

## 📚 Key Scripts Documentation

### AutoDriveController.cs
Autonomous driving system with X-axis based reverse functionality. Moves along waypoints and detects/avoids obstacles.

**Key Methods:**
- `StartAutoDrive()`: Start autonomous driving
- `StopAutoDrive()`: Stop autonomous driving
- `CheckReverseCondition()`: Check X-axis based reverse condition

### SteeringWheelController.cs
System that rotates steering wheel based on actual vehicle movement. Calculates in real-time based on Rigidbody angular velocity.

**Key Features:**
- Rigidbody angular velocity-based calculation
- Transform delta-based calculation (alternative method)
- Smooth rotation animation
- Automatic return functionality

### CollisionTriggerObject.cs
Trigger system that activates quizzes and provides visual effects when vehicle collides.

**Key Features:**
- Collision detection and tag filtering
- Particle effect generation
- Fade-out animation
- UnityEvent-based extension

### QuizManager.cs
System that sequentially manages multiple quiz canvases and handles correct/incorrect answers.

**Key Methods:**
- `OnCorrect(string message)`: Handle correct answer
- `OnWrong(string message)`: Handle incorrect answer
- `AnimateMoveAndShowResult()`: Vehicle movement animation

---

## 🎮 Usage Examples

### Using Test Script
```csharp
// Test vehicle using TestScript
testScript.StartTest();  // Press T to start
testScript.StopTest();   // Press S to stop
testScript.ResetCarPosition(); // Press R to reset
```

### Create Custom Waypoint Path
```csharp
Vector3[] customPath = new Vector3[] {
    new Vector3(0, 0, 0),
    new Vector3(10, 0, 5),
    new Vector3(20, 0, 10),
    new Vector3(30, 0, 15)
};

autoCarController.MoveToWaypoints(customPath);
```

---

## 🔧 Tech Stack

- **Game Engine**: Unity 6000.0.45f1
- **XR Framework**: XR Interaction Toolkit 3.0.8
- **Physics Engine**: Unity Physics (WheelCollider)
- **Programming Language**: C#
- **Platform**: Android, iOS, VR Headsets

---

## 📝 Development Notes

### Implemented Features
- ✅ Waypoint-based autonomous driving
- ✅ X-axis based reverse system
- ✅ Real-time steering wheel rotation
- ✅ Collision-based quiz triggers
- ✅ Multiple quiz management system
- ✅ Timer and delay system
- ✅ Particle effects and sound

### Future Improvements
- [ ] AI-based path optimization
- [ ] Support for more quiz types
- [ ] Multiplayer mode
- [ ] Performance optimization
- [ ] More environment assets

---

## 👥 Team Members

- **dongchan**: Vehicle control system development
- **heeyeon**: UI and quiz system development

---

## 📄 License

This project is licensed under the MIT License.

---

## 🙏 Acknowledgments

- Unity Technologies - Unity Engine
- BoneCracker Games - Simple Car Controller
- Tokyo Street Asset Contributors

---

<div align="center">
Made with ❤️ using Unity
</div>
