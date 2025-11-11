# Production Deployment

Production-ready compiled services and configurations for GunnyArena game server. This directory contains all necessary files for deploying and running the game server in a production environment.

## 📐 Production Architecture

```
┌─────────────────────────────────────────────────────────┐
│                  Production Environment                  │
└──────────────────────┬──────────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
   ┌────▼────┐   ┌────▼────┐  ┌─────▼─────┐
   │ Center  │   │ Fighting │  │   Road    │
   │ Service │   │ Service  │  │  Service  │
   │ (EXE)   │   │  (EXE)   │  │  (EXE)    │
   └────┬────┘   └────┬─────┘  └─────┬─────┘
        │             │              │
        └─────────────┼──────────────┘
                      │
              ┌───────▼───────┐
              │  SQL Server   │
              │   Database    │
              └───────────────┘
```

## 🏗️ Directory Structure

```
Output/
├── center/                    # Center Service Production Files
│   ├── Center.Service.exe     # Center service executable
│   ├── Center.Service.exe.config  # Service configuration
│   ├── *.dll                  # Required DLL dependencies
│   ├── Languages/             # Localization files
│   │   ├── Language-vn.txt    # Vietnamese language
│   │   ├── Language-tr.txt    # Turkish language
│   │   └── SystemNotice.xml  # System announcements
│   ├── logconfig.xml          # Logging configuration
│   └── macrodrop/             # Macro drop configuration
│
├── fight/                     # Fighting Service Production Files
│   ├── Fighting.Service.exe   # Fighting service executable
│   ├── Fighting.Service.exe.config  # Service configuration
│   ├── *.dll                  # Required DLL dependencies
│   ├── Languages/             # Localization files
│   │   ├── Language-vn.txt    # Vietnamese language
│   │   └── SystemNotice.xml  # System announcements
│   └── logconfig.xml          # Logging configuration
│
└── road/                      # Road Service Production Files
    ├── Road.Service.exe       # Road service executable
    ├── Road.Service.exe.config  # Service configuration
    ├── *.dll                  # Required DLL dependencies
    ├── Languages/             # Localization files
    │   ├── Language-vn.txt    # Vietnamese language
    │   └── SystemNotice.xml   # System announcements
    ├── logconfig.xml          # Logging configuration
    ├── ScriptsPVE/            # PvE AI scripts (hot-reload)
    │   ├── AI/                # AI implementations
    │   │   ├── Game/          # Game AI scripts
    │   │   ├── Missions/       # Mission AI scripts
    │   │   └── NPC/           # NPC AI scripts
    │   └── Commands/          # Command scripts
    ├── datas/                 # Game data files
    │   ├── LittleGame/        # Mini-game data
    │   ├── consortiatask.data # Guild task data
    │   ├── shopfreecount.data # Shop data
    │   └── gmactivity.json    # GM activity data
    └── xml/                   # XML configuration files
```

## 🚀 Service Components

### 1. Center Service (`center/`)

**Central Server Management** - Authentication and server coordination

#### Key Files
- `Center.Service.exe` - Main service executable
- `Center.Service.exe.config` - Service configuration
- `Bussiness.dll` - Business logic library
- `Center.Server.dll` - Center server implementation
- `Game.Server.dll` - Game server core library
- `SqlDataProvider.dll` - Database access layer

#### Responsibilities
- Player authentication
- Server list management
- Cross-server communication
- Player session management
- Global server state coordination

#### Configuration
```xml
<!-- Center.Service.exe.config -->
<configuration>
  <connectionStrings>
    <add name="ConnectionString"
         connectionString="Server=localhost;Database=Db_Tank;..." />
  </connectionStrings>
  <appSettings>
    <add key="ServerID" value="1" />
    <add key="ServerName" value="Server 1" />
  </appSettings>
</configuration>
```

### 2. Fighting Service (`fight/`)

**Real-time Battle System** - Combat processing and room management

#### Key Files
- `Fighting.Service.exe` - Main service executable
- `Fighting.Service.exe.config` - Service configuration
- `Game.Server.dll` - Game server core library
- `Game.Logic.dll` - Game logic library
- `Fighting.Server.dll` - Fighting server implementation

#### Responsibilities
- Real-time combat processing
- Room creation and management
- Player synchronization in battles
- Battle result calculation
- PvP and PvE game modes

#### Configuration
```xml
<!-- Fighting.Service.exe.config -->
<configuration>
  <connectionStrings>
    <add name="ConnectionString"
         connectionString="Server=localhost;Database=Db_Tank;..." />
  </connectionStrings>
  <appSettings>
    <add key="MaxRooms" value="1000" />
    <add key="PlayersPerRoom" value="8" />
  </appSettings>
</configuration>
```

### 3. Road Service (`road/`)

**Game Progression Service** - Quest system and PvE content

#### Key Files
- `Road.Service.exe` - Main service executable
- `Road.Service.exe.config` - Service configuration
- `Game.Server.dll` - Game server core library
- `Game.Logic.dll` - Game logic library
- `ScriptsPVE/` - Hot-reloadable AI scripts

