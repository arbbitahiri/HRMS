USE [HRMS]
GO
/****** Object:  StoredProcedure [job].[StaffPayroll]    Script Date: 28-Feb-22 4:38:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Arb Tahiri
-- Create date: 28/02/2022
-- Description:	Job that will execute every end of the month.
-- ==========================================================
ALTER PROCEDURE [job].[StaffPayroll]
AS
BEGIN
	INSERT INTO [dbo].[StaffPayroll]
		([StaffID]
		,[DepartmentID]
		,[Month]
		,[BruttoSalary]
		,[EmployeeContribution]
		,[EmployerContribution]
		,[TotalTax]
		,[NettoSalary]
		,[InsertedFrom]
		,[InsertedDate])
	SELECT
		d.StaffID,
		d.DepartmentID,
		MONTH(GETDATE()),
		d.BruttoSalary,
		d.EmployeeContribution,
		d.EmployerContribution,
		(CASE WHEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) >= 450																						THEN ((170 * 0.04) + (200 * 0.08) + (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - 450) * 0.1)
			  WHEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) >= 250 AND (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) < 450	THEN ((170 * 0.04) + (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - 250) * 0.08)
			  WHEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) >= 80  AND (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) < 250	THEN ((d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - 80) * 0.04)
			  ELSE 0 END) AS TotalTax,
		(CASE WHEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) >= 450																						THEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - ((170 * 0.04) + (200 * 0.08) + (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - 450) * 0.1))
			  WHEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) >= 250 AND (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) < 450	THEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - ((170 * 0.04) + (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - 250) * 0.08))
			  WHEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) >= 80  AND (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) < 250	THEN (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - ((d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100) - 80) * 0.04))
			  ELSE (d.BruttoSalary - d.BruttoSalary * (d.EmployeeContribution * 1.0 / 100)) END) AS NettoSalary,
		'2bf29d98-aaba-48bd-86cd-fc4ba8bc353c',
		GETDATE()
	FROM StaffDepartment d
	WHERE
		GETDATE() BETWEEN d.StartDate AND d.EndDate
END
