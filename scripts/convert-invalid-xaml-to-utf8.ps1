# Converts XAML files that cannot be decoded as UTF-8 to UTF-8 (makes .bak backup)
$encUtf8 = New-Object System.Text.UTF8Encoding $false,$true
Get-ChildItem -Recurse -Filter *.xaml | ForEach-Object {
 $path = $_.FullName
 try {
 $encUtf8.GetString([System.IO.File]::ReadAllBytes($path)) | Out-Null
 Write-Host "OK: $path"
 } catch {
 Write-Host "Converting (backup saved): $path"
 $bak = "$path.bak"
 if (-not (Test-Path $bak)) { Copy-Item -Path $path -Destination $bak -Force }
 $bytes = [System.IO.File]::ReadAllBytes($path)
 # Try decode with system default encoding (likely Windows-1252) then write UTF8
 $text = [System.Text.Encoding]::Default.GetString($bytes)
 [System.IO.File]::WriteAllText($path, $text, [System.Text.Encoding]::UTF8)
 }
}