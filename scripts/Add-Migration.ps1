param (
    [Parameter(Mandatory)]
    [Alias("n")]
    [string] $name,
    [Alias("c")]
    [string] $context_root_dir = "D:\.dev\28c90573\projects\Reserve\Reserve.Data"
)

$working_dir = $(Get-Location).Path
Set-Location $context_root_dir
dotnet ef migrations add $name --context Reserve.Data.DatabaseContext --startup-project ../Reserve.API
Set-Location $working_dir