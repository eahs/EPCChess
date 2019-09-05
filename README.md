# ADSBackend
Advanced Data Structures mobile API backend boilerplate project

## Project Setup
- Open the Solution in Visual Studio 2019
- Initialize the SQL Server Express LocalDB
  - In Visual Studio, go to `View > Other Windows > Package Manager Console`
  - In the console that appears at the bottom, type the command `Update-Database` and wait for the migration to finish.
- Run the project
  - Press `Control + F5` to run the project without the debugger, or `F5` to run the project with the debugger attached.
- Seed Data
  - When running the project for the first time, the database will be seeded with an admin user.
  - You can log in to this account with the username `admin` and the password `Password123!`. It is _highly_ recommended that you change this password after logging in for the first time.

## Possible Issues
- Problem: Yellow triangle appears next to Dependencies for the ADSBackend project
  - Solution: Right click on the "ADSBackend" project and check to see that Target framework is set to .NET Core 2.2.  If it is not select "Install Other Frameworks" and install ".NET Core 2.2" for Visual Studio 2019 SDK.   Make sure to choose x64 SDK.  When finished installing, close and restart Visual Studio 2019. 