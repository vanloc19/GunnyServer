# Web Services & APIs

Comprehensive web services layer for GunnyArena game server, providing RESTful APIs, resource management, and launcher integration. Built with ASP.NET and Laravel, featuring high-performance request handling and scalable architecture.

## 📐 Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Client Applications                    │
│  (Game Client, Web Portal, Mobile App, Launcher)        │
└──────────────────────┬──────────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
   ┌────▼────┐   ┌────▼────┐  ┌─────▼─────┐
   │ Request │   │Resource │  │ Launcher  │
   │Handlers │   │ Server   │  │  Server   │
   │ (ASHX)  │   │          │  │ (Laravel) │
   └────┬────┘   └────┬─────┘  └─────┬─────┘
        │            │              │
        └────────────┼──────────────┘
                     │
              ┌──────▼──────┐
              │  Database   │
              │  & Services │
              └─────────────┘
```

## 🏗️ Project Structure

```
Web/
├── Request/                    # ASP.NET HTTP Request Handlers
│   ├── *.ashx                 # Generic HTTP handlers
│   ├── *.aspx                 # ASP.NET pages
│   ├── CelebList/             # Ranking/Leaderboard APIs
│   ├── CelebList/*.ashx       # Celebrity list handlers
│   ├── Properties/            # Assembly info
│   ├── Service References/    # WCF service references
│   └── *.xml                  # Response templates
│
├── Resource.Server/           # Game Resource Server
│   ├── image/                 # Game images (excluded from repo)
│   ├── sound/                 # Game sounds (excluded from repo)
│   ├── flash/                 # Flash resources
│   ├── *.xml                  # Configuration files
│   └── index.php              # Resource index
│
└── Launcher_Server/           # Laravel-based Launcher API
    ├── app/                   # Application core
    │   ├── Http/              # Controllers, Middleware
    │   ├── Models/            # Eloquent models
    │   └── Console/           # Artisan commands
    ├── config/                # Configuration files
    ├── database/              # Migrations, seeders
    ├── public/                # Public assets
    ├── resources/             # Views, assets
    ├── routes/                # Route definitions
    └── storage/               # Logs, cache, uploads
```

## 🚀 Components

### 1. Request Handlers (`Request/`)

**ASP.NET HTTP Handlers** - RESTful API endpoints for game operations

#### Key Features
- **159+ Request Handlers**: Comprehensive API coverage
- **ASHX Handlers**: Lightweight HTTP handlers
- **XML/JSON Responses**: Flexible response formats
- **Request Validation**: Input validation and sanitization
- **Error Handling**: Comprehensive error management

#### Main API Categories

##### Account Management
- `AccountRegister.ashx` - User registration
- `Login.ashx` - User authentication
- `CheckRegistration.ashx` - Registration validation
- `NickNameCheck.ashx` - Username availability
- `UserNameCheck.ashx` - Account name validation

##### Player Data
- `LoadUserItems.ashx` - Get player items
- `LoadUserEquip.ashx` - Get player equipment
- `LoadUserBox.ashx` - Get player inventory
- `LoadUserMail.ashx` - Get player mail
- `UserQuestList.ashx` - Get player quests
- `UserGoodsInfo.ashx` - Get player goods

##### Shop & Items
- `ShopItemList.ashx` - Shop item listing
- `ShopItemAllList.ashx` - All shop items
- `ShopGoodsShowList.ashx` - Featured shop items
- `LoadItemsCategory.ashx` - Item categories
- `ItemStrengthenList.ashx` - Item upgrade list

##### Guild/Consortia
- `ConsortiaList.ashx` - Guild listing
- `ConsortiaUsersList.ashx` - Guild members
- `ConsortiaEventList.ashx` - Guild events
- `ConsortiaAllyList.ashx` - Guild alliances
- `ConsortiaApplyUsersList.ashx` - Guild applications
- `ConsortiaNameCheck.ashx` - Guild name validation

##### Rankings & Leaderboards
- `CelebList/` - Celebrity/ranking handlers
  - `CelebByGpList.ashx` - GP rankings
  - `CelebByDayGPList.ashx` - Daily GP rankings
  - `CelebByWeekGPList.ashx` - Weekly GP rankings
  - `CelebByConsortiaHonor.ashx` - Guild honor rankings
  - `CelebByAchievementPointList.ashx` - Achievement rankings
  - And 20+ more ranking types

##### Game Data
- `ServerList.ashx` - Server list
- `MapServerList.ashx` - Map server list
- `NPCInfoList.ashx` - NPC information
- `QuestList.ashx` - Quest list
- `LevelList.ashx` - Level information
- `BallList.ashx` - Ball/item list
- `LoadMapsItems.ashx` - Map items
- `LoadPVEItems.ashx` - PvE items

##### Pet System
- `pettemplateinfo.ashx` - Pet templates
- `petskillinfo.ashx` - Pet skills
- `petskillelementinfo.ashx` - Pet skill elements
- `petskilltemplateinfo.ashx` - Pet skill templates
- `loadpetmoeproperty.ashx` - Pet properties

##### Payment & Recharge
- `ChargeMoney.aspx` - Payment processing
- `PayTransit.ashx` - Payment gateway
- `RechargeCardRequest` - Card recharge

##### Admin & GM Tools
- `KitoffUser.aspx` - Kick user from server
- `SendMailAndItem.aspx` - Send mail with items
- `SentReward.ashx` - Send rewards
- `ExperienceRate.aspx` - Adjust experience rate
- `SystemNotice.aspx` - System announcements

#### Request Handler Pattern

```csharp
public class ExampleHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // 1. Validate request
        // 2. Process business logic
        // 3. Query database
        // 4. Format response (XML/JSON)
        // 5. Return response
    }
}
```

### 2. Resource Server (`Resource.Server/`)

**Game Resource Management** - Static file server for game assets

#### Structure
- **image/**: Game images and textures (excluded from repo)
- **sound/**: Game sound effects and music (excluded from repo)
- **flash/**: Flash resources and assets
- **XML Files**: Configuration and metadata

#### Key Files
- `characterDefine.xml` - Character definitions
- `partical.xml` - Particle effects configuration
- `particallite.xml` - Lightweight particle config
- `crossdomain.xml` - Cross-domain policy
- `index.php` - Resource index page

#### Resource Management
- CDN-ready structure
- Efficient file serving
- Cache headers for performance
- Organized by resource type

### 3. Launcher Server (`Launcher_Server/`)

**Laravel-based API** - Modern RESTful API for game launcher

#### Technology Stack
- **Laravel Framework**: PHP framework
- **MySQL/MSSQL**: Database
- **RESTful APIs**: Modern API design
- **JWT Authentication**: Secure authentication
- **Eloquent ORM**: Database abstraction

#### Main Features

##### Authentication
- User registration
- Login with 2FA support
- Password reset
- Email verification
- Session management

##### Payment Integration
- ACB Bank integration
- MBBank integration
- Momo payment gateway
- Card recharge
- Payment history

##### Player Management
- Player list
- Player information
- Server selection
- Character management

##### Server Management
- Server list API
- Server status
- Server selection
- Update checking

##### Security Features
- Two-factor authentication (2FA)
- IP whitelisting
- Rate limiting
- CSRF protection
- Input validation

#### API Endpoints

```php
// Authentication
POST /api/login
POST /api/register
POST /api/logout
POST /api/forgot-password

// Payment
POST /api/charge
GET  /api/charge-history
POST /api/payment/callback

// Player
GET  /api/players
GET  /api/player/{id}
GET  /api/servers

// Launcher
GET  /api/update-check
GET  /api/play-vars
```

#### Laravel Structure

```
Launcher_Server/
├── app/
│   ├── Http/
│   │   ├── Controllers/      # API Controllers
│   │   │   ├── LoginController.php
│   │   │   ├── RegisterController.php
│   │   │   ├── ChargeController.php
│   │   │   ├── PaymentController.php
│   │   │   └── ...
│   │   ├── Middleware/        # Custom middleware
│   │   │   ├── ApiAuthentication.php
│   │   │   ├── IpMiddleware.php
│   │   │   └── ...
│   │   └── Requests/          # Form requests
│   ├── Models/                # Eloquent models
│   │   ├── Member.php
│   │   ├── Player.php
│   │   ├── PaymentMomo.php
│   │   └── ...
│   └── Console/               # Artisan commands
│       ├── ACB.php
│       ├── MBBank.php
│       └── Momo.php
├── config/                     # Configuration
├── database/
│   └── migrations/            # Database migrations
├── routes/
│   ├── api.php                # API routes
│   └── web.php                # Web routes
└── resources/
    └── views/                 # Blade templates
```

## 🛠️ Technology Stack

### Request Handlers
- **ASP.NET Framework**: Web application framework
- **C#**: Programming language
- **IIS**: Web server
- **XML/JSON**: Response formats

### Resource Server
- **PHP**: Server-side scripting
- **Apache/Nginx**: Web server
- **CDN**: Content delivery network

### Launcher Server
- **Laravel 8.x+**: PHP framework
- **PHP 7.4+**: Programming language
- **MySQL/MSSQL**: Database
- **Composer**: Dependency management
- **JWT**: Authentication tokens

## 📋 Prerequisites

### Request Handlers
- IIS 7.0+
- .NET Framework 4.7.2+
- SQL Server connection

### Resource Server
- PHP 7.0+
- Apache/Nginx
- Sufficient storage for resources

### Launcher Server
- PHP 7.4+
- Composer
- MySQL 5.7+ / SQL Server 2019+
- Laravel dependencies

## 🔧 Setup & Installation

### Request Handlers Setup

1. **Configure IIS**
   ```bash
   # Point IIS to Web/Request/ directory
   # Enable ASP.NET in IIS
   # Configure application pool (.NET Framework 4.x)
   ```

2. **Update Configuration**
   ```xml
   <!-- Web.config -->
   <connectionStrings>
     <add name="ConnectionString"
          connectionString="Server=...;Database=...;" />
   </connectionStrings>
   ```

3. **Set Permissions**
   - Grant IIS_IUSRS read/write permissions
   - Configure folder permissions

### Resource Server Setup

1. **Configure Web Server**
   ```bash
   # Point web server to Web/Resource.Server/
   # Enable PHP
   # Configure directory indexing
   ```

2. **Add Resources**
   - Place game images in `image/` directory
   - Place game sounds in `sound/` directory
   - Resources are excluded from Git (see .gitignore)

3. **Configure CDN** (Optional)
   - Set up CDN for static resources
   - Configure cache headers

### Launcher Server Setup

1. **Install Dependencies**
   ```bash
   cd Launcher_Server
   composer install
   ```

2. **Configure Environment**
   ```bash
   cp .env.example .env
   php artisan key:generate
   ```

3. **Configure Database**
   ```bash
   # Update .env with database credentials
   DB_CONNECTION=mysql
   DB_HOST=127.0.0.1
   DB_DATABASE=launcher_db
   DB_USERNAME=your_username
   DB_PASSWORD=your_password
   ```

4. **Run Migrations**
   ```bash
   php artisan migrate
   ```

5. **Configure Web Server**
   ```bash
   # Point web server to Launcher_Server/public/
   # Configure URL rewriting
   # Set document root to public/
   ```

## 🔒 Security Features

### Request Handlers
- **Input Validation**: All inputs validated
- **SQL Injection Prevention**: Parameterized queries
- **XSS Protection**: Output encoding
- **CSRF Protection**: Token validation
- **Rate Limiting**: Request throttling

### Launcher Server
- **Authentication**: JWT tokens
- **2FA Support**: Two-factor authentication
- **IP Whitelisting**: Restricted access
- **Rate Limiting**: API rate limits
- **Encryption**: Sensitive data encryption
- **HTTPS**: SSL/TLS support

## 📊 Performance Optimizations

### Request Handlers
- **Connection Pooling**: Database connection reuse
- **Response Caching**: Cache frequently accessed data
- **Async Operations**: Non-blocking I/O
- **Compression**: Gzip compression

### Resource Server
- **CDN Integration**: Content delivery network
- **Cache Headers**: Browser caching
- **Compression**: File compression
- **Lazy Loading**: On-demand resource loading

### Launcher Server
- **Query Optimization**: Efficient database queries
- **Caching**: Redis/Memcached support
- **API Caching**: Response caching
- **Database Indexing**: Optimized indexes

## 🧪 Testing

### Request Handlers
- **Unit Tests**: Test individual handlers
- **Integration Tests**: Test API endpoints
- **Load Testing**: Performance testing

### Launcher Server
```bash
# Run tests
php artisan test

# Run specific test
php artisan test --filter LoginTest
```

## 📝 API Documentation

### Request Handlers
- XML response format
- Standardized error codes
- Request/response examples

### Launcher Server
- RESTful API design
- JSON response format
- OpenAPI/Swagger documentation (if available)

## 🔍 Code Statistics

- **Request Handlers**: 159+ handlers
- **API Endpoints**: 200+ endpoints
- **Laravel Controllers**: 20+ controllers
- **Laravel Models**: 30+ models
- **Database Migrations**: 40+ migrations

## 🚀 Deployment

### Production Checklist
- [ ] Configure HTTPS/SSL
- [ ] Set up CDN for resources
- [ ] Configure database connection pooling
- [ ] Enable caching
- [ ] Set up monitoring
- [ ] Configure backup strategy
- [ ] Set up logging
- [ ] Configure error handling
- [ ] Performance testing
- [ ] Security audit

## 📚 Additional Resources

- Laravel Documentation: https://laravel.com/docs
- ASP.NET Documentation: https://docs.microsoft.com/aspnet
- IIS Configuration Guide
- API Best Practices

## 👥 Contributors

- **vanloc19** - Lead Developer

## 📄 License

**Proprietary - All Rights Reserved**

Copyright © 2024 vanloc19. All rights reserved.

---

**This web services layer provides robust, scalable APIs for game operations, resource management, and launcher integration, demonstrating modern web development practices and enterprise-level architecture.**

