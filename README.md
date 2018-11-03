# MyInvoicingApp

*Read this in other languages: [Polish](README.pl-PL.md)

MyInvoicingApp is a application for registering customer invoices and asign them to created budget within available funds. 

### Technologies
ASP.NET Core 2.1 MVC, C#, JavaScript, JQuery, CSS, MS SQL Server

### Libraries/Frameworks
EntityFarmework Core, Identity, SyncFusion, DataTables, Alertify, Numerals, Bootstrap

## Installation:

In order to run the application, a MS SQL database is required. New database can be created or can be restored from provided backup with sample data.

### I. New database 

1. create new database ex. MyInvoicingAppDb using following script ```create database MyInvoicingAppDb```
1. change database in application config file `appsettings.xml` if necessary
1. do `Update-Database` in VisualStudio in **Package Manager Console**
1. add sample data by running script **\scripts\Sample_data.sql**


### II. SampleDB

1. restore database backup with sample data. Backup can be found in folder **SampleDB**

## Sample data:

Sample data contains:
1. four roles (including three system roles): 
    * `Accountant` - one of the system roles
    * `Manager` - one of the system roles
    * `Admin` - one of the system roles
    * `Temporary` - temporary role
1. three users (password: _Qazwsx1@_):
    * `marcin` asigned to roles: Accountant, Manager, Admin - can login and have access to all modules
    * `tomek` asigned to role: Accountant, Manager - can login and have access to all modules except administration module
    * `kasia` not asigned to any role - can login but don't have access to any module
1. collection of sample:
    * `customers`
    * `budgets`
    * `invoices`
    
## Available modules:

#### Functionalities available in all modules
* Export data from all tables to PDF and Excel
* Sorting data by available columns
* Filtering data

### Budgets

Budget module allows for creation, modification, closing, opening and browsing created budgets. In budget details a list of invoices asigned to current budgets is also available.

#### Functionalities
* Creation of new budget
* Modification of created budget
* Closing opened budget
* Opening closed budget
* Browsing budget details along with invoices asigned to current budget

### Customers

Customer module allows for creation, modification, closing, opening and browsing created customers. In customer details a list of invoices for current customer is also available.

#### Functionalities
* Creation of new customer
* Modification of created customer
* Closing opened customer
* Opening closed customer
* Browsing customer details along with invoices for current customer

### Invoices

Invoice module allows for creation, modification, cancellation and browsing created invoices. Each invoice is created for specific customer. Invocie lines are asigned to selected budget.

#### Functionalities
* Creation of new invoice
* Modification of created invoice
* Adding new line to invoice while in edit mode
* Invoice cancellation
* Invoice line cancellation
* Browsing invoice details
* Export invoice to PDF i Excel with predefined layout
* Adding attachments to invoice

### Administration

Administration module allows for creation, modification, closing, opening roles/positions and asigning users to roles/positions. 

#### Functionalities
* Creation of new role/position
* Modification of created role/position
* Closing opened role/position if its not one of system roles : `Accountant`, `Manager`, `Admin`
* Opening closed role/position
* Asigning users to roles/positions
* Removing user from role/position

## Application:

In order to get access to application modules login with user that is asigned to at least one of the roles: `Accountant` or `Manager` which provides access to modules: `Budgets`, `Customers`, `Invoices`. 
`Admin` role provides access to administration module which allows to manage roles/positions and asigning users to them.

##### to be continued.
