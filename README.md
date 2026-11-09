
# birmingHack — 2D Unity Game

A 2D game developed using Unity and C# as part of a hackathon project. The game combines movement, procedurally generated terrain and interactive storytelling to explore misinformation in a world shaped by AI.

## About the Project

The game follows Alex, a character navigating a future where AI-generated content makes it difficult to distinguish real information from misinformation.

Players move through the environment, encounter different scenarios and make decisions about what information to trust.

## Features

- **Player movement:** Jumping, backflips and movement using Unity's 2D physics system.
- **Procedural terrain:** Terrain generation using Perlin noise and Unity Sprite Shapes.
- **Interactive storytelling:** Scenario-based questions with multiple-choice answers.
- **Game states:** Logic for menus, gameplay and game-over screens.
- **Distance tracking:** Tracks player progress through the environment.
- **JSON-based content:** Storylines and questions stored separately from game logic.

## Technologies Used

- Unity
- C#
- Unity Input System
- Unity 2D Physics
- Unity Sprite Shapes
- JSON

## Project Structure

```text
test_birminghack_game/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs
│   │   ├── PlayerMovement.cs
│   │   ├── StoryManager.cs
│   │   └── Terrain/
│   │       ├── TerrainGenerator.cs
│   │       ├── TerrainManager.cs
│   │       └── TerrainChunk.cs
│   ├── Resources/
│   │   └── storyline.json
│   └── Scenes/
└── ProjectSettings/
```

## Running the Project

1. Clone or download this repository.
2. Open Unity Hub.
3. Add the `test_birminghack_game` folder as a project.
4. Open the project using a compatible Unity Editor version.
5. Open the relevant scene from `Assets/Scenes`.
6. Press Play in the Unity Editor.

## What This Project Explores

This project brings together game development, programming and interactive storytelling. It explores how game mechanics can encourage players to question information and make decisions rather than accepting everything they encounter.

## Project Status

Hackathon project / prototype.
