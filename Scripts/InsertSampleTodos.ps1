Install-Module PSSQLite -Repository PSGallery
Import-Module PSSQLite

$root = Split-Path $PSScriptRoot -Parent
$db = "$root\Todo.db"

try {
  $query = "INSERT INTO TODO (Description, Completed) VALUES ('Feed The Dog', 0), ('Shop for groceries', 0);"
} catch {
  Write-Error $_
  Start-Sleep -s 5
  exit 1
}

Write-Host "`n"
Write-Host "Insert of todos successful" -ForegroundColor White -BackgroundColor DarkBlue -NoNewline
Write-Host "`n"

Invoke-SQLiteQuery -Query $query -DataSource $db