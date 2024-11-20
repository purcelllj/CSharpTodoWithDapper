Install-Module PSSQLite -Repository PSGallery
Import-Module PSSQLite

$root = Split-Path $PSScriptRoot -Parent
$db = "$root\Todo.db"

try {
  $query = "DROP TABLE Todo; CREATE TABLE Todo (
	Id INTEGER NOT NULL,
	Description	TEXT NOT NULL,
	Completed INTEGER NOT NULL,
	PRIMARY KEY(Id AUTOINCREMENT)
)"
} catch {
  Write-Error $_
  Start-Sleep -s 5
  exit 1
}

Write-Host "`n"
Write-Host "Drop and recreate of 'Todo' table successful" -ForegroundColor White -BackgroundColor DarkBlue -NoNewline
Write-Host "`n"

Invoke-SQLiteQuery -Query $query -DataSource $db