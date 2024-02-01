# TaskTracker

## Overview
TaskTracker is a dynamic task management system designed for optimizing team collaboration and project tracking. It offers a robust platform for managers and employees to manage tasks efficiently, track project progress, and maintain clear communication.

## Key Features
- **Task Management**: Create, assign, and manage tasks with detailed descriptions and due dates.
- **Project Tracking**: Access a dashboard for a comprehensive view of ongoing projects and task statuses.
- **User Roles**: Distinct roles for admins, managers, and employees with tailored functionalities.
- **Real-time Updates**: Employees can update task statuses, providing managers with up-to-date project insights.

## Installation & Setup

### Prerequisites
- .NET 8.0 SDK 
- Microsoft SQL Server
- Visual Studio 2022

### Setup Instructions
1. **Clone the Repository**
   git clone https://github.com/obd-79/TaskTracker
2. **Open Solution in Visual Studio**

3. **Restore NuGet Packages**
   dotnet restore


4. **Configure the Database**
- Update the connection string in `appsettings.json`.
- Apply migrations to set up the database:
  ```
  dotnet ef database update
  ```

5. **Run the Application**
- Start the project from Visual Studio.

## Usage Guide
- **Admins**: Manage users and projects via the `/admin` route.
- **Managers**: Oversee tasks and team performance on the `/manager` dashboard.
- **Employees**: View and update tasks assigned to them.

## Contributing
Contributions are welcome! Feel free to fork the repository and submit pull requests.

## License
This project is released under the [MIT License](LICENSE).

## Acknowledgements
- Special thanks to all contributors and supporters of Proje5.
- Project developed as a part of a .NET full-stack development course.

   

