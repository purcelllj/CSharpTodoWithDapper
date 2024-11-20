Install-Module PSSQLite -Repository PSGallery
Import-Module PSSQLite

$root = Split-Path $PSScriptRoot -Parent
$db = "$root\Todo.db"

try {
  $query = "DELETE FROM TODO; SELECT * FROM TODO;"
  Write-Host "`n"
  Write-Host "Database cleanup sucessful" -ForegroundColor White -BackgroundColor DarkBlue -NoNewline
  Write-Host "`n"
} catch {
  Write-Error $_
}

Invoke-SQLiteQuery -Query $query -DataSource $db