USE [HRMS]
GO

/****** Object:  Table [dbo].[StaffPayroll]    Script Date: 28-Feb-22 4:41:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StaffPayroll](
	[StaffPayrollID] [int] IDENTITY(1,1) NOT NULL,
	[StaffID] [int] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[BruttoSalary] [money] NOT NULL,
	[EmployeeContribution] [decimal](5, 2) NOT NULL,
	[EmployerContribution] [decimal](5, 2) NOT NULL,
	[TotalTax] [money] NOT NULL,
	[NettoSalary] [money] NOT NULL,
	[InsertedFrom] [nvarchar](450) NOT NULL,
	[InsertedDate] [datetime] NULL,
 CONSTRAINT [PK_StaffPayroll] PRIMARY KEY CLUSTERED 
(
	[StaffPayrollID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[StaffPayroll]  WITH CHECK ADD  CONSTRAINT [FK_StaffPayroll_AspNetUsers] FOREIGN KEY([InsertedFrom])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[StaffPayroll] CHECK CONSTRAINT [FK_StaffPayroll_AspNetUsers]
GO

ALTER TABLE [dbo].[StaffPayroll]  WITH CHECK ADD  CONSTRAINT [FK_StaffPayroll_Department] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Department] ([DepartmentID])
GO

ALTER TABLE [dbo].[StaffPayroll] CHECK CONSTRAINT [FK_StaffPayroll_Department]
GO

ALTER TABLE [dbo].[StaffPayroll]  WITH CHECK ADD  CONSTRAINT [FK_StaffPayroll_Staff] FOREIGN KEY([StaffID])
REFERENCES [dbo].[Staff] ([StaffID])
GO

ALTER TABLE [dbo].[StaffPayroll] CHECK CONSTRAINT [FK_StaffPayroll_Staff]
GO


