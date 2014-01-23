@echo off

set version=%appveyor_projectversion%
set configuration=Release

set wixSource=src\TranslateMe
set wixBinaries=%wixSource%\bin\%configuration%
set wixFiles=src\Installation
set wixObj=%wixFiles%\obj
set wixBin=%wixFiles%\bin

set msiFile=%wixBin%\TranslateMe-%version%.msi
set artifactFolder=%OutFolder%
set artifactAppFolder=%artifactFolder%\TranslateMe-%version%

call :verifyWixPath
call :buildSolution
call :buildInstallation
call :copyArtifacts
goto :eof


:verifyWixPath
	light.exe > nul 2> nul
	if errorlevel 1 call :addpath %~dp0..\tools\Wix38
	exit /b

:buildSolution
	call :header "Building version %version% in %configuration% mode"

	msbuild src\TranslateMe.sln /t:Publish /p:ApplicationVersion=%version%.0 /p:Configuration=%configuration%

	exit /b

:buildInstallation
	call :header "Creating msi package"

	candle.exe -ext WixUIExtension -ext WixNetFxExtension -out %wixObj%\ -dSrcDir=%wixSource% -dBinDir=%wixBinaries% -dBuildVersion=%version% %wixFiles%\TranslateMe.wxs

	if errorlevel 1 (
		echo MSI creation failed [Candle error %errorlevel%]
		exit %errorlevel%
	)

	light.exe -ext WixUIExtension -ext WixNetFxExtension -sval -pdbout %wixObj%\TranslateMe.wixpdb -out %msiFile% %wixObj%\TranslateMe.wixobj

	if errorlevel 1 (
		echo MSI creation failed [Light error %errorlevel%]
		exit %errorlevel%
	)

	exit /b

:copyArtifacts
	if not "%OutFolder%"=="" (
		call :header "Copying artifacts"

		copy %msiFile% %artifactFolder%

		md %artifactAppFolder%
		copy src\Translateme\bin\%configuration%\TranslateMe.exe %artifactAppFolder%
		copy src\Translateme\bin\%configuration%\TranslateMe.exe.config %artifactAppFolder%
	)

	exit /b

:header
	echo.
	echo.
	echo --------------------------------------------------------------------------------
	echo  %~1
	echo --------------------------------------------------------------------------------
	echo.
	exit /b

:addpath
	set path=%path%;%~1
	exit /b