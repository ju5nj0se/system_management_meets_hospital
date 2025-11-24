# 🏥 **System management meets**

## 📋 General Description of the System

That system is a web application developed with ASP.NET Core MVC that facilitates the management of medical appointments between doctors and patients.  
Its main goal is to streamline the administration of medical encounters (creation, viewing, updating, and deletion of appointments) and optimize the organization of information within the hospital.

The system allows:

- Managing doctors and patients (create, list, update, and delete).
- Scheduling new medical appointments between both.
- Validating availability to avoid appointment overlaps.
- Filtering appointments by doctor or patient ID.
- Managing appointment statuses: Pending, Attended, Canceled.

The primary role interacting with the system is the **Administrator**, who has full control over the management of medical appointments.

---

## 🚀 Steps to Run the Project

### Prerequisites
- Docker & Docker Compose

1. **Clone the repository:**
   ```bash
   git clone https://github.com/tu-usuario/Assesment_csharp.git
   cd SistemaGestionCitasHospital
   ```

2. **Run with Docker Compose:**
   Navigate to the docker directory and start the services.
   ```bash
   cd docker
   docker compose up -d --build
   ```

3. **Access the application:**
   Open your browser and go to: [http://localhost:8080](http://localhost:8080)

---

## 🛠️ Technologies Used

- **Framework:** ASP.NET Core MVC (.NET 9)
- **Database:** PostgreSQL 15
- **ORM:** Entity Framework Core
- **Containerization:** Docker & Docker Compose
- **Frontend:** Razor Views, Bootstrap 5, CSS
- **Git/GitHub**

---

## 📊 Diagrams

| Class Diagram |                  Use Case Diagram                  |
|:---:|:--------------------------------------------------:|   
| ![Class Diagram]() | ![Use Case Diagram]("./docs/DiagramUseCases.png") |


---

## 📸 Application Views

### Doctors Management
![Doctors Management](../docs/imgs/doctors.png)

### Patients Management
![Patients Management](../docs/imgs/pacients.png)

### Appointments Management
![Appointments Management](../docs/imgs/schedules.png)