#### Responsibilities
- Quest processing
- Player level progression
- PvE game modes
- Script execution and hot-reload
- Story mode management

#### Configuration
```xml
<!-- Road.Service.exe.config -->
<configuration>
  <connectionStrings>
    <add name="ConnectionString"
         connectionString="Server=localhost;Database=Db_Tank;..." />
  </connectionStrings>
  <appSettings>
    <add key="ScriptPath" value="ScriptsPVE" />
    <add key="EnableHotReload" value="true" />
  </appSettings>
</configuration>
```

## 📋 Prerequisites

### System Requirements
- **OS**: Windows Server 2016+ / Windows 10+
- **.NET Framework**: 4.7.2 or higher
- **SQL Server**: 2019/2020+
- **RAM**: Minimum 4GB per service (8GB+ recommended)
- **CPU**: Multi-core processor (4+ cores recommended)
- **Disk**: SSD recommended for database and logs

### Network Requirements
- **Ports**:
  - Center Service: 9200 (default)
  - Fighting Service: 9201 (default)
  - Road Service: 9202 (default)
- **Firewall**: Configure firewall rules for service ports
- **Bandwidth**: Sufficient bandwidth for player connections

### Database Requirements
- SQL Server instance running
- Databases created and configured:
  - `Db_Admin`
  - `Db_GM`
  - `Db_Member`
  - `Db_Tank`
  - `Db_Tank41`
- Database user with appropriate permissions

## 🔧 Installation & Setup

### 1. Pre-Deployment Checklist

- [ ] SQL Server installed and running
- [ ] Databases created and configured
- [ ] .NET Framework 4.7.2+ installed
- [ ] Firewall ports configured
- [ ] Service accounts created (if using Windows Services)
- [ ] Log directories created with write permissions
- [ ] Configuration files prepared

### 2. Database Setup

```sql
-- Create databases
CREATE DATABASE Db_Admin;
CREATE DATABASE Db_GM;
CREATE DATABASE Db_Member;
CREATE DATABASE Db_Tank;
CREATE DATABASE Db_Tank41;

-- Run database scripts
-- (Execute scripts from Database/ folder)
```

### 3. Configuration

#### Update Connection Strings

Edit each service's `.exe.config` file:

```xml
<connectionStrings>
  <add name="ConnectionString"
       connectionString="Server=YOUR_SERVER;Database=Db_Tank;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;" />
</connectionStrings>
```

#### Configure Service Settings

```xml
<appSettings>
  <!-- Server Identification -->
  <add key="ServerID" value="1" />
  <add key="ServerName" value="Production Server 1" />

  <!-- Performance Settings -->
  <add key="MaxConnections" value="10000" />
  <add key="ThreadPoolSize" value="100" />

  <!-- Logging -->
  <add key="LogLevel" value="Info" />
  <add key="LogPath" value="logs\" />
</appSettings>
```

### 4. Deploy Services

#### Option A: Manual Execution

```bash
# Start Center Service
cd center
Center.Service.exe

# Start Fighting Service (in separate terminal)
cd fight
Fighting.Service.exe

# Start Road Service (in separate terminal)
cd road
Road.Service.exe
```

#### Option B: Windows Service Installation

```bash
# Install as Windows Service
Center.Service.exe install
Fighting.Service.exe install
Road.Service.exe install

# Start services
net start CenterService
net start FightingService
net start RoadService
```

### 5. Verify Deployment

- Check service logs for errors
- Verify database connections
- Test service endpoints
- Monitor resource usage
- Check firewall rules

## ⚙️ Configuration Files

### Logging Configuration (`logconfig.xml`)

```xml
<log4net>
  <appender name="FileAppender" type="log4net.Appender.FileAppender">
    <file value="logs/Service.log" />
    <appendToFile value="true" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %level %logger - %message%newline" />
    </layout>
  </appender>
  <root>
    <level value="INFO" />
    <appender-ref ref="FileAppender" />
  </root>
</log4net>
```

### Language Files (`Languages/`)

- `Language-vn.txt` - Vietnamese localization
- `Language-tr.txt` - Turkish localization
- `SystemNotice.xml` - System announcements

### System Notice Configuration

```xml
<!-- SystemNotice.xml -->
<SystemNotice>
  <Notice>
    <ID>1</ID>
    <Title>Welcome</Title>
    <Content>Welcome to GunnyArena!</Content>
    <Type>Info</Type>
  </Notice>
</SystemNotice>
```

## 🔒 Security Best Practices

### 1. Service Account
- Use dedicated service accounts (not administrator)
- Grant minimum required permissions
- Use strong passwords

### 2. Database Security
- Use SQL Server authentication with strong passwords
- Limit database user permissions
- Enable SQL Server encryption
- Regular security audits

### 3. Network Security
- Use firewall rules to restrict access
- Consider VPN for remote administration
- Enable SSL/TLS for sensitive communications
- Regular security updates

