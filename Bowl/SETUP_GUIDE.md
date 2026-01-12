# Bowling Game - Unity Setup Guide

Complete step-by-step guide to set up your iOS bowling game in Unity 6.

**Written by Claude Code on 2026-01-11**
**User request: Create physics-based iOS bowling game with swipe controls**

---

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Project Configuration](#project-configuration)
3. [Physics Materials Setup](#physics-materials-setup)
4. [Scene Hierarchy Setup](#scene-hierarchy-setup)
5. [GameObject Configuration](#gameobject-configuration)
6. [UI Setup](#ui-setup)
7. [Component Connection](#component-connection)
8. [Testing](#testing)
9. [iOS Build](#ios-build)

---

## Prerequisites

- Unity 6 (version 6000.0.23f1 or later)
- Universal Mobile 3D Template (already set up)
- iOS Build Support module installed
- All scripts already created in `/Assets/Scripts/`

---

## Project Configuration

### 1. Configure Tags

Go to **Edit > Project Settings > Tags and Layers**

**Add Tags:**
1. `BowlingBall`
2. `Pin`
3. `Gutter`

### 2. Configure Layers

In the same Tags and Layers window:

**Add Layers:**
- Layer 6: `Ball`
- Layer 7: `Pins`
- Layer 8: `Lane`
- Layer 9: `Bumpers`

### 3. Input System Setup

The project already has `InputSystem_Actions.inputactions`. The scripts use programmatic input setup, but you can also configure it manually:

1. Double-click `/Assets/InputSystem_Actions.inputactions`
2. Add a new Action Map named `Bowling`
3. Add actions:
   - `TouchPosition` (Value, Vector2) → `<Touchscreen>/primaryTouch/position`
   - `TouchPress` (Button) → `<Touchscreen>/primaryTouch/press`
   - `TouchDelta` (Value, Vector2) → `<Touchscreen>/primaryTouch/delta`

**Note:** The SwipeInputHandler script will work without this as it creates input actions programmatically.

---

## Physics Materials Setup

### 1. Create Ball Physics Material

1. Right-click in `/Assets/Materials/`
2. Select **Create > Physic Material** (NOT Physics Material 2D)
3. Name it `BallPhysicsMaterial`
4. Set properties:
   - **Dynamic Friction:** 0.3
   - **Static Friction:** 0.3
   - **Bounciness:** 0.2
   - **Friction Combine:** Average
   - **Bounce Combine:** Average

### 2. Create Pin Physics Material

1. Create new Physic Material
2. Name it `PinPhysicsMaterial`
3. Set properties:
   - **Dynamic Friction:** 0.4
   - **Static Friction:** 0.4
   - **Bounciness:** 0.1
   - **Friction Combine:** Average
   - **Bounce Combine:** Minimum

### 3. Create Lane Physics Material

1. Create new Physic Material
2. Name it `LanePhysicsMaterial`
3. Set properties:
   - **Dynamic Friction:** 0.1
   - **Static Friction:** 0.1
   - **Bounciness:** 0
   - **Friction Combine:** Minimum
   - **Bounce Combine:** Minimum

---

## Scene Hierarchy Setup

Open `SampleScene.unity` and build the following hierarchy:

### 1. Camera Setup

Select **Main Camera**:
- Position: `(0, 4, -10)`
- Rotation: `(25, 0, 0)`
- FOV: `60`

### 2. Create GameManager

1. Create empty GameObject: **GameObject > Create Empty**
2. Name it `GameManager`
3. Add scripts:
   - `GameManager.cs`
   - `BowlingScorer.cs`
   - `PinManager.cs`

### 3. Create InputManager

1. Create empty GameObject
2. Name it `InputManager`
3. Add script: `SwipeInputHandler.cs`

### 4. Create BowlingLane Structure

Create parent object:
1. Create empty GameObject, name it `BowlingLane`
2. Add script: `LaneController.cs`

**Under BowlingLane, create:**

#### a) Lane (Main surface)
1. Create **3D Object > Cube**
2. Name it `Lane`
3. Transform:
   - Position: `(0, 0, 8)`
   - Rotation: `(0, 0, 0)`
   - Scale: `(1, 0.1, 18)`
4. Add **Box Collider** (should be automatic)
5. In Box Collider, assign **Material:** `LanePhysicsMaterial`
6. Set Layer: `Lane`

#### b) Left Gutter
1. Create **3D Object > Cube**
2. Name it `LeftGutter`
3. Transform:
   - Position: `(-0.75, 0, 8)`
   - Scale: `(0.5, 0.1, 18)`
4. Box Collider:
   - Check **Is Trigger**
5. Tag: `Gutter`

#### c) Right Gutter
1. Create **3D Object > Cube**
2. Name it `RightGutter`
3. Transform:
   - Position: `(0.75, 0, 8)`
   - Scale: `(0.5, 0.1, 18)`
4. Box Collider:
   - Check **Is Trigger**
5. Tag: `Gutter`

#### d) Left Bumper
1. Create **3D Object > Cube**
2. Name it `LeftBumper`
3. Transform:
   - Position: `(-0.65, 0.25, 8)`
   - Scale: `(0.3, 0.5, 18)`
4. Box Collider: Keep as solid (not trigger)
5. Layer: `Bumpers`
6. **Disable this GameObject** (Inspector checkbox at top)

#### e) Right Bumper
1. Create **3D Object > Cube**
2. Name it `RightBumper`
3. Transform:
   - Position: `(0.65, 0.25, 8)`
   - Scale: `(0.3, 0.5, 18)`
4. Box Collider: Keep as solid
5. Layer: `Bumpers`
6. **Disable this GameObject**

#### f) Back Wall
1. Create **3D Object > Cube**
2. Name it `BackWall`
3. Transform:
   - Position: `(0, 0.5, 17)`
   - Scale: `(2, 1, 0.2)`
4. Box Collider: Keep as solid

### 5. Create Bowling Ball

1. Create **3D Object > Sphere**
2. Name it `BowlingBall`
3. Transform:
   - Position: `(0, 0.11, -8)`
   - Scale: `(0.22, 0.22, 0.22)` (radius 0.11m)
4. Add **Rigidbody** component:
   - **Mass:** 7.26
   - **Drag:** 0.5
   - **Angular Drag:** 0.5
   - **Use Gravity:** Checked
   - **Interpolate:** Interpolate
   - **Collision Detection:** Continuous
   - **Constraints:** Freeze Rotation X, Z (check both)
5. Sphere Collider:
   - **Material:** `BallPhysicsMaterial`
6. Add script: `BallController.cs`
7. Tag: `BowlingBall`
8. Layer: `Ball`

### 6. Create Pin Prefab

#### a) Create Pin GameObject
1. Create **3D Object > Capsule**
2. Name it `Pin`
3. Transform:
   - Position: `(0, 0.19, 16)` (temporary)
   - Scale: `(0.12, 0.19, 0.12)` (height 0.38m, radius 0.06m)
4. Add **Rigidbody**:
   - **Mass:** 1.5
   - **Drag:** 0.1
   - **Angular Drag:** 0.5
   - **Use Gravity:** Checked
   - **Interpolate:** Interpolate
   - **Collision Detection:** Continuous Dynamic
5. Capsule Collider:
   - **Material:** `PinPhysicsMaterial`
6. Add script: `PinController.cs`
7. Tag: `Pin`
8. Layer: `Pins`

#### b) Create Prefab
1. Drag the `Pin` GameObject to `/Assets/Prefabs/` folder
2. Delete the Pin GameObject from the scene

---

## UI Setup

### 1. Create UI Canvas

1. **GameObject > UI > Canvas**
2. Canvas settings:
   - **Render Mode:** Screen Space - Overlay
   - **UI Scale Mode:** Scale With Screen Size
   - **Reference Resolution:** 1920 x 1080
3. Add script to Canvas: `BowlingUI.cs`

### 2. Create Score Panel (Top Left)

Under Canvas:

#### Score Text
1. **GameObject > UI > Text - TextMeshPro**
2. Name: `ScoreText`
3. Rect Transform:
   - **Anchors:** Top-Left
   - **Position:** `(20, -20)` from top-left
   - **Width:** 300, **Height:** 60
4. TextMeshPro properties:
   - **Text:** "Score: 0"
   - **Font Size:** 36
   - **Alignment:** Left, Top
   - **Color:** White

#### Frame Text
1. **GameObject > UI > Text - TextMeshPro**
2. Name: `FrameText`
3. Rect Transform:
   - **Anchors:** Top-Left
   - **Position:** `(20, -80)` from top-left
   - **Width:** 200, **Height:** 40
4. TextMeshPro:
   - **Text:** "Frame: 1"
   - **Font Size:** 28
   - **Alignment:** Left, Top

#### Throw Text
1. **GameObject > UI > Text - TextMeshPro**
2. Name: `ThrowText`
3. Rect Transform:
   - **Anchors:** Top-Left
   - **Position:** `(20, -120)` from top-left
   - **Width:** 200, **Height:** 40
4. TextMeshPro:
   - **Text:** "Throw: 1"
   - **Font Size:** 28
   - **Alignment:** Left, Top

### 3. Create Status Text (Center)

1. **GameObject > UI > Text - TextMeshPro**
2. Name: `StatusText`
3. Rect Transform:
   - **Anchors:** Middle-Center
   - **Position:** `(0, 200)` from center
   - **Width:** 800, **Height:** 100
4. TextMeshPro:
   - **Text:** "Swipe to throw!"
   - **Font Size:** 48
   - **Alignment:** Center, Middle
   - **Color:** Yellow

### 4. Create Bumper Toggle (Bottom Right)

1. **GameObject > UI > Toggle**
2. Name: `BumperToggle`
3. Rect Transform:
   - **Anchors:** Bottom-Right
   - **Position:** `(-150, 50)` from bottom-right
   - **Width:** 200, **Height:** 40
4. Add script: `BumperToggle.cs`
5. Edit the Label text to say: "Bumpers"

---

## Component Connection

Now connect all the references in the Inspector:

### 1. GameManager GameObject

Select `GameManager` in Hierarchy.

#### GameManager Script:
- **Ball Controller:** Drag `BowlingBall` from Hierarchy
- **Pin Manager:** Reference itself (GameManager)
- **Input Handler:** Drag `InputManager` from Hierarchy
- **Scorer:** Reference itself (GameManager)
- **Bowling UI:** Drag `Canvas` from Hierarchy

#### PinManager Script:
- **Pin Prefab:** Drag `Pin` prefab from `/Assets/Prefabs/` folder
- **Formation Start Position:** `(0, 0.19, 16)`
- **Pin Spacing:** `0.24`

### 2. InputManager GameObject

Select `InputManager`:

#### SwipeInputHandler Script:
- **Minimum Swipe Distance:** `50`
- **Force Multiplier:** `0.01`
- **Min Force:** `3`
- **Max Force:** `20`
- **Show Debug Logs:** Check (for testing)

### 3. BowlingLane GameObject

Select `BowlingLane`:

#### LaneController Script:
- **Left Bumper:** Drag `LeftBumper` child object
- **Right Bumper:** Drag `RightBumper` child object
- **Left Gutter:** Drag `LeftGutter` child object
- **Right Gutter:** Drag `RightGutter` child object
- **Bumpers Enabled:** Unchecked (default off)

### 4. Canvas (UI)

Select `Canvas`:

#### BowlingUI Script:
- **Score Text:** Drag `ScoreText` from Hierarchy
- **Frame Text:** Drag `FrameText` from Hierarchy
- **Throw Text:** Drag `ThrowText` from Hierarchy
- **Status Text:** Drag `StatusText` from Hierarchy

### 5. BumperToggle

Select `BumperToggle`:

#### BumperToggle Script:
- **Lane Controller:** Drag `BowlingLane` from Hierarchy
- **Toggle:** Should auto-assign to itself

---

## Testing

### In Unity Editor (Play Mode)

1. Click **Play** button
2. **Mouse controls:**
   - Click and drag in Game view to simulate swipe
   - Vertical drag = forward power
   - Horizontal drag = lateral spin
3. **Check:**
   - Ball launches with appropriate force
   - Pins fall when hit
   - Score updates correctly
   - UI displays messages
   - Bumper toggle works

### Debug Features

All scripts have debug logs enabled. Check the Console window for:
- Swipe velocity and force calculations
- Ball launch confirmation
- Pin knock-down counts
- State transitions
- Score calculations

### Common Issues

**Ball doesn't move:**
- Check Rigidbody is on BowlingBall
- Check GameManager references are connected
- Check Input System is enabled in Project Settings

**Pins don't fall:**
- Check Pin prefab has Rigidbody
- Check Physics materials are assigned
- Check masses (Ball: 7.26, Pin: 1.5)

**UI doesn't update:**
- Check all TextMeshPro references in BowlingUI script
- Check Canvas is set to Screen Space - Overlay

**Swipe not detected:**
- In Editor, use mouse (click and drag)
- Check SwipeInputHandler logs in Console
- Try adjusting Minimum Swipe Distance

---

## iOS Build

### 1. Configure Build Settings

1. **File > Build Settings**
2. Select **iOS** platform
3. Click **Switch Platform** (if not already iOS)
4. Add `SampleScene` to build (should be Scene 0)

### 2. Player Settings

**File > Build Settings > Player Settings**

#### Identification:
- **Company Name:** Your name
- **Product Name:** Bowling Game
- **Bundle Identifier:** com.yourname.bowlinggame

#### Resolution and Presentation:
- **Default Orientation:** Landscape Left
- **Allowed Orientations:** Landscape only

#### Other Settings:
- **Target minimum iOS Version:** 13.0 or higher
- **Architecture:** ARM64
- **Graphics API:** Metal
- **Active Input Handling:** Input System Package (New) or Both

### 3. Build to iOS

1. Click **Build** in Build Settings
2. Choose output folder
3. Unity will generate Xcode project

### 4. Xcode Setup

1. Open `.xcodeproj` file in Xcode
2. Select your development team
3. Connect iOS device via USB
4. Select device as build target
5. Click **Run** (▶️ button)

### 5. Testing on Device

- Use finger swipes on screen
- Test different swipe speeds
- Verify 60 FPS performance
- Check touch responsiveness
- Test bumper toggle

---

## Optional Enhancements

### Visual Improvements

1. **Materials:**
   - Create materials in `/Assets/Materials/`
   - Lane: Brown/wood texture
   - Ball: Dark blue or custom color
   - Pins: White with red stripe
   - Gutters: Dark gray/black

2. **Lighting:**
   - Adjust Directional Light intensity
   - Add spot lights for dramatic effect
   - Configure URP settings for mobile

### Audio (Not Implemented)

To add audio:
1. Import sound effects
2. Add AudioSource to BallController
3. Play sounds on:
   - Ball launch
   - Pin collision
   - Strike/spare

### Particle Effects (Not Implemented)

To add effects:
1. Create Particle Systems
2. Trigger on:
   - Pin impacts
   - Strike achievements

---

## Summary

Your bowling game is now complete with:

✅ Physics-based ball and pin simulation
✅ Touch swipe input (works with mouse in editor)
✅ Bowling scoring system (strikes, spares)
✅ Single frame gameplay mode
✅ Bumper toggle for accessibility
✅ Clean UI with score tracking
✅ iOS-ready build configuration

**Next Steps:**
1. Set up the scene following this guide
2. Test in Unity Editor with mouse
3. Build to iOS device
4. Fine-tune physics values for feel
5. Add visual polish (materials, lighting)

---

**Created by Claude Code - 2026-01-11**
For questions or issues, refer to individual script comments.
