# Detect XAML files that cannot be decoded as UTF-8 (throw on invalid bytes)
$enc = New-Object System.Text.UTF8Encoding $false, $true # throwOnInvalidBytes = $true
Get-ChildItem -Recurse -Filter *.xaml | ForEach-Object {
 $path = $_.FullName
 try {
 $bytes = [System.IO.File]::ReadAllBytes($path)
 $enc.GetString($bytes) | Out-Null
 Write-Host "OK: $path"
 } catch {
 Write-Host "INVALID: $path" -ForegroundColor Red
 }
}
