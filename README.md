Summary of the project:
--------------------------------------------
1) Description
Implement a service to upload transaction data from files of various formats into database
and query transactions by specified criteria.
Use best practices and design patterns, skills in design/architecture, ability to build testable
and maintainable software.

2) Given
You have two possible formats of input files: csv and xml based. All values are mandatory so
if one missing then the record is invalid. If any record is invalid whole file is treated as invalid and
should not be imported. However, you should be able to identify all records that were invalid
and collect them in some log.

# This is a project required tools are: 
1) Visual Studio Code (https://code.visualstudio.com/)
2) MySQL Server

# The Backend programs are using : 
1) .Net Core 7
2) Linq to Entities

# The Database connected with: 
1) MySQL


# URL Start with:
1) To upload a single file
    https://localhost:7128/api/transactions/upload
2) To get info
    https://localhost:7128/api/transactions/GetTransactions

You also can use (http://localhost:5296/)

