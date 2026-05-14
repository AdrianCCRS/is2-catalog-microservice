$uri = "http://localhost:5290/api/v1/catalog/products/bulk/demo"
$response = Invoke-WebRequest -Uri $uri -Method Post -ContentType "application/json"
Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Response: $($response.Content)"
