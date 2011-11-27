@ECHO UnInstalling Job Service...
@SET PATH=%PATH%;C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\
@InstallUtil /u ..\B4F.WindowsService.JobService.exe
@ECHO UnInstall Done.
@pause