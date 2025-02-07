# MotoLogic - A Car Showroom Management System

# Overview
The inspiration for MotoLogic comes from a real-world understanding of the complexities in
managing a car dealership. Whether it's keeping track of inventory, managing customer
relationships, or ensuring staff assignments run smoothly, there are numerous moving parts.
This system aims to integrate and simplify these tasks while maintaining accuracy and
scalability.

# Features Overview
# Car Management
> Create, Read, Update, and Delete (CRUD) operations for vehicles.
> Store details including make, model, year, and associated owner.

# Owner Management
> CRUD operations for vehicle owners.
> Store first name, last name, and contact details.

# Staff Management
> CRUD operations for staff members.
> Store first name, last name, position, and assigned cars.

# Relationship Management

> Many-to-Many: Staff members can be assigned to multiple cars, and each car can have multiple staff members.
> One-to-Many: An owner can be associated with multiple cars, but each car belongs to a single owner.

# API Endpoints

# Cars API

GET /api/CarsAPI/List - Returns a list of all cars.

GET /api/CarsAPI/{id} - Returns details of a specific car by ID.

POST /api/CarsAPI/Add - Adds a new car.
PUT /api/CarsAPI/Update/{id} - Updates the details of a specific car by ID.
DELETE /api/CarsAPI/Delete/{id} - Deletes a specific car by ID.

# Owner API

GET /api/Owner/List - Returns a list of all owners.
GET /api/Owner/{id} - Returns details of a specific owner by ID.
POST /api/Owner/Add - Adds a new owner.
PUT /api/Owner/{id} - Updates the details of a specific owner by ID.
DELETE /api/Owner/{id} - Deletes a specific owner by ID.

# Staff API

GET /api/StaffAPI/List - Returns a list of all staff members.
GET /api/StaffAPI/Find/{id} - Returns details of a specific staff member by ID.
POST /api/StaffAPI/Add - Adds a new staff member.
PUT /api/StaffAPI/Update/{id} - Updates the details of a specific staff member by ID.
DELETE /api/StaffAPI/Delete/{id} - Deletes a specific staff member by ID.
