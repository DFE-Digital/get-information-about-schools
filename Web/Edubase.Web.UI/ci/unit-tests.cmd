call npm run gulp build
@echo off
echo Exit Code is %errorlevel%
@echo on
if not "%errorlevel%"=="0"  EXIT /B %errorlevel%

