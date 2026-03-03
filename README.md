# 📘 Smart Documentation Hub

A real-time collaborative documentation platform built using **React.js**, **ASP.NET Core Web API (.NET 8)**, and **MySQL**.  
The system supports secure authentication, document versioning, backend text extraction, inline commenting, and real-time collaboration.

---

## 🚀 Features

- 🔐 JWT Authentication (Login, Forgot Password, Reset Password)
- 📄 Document Metadata Management
- 📤 File Upload Support (PDF, DOCX, TXT)
- 🧠 Backend Text Extraction & Normalization
- 🕒 Document Versioning
- 💬 Inline Comments using Text Indexing
- ⚡ Real-time Collaboration using SignalR
- 👤 User-based Access Control

---

## 🛠 Tech Stack

### Frontend
- React.js
- Axios
- SignalR Client
- Rich Text Editor (Quill / Slate)

### Backend
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SignalR
- JWT Authentication
- BCrypt Password Hashing

### Database
- MySQL

---

## 🏗 System Architecture

```
React Frontend
        ↓
ASP.NET Core Web API
        ↓
Service Layer
        ↓
Entity Framework Core
        ↓
MySQL Database
```

Real-time updates and collaboration are handled using **SignalR**.

---

## 🔁 Application Workflow

1. User logs in → JWT token is issued.
2. User creates document metadata.
3. File is uploaded → Backend extracts and normalizes text.
4. Document version is created and stored.
5. Inline comments are mapped using text indices.
6. Real-time collaboration is enabled via SignalR.

---

## 🔐 Security Implementation

- JWT-based authentication & authorization
- BCrypt password hashing
- Secure token-based password reset flow
- Backend-enforced ownership validation
- Protected API routes

---

## 📂 Project Structure

```
Smart-Documentation-Hub/
│
├── frontend/      # React.js Application
└── backend/       # ASP.NET Core Web API
```

---

## 🎓 Technical Highlights

- Clean Architecture with DTOs and Service Layer separation
- Backend-driven validation and security enforcement
- Real-time collaboration using SignalR
- Version-controlled document storage
- Designed for scalability and maintainability

---

## 📌 Future Enhancements

- Role-Based Access Control (RBAC)
- Cloud-based file storage (AWS / Azure)
- AI-based document insights and summarization
- Advanced collaboration tools (mentions, notifications)
- Activity logs and audit trails

---

## ⚙️ Setup Instructions

### 1️⃣ Clone Repository

```bash
git clone https://github.com/your-username/Smart_Documentation_Hub.git
cd Smart_Documentation_Hub
```

---

### 2️⃣ Backend Setup

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

Update `appsettings.json` with your MySQL connection string before running.

---

### 3️⃣ Frontend Setup

```bash
cd frontend
npm install
npm start
```

---

## 📄 License

This project is intended for **Academic / Educational Use**.

---

