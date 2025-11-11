# Game Server Source Code

Enterprise-grade game server implementation for real-time multiplayer tank battle game. This codebase demonstrates advanced software engineering practices, scalable architecture, and high-performance game server development.

## 📐 Architecture Overview

The game server follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                  Client Applications                      │
└──────────────────────┬──────────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
   ┌────▼────┐    ┌───▼────┐   ┌────▼────┐
   │ Center  │    │Fighting│   │  Road   │
   │ Service │    │ Service│   │ Service │
   └────┬────┘    └───┬────┘   └────┬────┘
        │             │             │
        └─────────────┼─────────────┘
                      │
        ┌─────────────┼─────────────┐
        │             │             │
   ┌────▼────┐  ┌────▼────┐  ┌────▼────┐
   │ Business│  │  Game   │  │   SQL   │
   │  Layer  │  │  Logic  │  │Provider │
   └────┬────┘  └────┬────┘  └────┬────┘
        │            │            │
        └────────────┼────────────┘
                     │
              ┌──────▼──────┐
              │ Game.Base   │
              │  (Core)     │
              └────────────┘
```

## 🏗️ Project Structure

### Core Libraries

#### `Game.Base/`
**Foundation layer** - Core infrastructure and base classes
- **Game.Base**: Base classes, interfaces, and utilities
- **Game.Base.Commands**: Command pattern implementation
- **Game.Base.Config**: Configuration management
- **Game.Base.Events**: Event system (Observer pattern)
- **Game.Base.Packets**: Network packet handling
- **Road.Base**: Road service base classes
- **Road.Base.Packets**: Road service packet definitions

**Key Features:**
- Abstract base classes for game objects
- Event-driven architecture
- Configuration management system
- Network packet serialization/deserialization

#### `Game.Logic/`
**Game Logic Layer** - Core game mechanics and rules
- **Game.Logic**: Main game logic classes
- **Game.Logic.Actions**: Game action handlers (50+ actions)
- **Game.Logic.AI**: Artificial Intelligence system
  - **Game.Logic.AI.Game**: Game-level AI
  - **Game.Logic.AI.Mission**: Mission AI
  - **Game.Logic.AI.Npc**: NPC AI behaviors
- **Game.Logic.Cmd**: Command handlers (33 commands)
- **Game.Logic.Effects**: Game effects system (48 effects)
- **Game.Logic.PetEffects**: Pet effect system (326 effects)
- **Game.Logic.Phy**: Physics engine
  - **Game.Logic.Phy.Actions**: Physics actions
  - **Game.Logic.Phy.Maps**: Map physics
  - **Game.Logic.Phy.Maths**: Math utilities
  - **Game.Logic.Phy.Object**: Physics objects
- **Game.Logic.Spells**: Spell system
  - **Game.Logic.Spells.FightingSpell**: Battle spells
  - **Game.Logic.Spells.NormalSpell**: Normal spells
- **CardEffect**: Card effect system (35 effects)

**Key Features:**
- State machine-based game logic
- Advanced AI with decision trees
- Physics simulation engine
- Comprehensive effect system
- Spell and card mechanics

#### `Game.Server/`
**Main Server Implementation** - Core server functionality
- **~2000 C# files** - Comprehensive server implementation
- Player management
- Room management
- Battle system
- Item management
- Quest system
- Guild/Consortia system
- Auction house
- Ranking system
- And much more...

**Key Features:**
- Real-time multiplayer support
- Scalable room management
- Complex game state management
- Player synchronization
- Event handling

### Service Implementations

#### `Center.Server/` & `Center.Service/`
**Central Server** - Authentication and server management
- Player authentication
- Server list management
- Cross-server communication
- Player session management

**Responsibilities:**
- User login/logout
- Server selection
- Player data synchronization
- Global server state

#### `Fighting.Server/` & `Fighting.Service/`
**Fighting Service** - Real-time battle system
- **Fighting.Server.GameObjects**: Game object definitions
- **Fighting.Server.Games**: Game mode implementations
- **Fighting.Server.Guild**: Guild battle system
- **Fighting.Server.Rooms**: Room management (5 room types)
- **Fighting.Server.Servers**: Server management

**Responsibilities:**
- Real-time combat processing
- Room creation and management
- Player synchronization in battles
- Battle result calculation

#### `Road.Service/`
**Road Service** - Game progression and quest system
- Quest management
- Player progression
- Story mode
- PvE content
- Script hot-reload support

**Responsibilities:**
- Quest processing
- Player level progression
- PvE game modes
- Script execution

### Data Access Layer

#### `SqlDataProvider/`
**Database Access Layer** - Data persistence
- **DAL**: Data Access Layer (SQL Helper, Connection Pooling)
- **SqlDataProvider.BaseClass**: Base data provider classes
- **SqlDataProvider.Data**: Data models (255+ data classes)
  - Player data
  - Item data
  - Quest data
  - Guild data
  - And more...

**Key Features:**
- Connection pooling
- Async database operations
- Parameterized queries (SQL injection prevention)
- Transaction management
- Data caching

#### `Bussiness/`
**Business Logic Layer** - Business rules and managers
- **Bussiness**: Core business logic
- **Bussiness.CenterService**: Center service business logic
- **Bussiness.Helpers**: Helper utilities
- **Bussiness.Interface**: Business interfaces
- **Bussiness.Managers**: Manager classes (27 managers)
- **Bussiness.Protocol**: Protocol definitions
- **Bussiness.WebLogin**: Web login integration

**Key Features:**
- Business rule enforcement
- Manager pattern implementation
- Service layer abstraction
- Protocol handling

### Client Integration

#### `Tank.Request/`
**HTTP Request Handlers** - Web API endpoints
- **159 C# files** - Comprehensive API implementation
- RESTful API handlers
- XML/JSON response formatting
- Request validation
- Error handling

**Key Features:**
- Account management APIs
- Item shop APIs
- Ranking APIs
- Guild APIs
- And more...

#### `Tank.Flash/`
**Flash Client Integration** - Client-server communication
- Flash client handlers
- Login processing
- Session management
- Client validation

#### `Road.Flash/`
**Road Flash Integration** - Road service client handlers
- Road service client communication
- Loading management
- Client utilities

## 🔧 Key Design Patterns

### 1. **Layered Architecture**
- Clear separation between presentation, business, and data layers
- Dependency injection between layers
- Interface-based design

### 2. **Manager Pattern**
- Centralized management classes for different game systems
- Singleton pattern for managers
- Lazy initialization

### 3. **Command Pattern**
- Game actions implemented as commands
- Undo/redo capability
- Command queue system

### 4. **Observer Pattern**
- Event-driven architecture
- Decoupled event handling
- Publisher-subscriber model

### 5. **Factory Pattern**
- Object creation abstraction
- Dynamic object instantiation
- Configuration-driven factories

### 6. **Repository Pattern**
- Data access abstraction
- Database-independent design
- Testable data layer

## 🎮 Core Systems

### Battle System
- **Real-time Combat**: Low-latency battle processing
- **Physics Engine**: Collision detection, movement, projectiles
- **Damage Calculation**: Complex damage formulas with modifiers
- **Skill System**: Cooldowns, effects, combos
- **Multiple Game Modes**: PvP, PvE, Training, Guild Wars

### AI System
- **State Machines**: NPC behavior states
- **Decision Trees**: AI decision making
- **Pathfinding**: Navigation algorithms
- **Boss AI**: Multi-phase boss battles
- **Dynamic Difficulty**: Adaptive AI difficulty

### Quest System
- **Quest Types**: Main, side, daily, weekly quests
- **Quest Chains**: Sequential quest progression
- **Quest Rewards**: Item, experience, currency rewards
- **Quest Conditions**: Complex condition checking

### Item System
- **Item Types**: Weapons, armor, consumables, materials
- **Equipment System**: Equipment slots, stats, bonuses
- **Upgrade System**: Item enhancement, refinement
- **Set Items**: Set bonuses for equipment combinations

### Guild System
- **Guild Management**: Creation, joining, leaving
- **Guild Wars**: Inter-guild battles
- **Guild Rankings**: Multiple ranking systems
- **Guild Benefits**: Shared resources, bonuses

## 📊 Performance Optimizations

### 1. **Connection Pooling**
```csharp
// Efficient database connection reuse
SqlHelper.ExecuteReader(connectionString, commandText, parameters);
```

### 2. **Async/Await Pattern**
```csharp
// Non-blocking I/O operations
await ProcessPlayerActionAsync(playerId, action);
```

### 3. **Object Pooling**
- Reuse game objects to reduce GC pressure
- Pre-allocated object pools
- Memory-efficient design

### 4. **Caching Strategy**
- In-memory caching for frequently accessed data
- Cache invalidation policies
- Distributed caching support

### 5. **Batch Processing**
- Group database operations
- Reduce round trips
- Transaction batching

## 🔒 Security Features

- **SQL Injection Prevention**: Parameterized queries
- **Input Validation**: All user inputs validated
- **Authentication**: Secure login system
- **Authorization**: Role-based access control
- **Rate Limiting**: API rate limiting
- **Data Encryption**: Sensitive data encryption

## 📝 Code Quality

### SOLID Principles
- **Single Responsibility**: Each class has one responsibility
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Proper inheritance hierarchy
- **Interface Segregation**: Focused interfaces
- **Dependency Inversion**: Depend on abstractions

### Code Organization
- **Namespaces**: Logical namespace structure
- **Naming Conventions**: Consistent naming throughout
- **Comments**: XML documentation for public APIs
- **Error Handling**: Comprehensive exception handling

## 🚀 Building the Project

### Prerequisites
- Visual Studio 2022
- .NET Framework 4.7.2+
- SQL Server 2019/2020+

### Build Steps

1. **Open Solution**
   ```bash
   # Open in Visual Studio
   start GunArena.sln
   ```

2. **Restore NuGet Packages**
   ```bash
   nuget restore GunArena.sln
   ```

3. **Build Solution**
   ```bash
   # In Visual Studio: Build > Build Solution (Ctrl+Shift+B)
   # Or via command line:
   msbuild GunArena.sln /p:Configuration=Release
   ```

4. **Build Individual Projects**
   ```bash
   msbuild Game.Base/Game.Base.csproj /p:Configuration=Release
   msbuild Game.Logic/Game.Logic.csproj /p:Configuration=Release
   msbuild Game.Server/Game.Server.csproj /p:Configuration=Release
   ```

### Build Order
1. `Game.Base` - Foundation layer
2. `SqlDataProvider` - Data access layer
3. `Game.Logic` - Game logic layer
4. `Bussiness` - Business layer
5. `Game.Server` - Main server
6. Service projects (Center, Fighting, Road)

## 🧪 Testing

### Unit Testing
- Test individual components in isolation
- Mock dependencies for testing
- Test business logic thoroughly

### Integration Testing
- Test service interactions
- Database integration tests
- API endpoint testing

## 📚 Dependencies

### NuGet Packages
- **log4net**: Logging framework
- **Newtonsoft.Json**: JSON processing
- **Protobuf-net**: Protocol buffer serialization
- **MoreLinq**: LINQ extensions

### Third-party Libraries
- **Lsj.Util**: Utility library
- **Lsj.Util.Dynamic**: Dynamic utilities
- **Lsj.Util.JSON**: JSON utilities

## 🔍 Code Statistics

- **Total Files**: ~2,000+ C# files
- **Lines of Code**: ~500,000+ lines
- **Classes**: 1,000+ classes
- **Interfaces**: 100+ interfaces
- **Managers**: 27+ manager classes
- **Effects**: 400+ effect implementations
- **AI Scripts**: 100+ AI behavior scripts

## 📖 Documentation

- **XML Comments**: Public APIs documented
- **Code Structure**: Clear namespace organization
- **Design Patterns**: Well-documented patterns
- **Architecture**: Layered architecture documentation

## 🎯 Best Practices

1. **Error Handling**: Always handle exceptions properly
2. **Logging**: Log important events and errors
3. **Performance**: Optimize hot paths
4. **Security**: Validate all inputs
5. **Maintainability**: Write clean, readable code
6. **Testing**: Test critical functionality
7. **Documentation**: Document complex logic

## 🚧 Development Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful variable names
- Keep methods focused and small
- Avoid deep nesting

### Git Workflow
- Feature branches for new features
- Meaningful commit messages
- Code reviews before merging
- Regular commits

## 📞 Support

For questions or issues:
- Check existing documentation
- Review code comments
- Contact the development team

---

**This codebase represents years of development and demonstrates enterprise-level software engineering practices, scalable architecture, and high-performance game server implementation.**
