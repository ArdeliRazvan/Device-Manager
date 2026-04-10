\# Device Manager



A full-stack asset management system built with \*\*.NET 8\*\*, \*\*MongoDB\*\*, and \*\*Angular 17\*\*.



\## 🛠️ Tech Stack

\- \*\*Backend\*\*: .NET 8 Web API

\- \*\*Database\*\*: MongoDB (Repository Pattern)

\- \*\*Frontend\*\*: Angular 17

\- \*\*Auth\*\*: JWT (JSON Web Tokens)



\## 🔐 Admin Credentials

For quick testing, use the pre-seeded admin account:

\- \*\*Email\*\*: `admin@demo.com`

\- \*\*Password\*\*: `Admin@123`



\## 🚀 Quick Start



\### 1. Clone \& Database

```bash

git clone \[https://github.com/ArdeliRazvan/Device-Manager.git](https://github.com/ArdeliRazvan/Device-Manager.git)

cd Device-Manager



Ensure MongoDB is running on localhost:27017, then seed the data:

Bash



mongosh mongodb://localhost:27017 --file scripts/init-db.js



2\. Run Backend

Bash



cd DeviceManager.API

dotnet run



&#x20;   API: http://localhost:5000 | Swagger: /swagger



3\. Run Frontend

Bash



cd device-manager-ui

npm install \&\& ng serve



&#x20;   App: http://localhost:4200



✨ Core Features



&#x20;   CRUD Operations: Full management of device inventory.



&#x20;   User-Device Mapping: Securely assign/unassign devices to users.



&#x20;   Identity: Registration and JWT-based login system.



&#x20;   Reliability: Integration tests using Testcontainers and idempotent scripts.



📡 API Endpoints

Method	Endpoint	Description

GET	/api/devices	Get all devices

POST	/api/devices/{id}/assign	Assign to current user

POST	/api/auth/login	Login \& get Token

GET	/api/users	List all users

