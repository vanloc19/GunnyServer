# GunnyServer

High-performance game server implementation for GunnyArena - a real-time multiplayer tank battle game. Built with C# .NET Framework, featuring microservices architecture, scalable design, and comprehensive game logic.

## 🎮 Overview

GunnyServer is a robust, enterprise-grade game server solution designed to handle thousands of concurrent players. The architecture implements a distributed microservices pattern with separate services for game logic, center management, and road services, ensuring high availability and scalability.

## 🏗️ Architecture

### Microservices Design

```
┌─────────────────────────────────────────────────────────────┐
│                    Client Applications                       │
└────────────────────┬────────────────────────────────────────┘
                     │
        ┌────────────┼────────────┐
        │            │            │
   ┌────▼────┐  ┌───▼────┐  ┌───▼────┐
   │ Center  │  │Fighting│  │  Road  │
   │ Service │  │ Service│  │ Service│
   └────┬────┘  └───┬────┘  └───┬────┘
        │           │           │
        └───────────┼───────────┘
                    │
            ┌───────▼───────┐
            │ SQL Database │
            └──────────────┘
```

### Core Components

- **Center Service**: Central server management, player authentication, server list management
- **Fighting Service**: Real-time battle logic, room management, PvP/PvE combat system
- **Road Service**: Game progression, quest system, player data management
- **Web Services**: RESTful API endpoints, resource management, launcher integration

## 🚀 Key Features

### Game Server Features
- **Real-time Multiplayer**: Low-latency battle system supporting hundreds of concurrent players
- **Scalable Architecture**: Microservices design allows horizontal scaling
- **Advanced AI System**: Complex NPC behaviors with state machines and decision trees
- **Quest & Progression**: Comprehensive quest system with multiple difficulty levels
- **Guild/Consortia System**: Social features with guild wars and rankings
- **Item & Equipment System**: Complex item management with upgrades and enhancements
- **Pet System**: Pet collection, training, and battle integration
- **Auction House**: Player-to-player trading system
- **Ranking System**: Multiple leaderboards (daily, weekly, monthly)

### Technical Features
- **Async/Await Pattern**: Non-blocking I/O operations for high performance
- **Connection Pooling**: Efficient database connection management
- **Caching Layer**: In-memory caching for frequently accessed data
- **Logging System**: Comprehensive logging with log4net
- **Configuration Management**: Flexible configuration system
- **Script Hot-reload**: Dynamic script loading for game logic updates

## 📁 Project Structure

```
Server/
├── GameSrc/                    # Core game server source code
│   ├── Game.Server/            # Main game server logic
│   ├── Game.Logic/              # Business logic layer
│   ├── Game.Base/               # Base classes and utilities
│   ├── Center.Server/           # Center service implementation
│   ├── Center.Service/          # Center service executable
│   ├── Fighting.Server/         # Fighting service implementation
│   ├── Fighting.Service/       # Fighting service executable
│   ├── Road.Service/            # Road service implementation
│   ├── Bussiness/               # Business logic layer
│   ├── SqlDataProvider/         # Database access layer
│   ├── Tank.Request/            # Request handlers
│   └── Tank.Flash/              # Flash client integration
│
├── Web/                         # Web services and APIs
│   ├── Request/                 # HTTP request handlers (ASHX)
│   ├── Resource.Server/        # Game resource server
│   └── Launcher_Server/         # Laravel-based launcher API
│
└── Output/                      # Compiled services and configurations
    ├── center/                  # Center service output
    ├── fight/                   # Fighting service output
    └── road/                    # Road service output
```

## 🛠️ Technology Stack

### Backend
- **.NET Framework 4.x**: Core server framework
- **C#**: Primary programming language
- **SQL Server**: Database management system
- **log4net**: Logging framework
- **Protobuf**: Serialization for network communication
- **Newtonsoft.Json**: JSON processing

### Web Services
- **ASP.NET**: Web request handlers
- **Laravel (PHP)**: Launcher server API
- **RESTful APIs**: HTTP-based communication

### Development Tools
- **Visual Studio 2022**: IDE (latest version)
- **Git**: Version control
- **SQL Server Management Studio**: Database management

## 📋 Prerequisites

- **.NET Framework 4.7.2+**
- **SQL Server 2019/2020+**
- **Visual Studio 2022** (latest version, for development)
- **IIS** (for web services)
- **PHP 7.4+** (for Laravel launcher server)
- **Composer** (for PHP dependencies)

