call grunt test-server
@echo off
echo Exit Code is %errorlevel%
@echo on
if not "%errorlevel%"=="0"  EXIT /B %errorlevel%

