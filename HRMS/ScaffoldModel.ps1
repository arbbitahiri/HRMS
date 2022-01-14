echo "*****************************************"
echo "* Updating model for HRMS from database *"
echo "*****************************************"

dotnet ef dbcontext Scaffold "Server=ARBTAHIRI;Database=HRMS_Work;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o .\Data\General --force --no-pluralize --context HRMS_Work

echo "*****************************************"
echo "* Done.                                 *"
echo "*****************************************"