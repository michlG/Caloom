%~d0
CD "%~dp0"

SET DestStorage=
SET DestUri=http://127.0.0.1:10000/devstoreaccount1/
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\*.dll" "%DestUri%theballdemo-worker-role-accelerator"
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\CaloomWorkerRole.dll" "%DestUri%theballdemo-worker-role-accelerator"
Tools\Ext\CloudCopy.exe "%~dp0bin\CaloomWorkerRole\__entrypoint.txt" "%DestUri%theballdemo-worker-role-accelerator"