### 4. File Permissions
- Restrict file system permissions
- Protect configuration files
- Secure log files
- Backup encryption

## 📊 Monitoring & Maintenance

### Logging

#### Log Locations
- Center Service: `center/logs/`
- Fighting Service: `fight/logs/`
- Road Service: `road/logs/`

#### Log Rotation
- Configure log rotation to prevent disk space issues
- Archive old logs
- Monitor log file sizes

### Performance Monitoring

#### Key Metrics
- **CPU Usage**: Monitor per-service CPU usage
- **Memory Usage**: Track memory consumption
- **Database Connections**: Monitor connection pool
- **Network Traffic**: Track bandwidth usage
- **Player Count**: Monitor concurrent players

#### Tools
- Windows Performance Monitor
- SQL Server Activity Monitor
- Custom monitoring scripts
- Third-party monitoring tools

### Health Checks

```bash
# Check service status
sc query CenterService
sc query FightingService
sc query RoadService

# Check database connectivity
# (Use SQL Server Management Studio or scripts)

# Check log files for errors
# (Review recent log entries)
```

## 🔄 Updates & Maintenance

### Service Updates

1. **Backup Current Version**
   ```bash
   # Backup entire Output directory
   xcopy Output Output_backup /E /I
   ```

2. **Stop Services**
   ```bash
   net stop CenterService
   net stop FightingService
   net stop RoadService
   ```

3. **Deploy New Files**
   - Copy new executables and DLLs
   - Preserve configuration files
   - Update scripts if needed

4. **Start Services**
   ```bash
   net start CenterService
   net start FightingService
   net start RoadService
   ```

5. **Verify**
   - Check logs
   - Test functionality
   - Monitor performance

### Hot-Reload (Road Service)

Road Service supports hot-reloading of AI scripts:

```bash
# Scripts in ScriptsPVE/ can be updated without restart
# Service automatically detects and reloads changes
```

### Database Maintenance

- Regular database backups
- Index maintenance
- Statistics updates
- Log file management

## 🚨 Troubleshooting

### Common Issues

#### Service Won't Start
- Check .NET Framework version
- Verify configuration files
- Check database connectivity
- Review log files for errors
- Verify file permissions

#### Database Connection Errors
- Verify connection string
- Check SQL Server is running
- Verify user permissions
- Check firewall rules
- Test connection manually

#### High Memory Usage
- Check for memory leaks
- Review player count
- Optimize queries
- Increase available memory
- Consider service restart

#### Performance Issues
- Monitor CPU and memory
- Check database performance
- Review query execution plans
- Optimize indexes
- Consider scaling

### Log Analysis

```bash
# Check recent errors
findstr /i "error" logs\*.log

# Check specific service
type logs\CenterService.log | findstr /i "error"

# Monitor live logs
tail -f logs\Service.log
```

## 📈 Scaling & High Availability

### Horizontal Scaling

- Deploy multiple instances of services
- Use load balancer for distribution
- Configure database sharding
- Implement session management

### High Availability

- Deploy services on multiple servers
- Configure failover mechanisms
- Implement database replication
- Set up monitoring and alerts

### Load Balancing

- Distribute player load across instances
- Configure health checks
- Implement sticky sessions if needed
- Monitor load distribution

## 📦 Dependencies

### Required DLLs

Each service requires:
- `Bussiness.dll` - Business logic
- `Game.Server.dll` - Game server core
- `Game.Logic.dll` - Game logic
- `SqlDataProvider.dll` - Database access
- `Game.Base.dll` - Base classes
- `log4net.dll` - Logging
- `Newtonsoft.Json.dll` - JSON processing
- `protobuf-net.dll` - Protocol buffers
- `MoreLinq.dll` - LINQ extensions

### Third-party Libraries

- **log4net**: Logging framework
- **Newtonsoft.Json**: JSON serialization
- **Protobuf-net**: Network serialization
- **MoreLinq**: LINQ extensions

## 🔍 Production Checklist

### Pre-Launch
- [ ] All services tested
- [ ] Database configured and tested
- [ ] Configuration files verified
- [ ] Logging configured
- [ ] Monitoring set up
- [ ] Backup strategy implemented
- [ ] Security measures in place
- [ ] Performance tested
- [ ] Documentation reviewed

### Post-Launch
- [ ] Monitor service health
- [ ] Review logs regularly
- [ ] Track performance metrics
- [ ] Handle user issues
- [ ] Plan for scaling
- [ ] Regular maintenance schedule

## 📚 Additional Resources

- Service configuration documentation
- Database schema documentation
- API documentation
- Troubleshooting guides
- Performance tuning guides

## 👥 Support

For production issues:
- Check logs first
- Review configuration
- Consult documentation
- Contact development team

## 📄 License

**Proprietary - All Rights Reserved**

Copyright © 2024 vanloc19. All rights reserved.

---

**This production deployment package represents a complete, enterprise-ready game server solution, demonstrating professional deployment practices, monitoring capabilities, and scalable architecture for high-performance game operations.**

