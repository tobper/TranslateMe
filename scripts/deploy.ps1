Param (
    $variables = @{},        
    $artifacts = @{},
    $scriptPath,
    $buildFolder,
    $srcFolder,
    $outFolder,
    $tempFolder,
    $projectName,
    $projectVersion,
    $projectBuildNumber
)

function Write-Header
{
    param($text)

    Write-Output ""
    Write-Output ""
    Write-Output "--------------------------------------------------------------------------------"
    Write-Output " $text"
    Write-Output "--------------------------------------------------------------------------------"
    Write-Output ""
}

function Publish-Release
{
    param(
        $oauth,
        $owner,
        $repo,
        $version,
        $files
    )
    
    Write-Header "Creating release"
    Write-Output "Owner: $owner"
    Write-Output "Repo: $repo"
    Write-Output "Version: $version"

    $uri = "https://api.github.com/repos/$owner/$repo/releases"

    $headers = @{
        "Authorization" = "token $oauth";
    }

    $data =  @{
        tag_name = $version;
        name = $version;
        prerelease = $True
    }

    $response = Invoke-WebRequest -Method Post -Uri $uri -Headers $headers -Body (ConvertTo-Json $data) -UseBasicParsing
    $release = ConvertFrom-Json $response.content

    foreach ($file in $files)
    {
        Publish-ReleaseAsset $oauth $release.upload_url $file
    }
}

function Publish-ReleaseAsset
{
    param(
        [string] $oauth,
        [string] $uploadUrl,
        [string] $file
    )

    $fileName = [IO.Path]::GetFileName($file)
    $uri = $uploadUrl.Replace("{?name}", "?name=$fileName")
    $contentType = Get-ContentType $fileName
    $data = [IO.File]::ReadAllBytes($file)

    Write-Output "File: $fileName"

    $wc = New-Object System.Net.WebClient
    $wc.Headers["Accept"] = "application/vnd.github.v3+json"
    $wc.Headers["Authorization"] = "token $oauth"
    $wc.Headers["Content-Type"] = $contentType
    $r = $wc.UploadData($uri, $data)
}

function Get-ContentType
{
    param($file)

    $ext = [IO.Path]::GetExtension($file)

    switch ($ext)
    {
        ".zip" { return "application/zip" }
        default { return "application/octet-stream" }
    }
}

$token = $variables.secureToken
$version = "v$projectVersion"
$files = $artifacts.values | foreach { $_.path }

Publish-Release $token "tobper" "TranslateMe" $version $files
