:: CLI args:
:: 1. Anaconda path

set "activate_conda_batch=%1\condabin\activate.bat"
call %activate_conda_batch%

conda env list | find /i "inforadar"
if errorlevel 1 (conda env create -f environment.yml) else (echo Env already exists)
