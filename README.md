# 🚗 AutoHub

A production-ready Vehicle Marketplace built with **ASP.NET Core**, **PostgreSQL**, **Redis**, **AWS**, **Docker**, **GitHub Actions CI/CD**, **Prometheus**, **Grafana**, and **CloudWatch**.

> Designed to demonstrate modern backend architecture, cloud deployment, monitoring, and DevOps practices.




## ⭐ Key Highlights

- Production deployment on AWS EC2
- PostgreSQL hosted on Amazon RDS
- Image storage using Amazon S3
- Automated CI/CD with GitHub Actions
- HTTPS secured with Nginx and Let's Encrypt
- Background processing using Hangfire
- Redis caching
- Monitoring with CloudWatch, Prometheus & Grafana
- Health Checks and structured logging
- Dockerized development and production environments


![.NET](https://img.shields.io/badge/.NET-10-blueviolet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue)
![Docker](https://img.shields.io/badge/Docker-Enabled-blue)
![AWS](https://img.shields.io/badge/AWS-EC2%20%7C%20RDS%20%7C%20S3-orange)
![Redis](https://img.shields.io/badge/Redis-Cache-red)
![Hangfire](https://img.shields.io/badge/Hangfire-Background%20Jobs-green)
![License](https://img.shields.io/badge/License-MIT-success)




## 🌐 Live Demo

Frontend

https://autohub-app-theta.vercel.app

Backend API

https://autohub-demo.buckdns.org

Swagger

https://autohub-demo.buckdns.org/swagger




## 📷 Screenshots

### Home

(image)

### Vehicle Details

(image)

### Swagger

(image)

### Grafana Dashboard

(image)




## Why AutoHub?

AutoHub was built to demonstrate how a production-ready backend should be designed and deployed.

Instead of focusing only on CRUD operations, the project emphasizes:

- Cloud-native deployment
- Clean Architecture
- CI/CD
- Monitoring & Observability
- Secure authentication
- Scalable infrastructure




## Features

See

docs/FEATURES.md


## Architecture

See

docs/ARCHITECTURE.md


## API Documentation

Complete API reference:

docs/API.md

Swagger:

https://autohub-demo.buckdns.org/swagger


## Folder Structure

See

docs/FOLDER_STRUCTURE.md




## 🚀 Local Setup

### Prerequisites

Before running the project, make sure you have the following installed:

- .NET SDK 10
- Docker Desktop
- PostgreSQL client (PgAdmin or DBeaver)
- Git
- Visual Studio 2022 / Visual Studio Code

---

### 1. Clone the Repository

```bash
git clone https://github.com/JindalMistry/AutoHub.git
cd AutoHub
```

---

### 2. Start Required Infrastructure

From the solution root, run:

```bash
docker compose up -d
```

This will start the required infrastructure using Docker:

- PostgreSQL
- Redis
- MinIO

Docker will also create the required persistent volumes automatically.

> **Important:** Docker Desktop must be running before starting the API. Otherwise, the application will fail to connect to its dependencies.

---

### 3. Create the Database

Open PostgreSQL using **PgAdmin** or **DBeaver** and create a new database.

Example:

```
AutoHub
```

Use this database name in your User Secrets.

---

### 4. Create the MinIO Bucket

Open the MinIO Console:

```
http://localhost:9001
```

Login using:

```
Username: minioadmin
Password: minioadmin
```

Create a bucket and use its name in your User Secrets.

Example:

```
autohub
```

---

### 5. Configure User Secrets

Initialize User Secrets:

```bash
dotnet user-secrets init
```

Configure the following values:

```json
{
  "Redis:ConnectionString": "localhost:6379",

  "Jwt:Secret": "CREATE_YOUR_SECRET",

  "Hangfire:Username": "admin",
  "Hangfire:Password": "admin123",

  "ConnectionStrings:DefaultConnection": "Host=localhost;Port=5342;Database=YOUR_DATABASE_NAME;Username=postgres;Password=postgres",

  "Storage:Provider": "MinIO",
  "Storage:BucketName": "YOUR_BUCKET_NAME",
  "Storage:Endpoint": "localhost:9000",
  "Storage:AccessKey": "minioadmin",
  "Storage:SecretKey": "minioadmin",
  "Storage:Region": "",
  "Storage:UseSSL": false
}
```

---

### 6. Run the API

Using Visual Studio:

- Set **AutoHub.API** as the startup project.
- Press **F5**.

Or using the .NET CLI:

```bash
dotnet run --project AutoHub.API
```

---

### 7. Verify Everything

After the application starts, verify the following:

| Service | URL |
|---------|-----|
| API | `https://localhost:<port>` |
| Swagger | `https://localhost:<port>/swagger` |
| Health Check | `https://localhost:<port>/health` |
| Hangfire | `https://localhost:<port>/hangfire` |
| MinIO Console | `http://localhost:9001` |