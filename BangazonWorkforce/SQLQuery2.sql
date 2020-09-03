            SELECT e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                Department.Name
                
            FROM Employee e JOIN Department ON e.DepartmentId = Department.Id