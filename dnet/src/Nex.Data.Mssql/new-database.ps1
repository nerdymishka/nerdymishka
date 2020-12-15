param (
    [Parameter(Position = 0)]
    [String]
    $DatabaseName = "Nex_Migrations"
)

$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $env:NMX_MSSQL_CONNECTION_STRING
$connection.Open()
$cmd = $connection.CreateCommand();
$cmd.CommandText = "CREATE DATABASE [$DatabaseName]"
$cmd.ExecuteNonQuery()
$cmd.Dispose()
$connection.Dispose()