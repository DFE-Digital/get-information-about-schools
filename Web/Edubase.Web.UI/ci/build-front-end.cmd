call npm run gulp compile-ui-assets:prod

@echo off
echo Exit Code is %errorlevel%
@echo on
if not "%errorlevel%"=="0"  EXIT /B %errorlevel%
