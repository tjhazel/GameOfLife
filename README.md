# Game of Life API

A C# .NET 8.0 implementation of Conway's Game of Life with REST API endpoints for managing board states and simulating game progression.

## Functional Requirements

The API provides the following core endpoints:

- **UploadBoardState** - Upload initial board state (uses file system storage)
- **GetNextBoardState** - Calculate and return the next iteration of the game
- **GetBoardStateAtTick** - Get the board state at a specific tick (restarts from original)
- **GetFinalState** - Calculate and return the final stable state (restarts from original)

## Non-Functional Requirements

### Persistence
- Board states are persisted as JSON files on the local file system
- In production, this could easily be replaced with Azure Blob Storage or AWS S3

### Production Readiness
- Clean, modular, and testable architecture
- Basic error handling and validation (could be enhanced)
- Follows C# best practices
- Ready for deployment (excluding file-based storage)

**Note**: The `GridService` would benefit from additional testing and potential refactoring.

## Architecture Decisions

### Grid Structure
- **Top-left anchor [0,0]**: Uses the standard approach with position (0,0) at the top-left corner
- **Fixed grid**: Implements a bounded grid rather than infinite expansion (time constraint decision)
- Alternative considered: Center anchor [0,0] for symmetrical patterns, but would complicate external pattern interpretation

### API Design
- Clean, intuitive request/response contracts
- Web API built on top of domain layer for easy extension (console, functions, etc.)
- Swagger integration for F5 testing experience

### State Management
- Local file system for portability and development ease
- Designed for easy migration to document/blob storage in cloud environments

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio or Visual Studio Code

### Setup
1. Clone the repository locally
2. Open the solution in Visual Studio
3. Set `GameOfLife.Api` as the startup project
4. Run in debug mode to launch Swagger UI

### Usage
Access the application at `https://localhost:7247/index.html`

Use Swagger to interact with the API:
- **GetOriginalBoardState** - Retrieve original board state or pre-loaded game examples
- **GetLatestBoardState** - Get the current state including latest game progression
- **ResetGame** - Reset a game to its original state
- **GetGameList** - List all available games
- **UploadBoardState** - Upload new board configurations
- **GetNextBoardState** - Progress game by one tick
- **GetBoardStateAtTick** - Jump to specific game tick
- **GetFinalState** - Calculate final game state

## 📝 Getting Started with a new Game:

- Select the Swagger POST endpoint **UploadBoardState** 
- Create or reuse a pattern in the accepted game schema:
//sample glider pattern that moves diagonally
```
{
  "gameId": "glider",
  "pattern": [
    "........................................................",
    "...OOO..................................................",
    "...O....................................................",
    "...O....................................................",
    "....O...................................................",
    "........................................................",
    "........................................................"
  ]
}
```
- Cell state notation (. for dead, O for live)
- Game will choose width based on the longest row supplied and fill in the missing spaces to create a grid.
- Once posted, the game can be viewed and played
- **GetGameList** will return a list of common and all user loaded games available

## 📁 Project Structure

```
GameOfLife/
├── GameOfLife.Api/         # REST API & Swagger configuration
│   ├── Controllers/        # API endpoints
│   ├── Program.cs          # Application entry point
│   └── AppBuilder.cs       # abstraction to dry up startup
├── GameOfLife.Domain/      # Core business logic
│   ├── Content/            # Domain entities
│   │   ├── Original/       # stateless json game payloads 
│   │   └── Stateful/       # stateful json game payloads 
│   ├── Extensions/         # simple borrowed extension for 2d arrays
│   ├── Models/             # Domain entities
│   ├── Services/           # Business services
│   └── Game.cs             # Main game orchestrator
└── GameOfLife.Tests/       # Unit tests (stubs implemented)
    ├── Services/           # Service layer tests
    ├── Utilities/          # Xunit logger support
    └── GameTests.cs        # Domain layer tests
```

## 🧪 Testing

Test stubs are NOT implemented:

```bash
# Run all tests
dotnet test

# Run tests excluding ignored stubs
dotnet test --filter "Category!=Ignore"
```

**Current Status**: Test stubs with TODO implementations
- ✅ Test structure and naming conventions
- ✅ Mock setups and assertions planned  
- ⚠️ Implementation pending

## 🔄 Game of Life Rules

This implementation follows Conway's standard rules:
1. **Underpopulation**: Live cell with < 2 neighbors dies
2. **Survival**: Live cell with 2-3 neighbors survives  
3. **Overpopulation**: Live cell with > 3 neighbors dies
4. **Reproduction**: Dead cell with exactly 3 neighbors becomes alive

### 📚 Learn More About Conway's Game of Life

- **[Cornell University - Game of Life Tutorial](https://pi.math.cornell.edu/~lipa/mec/lesson6.html)** - Mathematical foundations and educational overview
- **[Wikipedia - Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)** - Comprehensive history, rules, and mathematical analysis
- **[Conway Life Patterns](https://conwaylife.com/patterns)** - Extensive collection of interesting patterns and configurations

## 🚧 Future Enhancements

- [ ] Replace file system with cloud blob storage
- [ ] Implement comprehensive unit tests
- [ ] Add integration tests
- [ ] Enhanced error handling and validation
- [ ] GridService refactoring based on test findings
- [ ] Infinite grid support
- [ ] Performance optimizations for large grids
- [ ] Real-time game progression via SignalR
- [ ] Pattern library and templates

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request