## 🔧 Setup & Installation

### 1. Database Setup

```sql
-- Create databases
CREATE DATABASE Db_Admin;
CREATE DATABASE Db_GM;
CREATE DATABASE Db_Member;
CREATE DATABASE Db_Tank;
CREATE DATABASE Db_Tank41;

-- Run database scripts (located in Database/ folder)
```

### 2. Configuration

Update configuration files in `Output/` directories:

```xml
<!-- Center.Service.exe.config -->
<connectionStrings>
  <add name="ConnectionString"
       connectionString="Server=localhost;Database=Db_Tank;..." />
</connectionStrings>
```

### 3. Build Project

```bash
# Open solution in Visual Studio
# Build solution (Ctrl+Shift+B)
# Or use MSBuild
msbuild GunArena.sln /p:Configuration=Release
```

### 4. Run Services

```bash
# Start Center Service
cd Output/center
Center.Service.exe

# Start Fighting Service
cd Output/fight
Fighting.Service.exe

# Start Road Service
cd Output/road
Road.Service.exe
```

### 5. Web Services Setup

```bash
# For ASP.NET handlers
# Configure IIS to point to Web/Request/

# For Laravel launcher
cd Web/Launcher_Server
composer install
cp .env.example .env
php artisan key:generate
php artisan migrate
```

## 🎯 Core Systems

### Battle System
- Real-time combat with physics simulation
- Multiple game modes (PvP, PvE, Training)
- Skill system with cooldowns and effects
- Damage calculation with equipment modifiers

### AI System
- State machine-based NPC behaviors
- Pathfinding algorithms
- Dynamic difficulty adjustment
- Boss AI with multiple phases

### Data Management
- Efficient data access layer with connection pooling
- Caching strategies for performance
- Transaction management
- Data synchronization across services

### Security
- SQL injection prevention
- Input validation
- Rate limiting
- Authentication and authorization

## 📊 Performance Optimizations

- **Connection Pooling**: Reuses database connections
- **Async Operations**: Non-blocking I/O for scalability
- **Memory Management**: Efficient object pooling
- **Caching**: Reduces database queries
- **Batch Processing**: Groups operations for efficiency

## 🔍 Code Quality

- **Separation of Concerns**: Clear layer architecture
- **SOLID Principles**: Well-structured codebase
- **Design Patterns**: Factory, Singleton, Observer patterns
- **Error Handling**: Comprehensive exception management
- **Code Documentation**: XML comments for public APIs

## 📈 Scalability

The server architecture supports:
- **Horizontal Scaling**: Multiple instances per service
- **Load Balancing**: Distribute players across servers
- **Database Sharding**: Partition data by server ID
- **Caching Layer**: Reduce database load

## 🧪 Development Workflow

```bash
# Clone repository
git clone git@github.com:vanloc19/GunnyServer.git
cd GunnyServer

# Open in Visual Studio
start GunArena.sln

# Build and run
# Use Visual Studio debugger or run executables directly
```

## 📝 Configuration Files

- `logconfig.xml`: Logging configuration
- `*.exe.config`: Service configurations
- `SystemNotice.xml`: System announcements
- `Language-*.txt`: Localization files

## 🔐 Security Considerations

- Database credentials stored in configuration files (use environment variables in production)
- Input sanitization on all user inputs
- SQL parameterized queries
- Rate limiting on API endpoints

## 📚 Additional Resources

- Game logic documentation in `GameSrc/README.md`
- API documentation for web services
- Database schema documentation

## 👥 Contributors

- **vanloc19** - Lead Developer & Architect

## 📚 Resources

- **Game Resources**: Sourced from China
- **Code**: Self-developed

## 📄 License

**Proprietary - All Rights Reserved**

Copyright © 2024 vanloc19. All rights reserved.

This software and associated documentation files (the "Software") are the proprietary and confidential property of vanloc19.

### Terms of Use

- **Unauthorized copying, modification, distribution, or use of this Software is strictly prohibited**
- This Software is provided for personal/educational purposes only
- Commercial use requires explicit written permission from the copyright holder
- The Software is provided "AS IS", without warranty of any kind

### Game Resources

- Game resources (images, sounds, assets) are sourced from China
- Resource files are not included in this repository
- All code implementations are original work by the developer

### Contact

For licensing inquiries or permissions, please contact the repository owner.

---

**Built with ❤️ for the gaming community**

*This server implementation demonstrates enterprise-level software architecture, scalable design patterns, and high-performance game server development.*

