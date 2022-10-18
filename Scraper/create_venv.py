import os
import subprocess
import sys
import venv

def create_venv(venvdir, requirements):
    # Setup Python virtual environment.
    if not os.path.exists(venvdir):
        print("Creating Python virtual environment.")
        venv.create(venvdir, with_pip=True)
    else:
        print("Python virtual environment already exists.")

    # Update to the latest version of pip within the virtual environment.
    print("Updating pip.")
    python_exe = os.path.join(venvdir, "scripts", "python.exe")
    subprocess.run([python_exe, "-m", "pip", "install", "--upgrade", "pip"], check=True)

    # Install required packages.
    print("Installing pip packages.")
    pip_exe = os.path.join(venvdir, "scripts", "pip.exe")
    subprocess.run([pip_exe, "install", "-r", requirements], check=True)

def running_in_venv():
    return ((hasattr(sys, 'base_prefix')) and sys.base_prefix != sys.prefix)
