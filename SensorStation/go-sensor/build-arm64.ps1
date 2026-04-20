go mod download
$env:GOOS="linux"
$env:GOARCH="arm64"
$env:CGO_ENABLED = "0"
go build -o ".\dist\sensorstation-arm64-rpi"
