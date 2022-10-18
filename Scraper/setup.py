import os

from create_venv import create_venv

# Establish the relevant paths.
scraper_dir = os.path.abspath(os.path.dirname(__file__))

# Setup Python virtual environment.
requirements_file = os.path.join(scraper_dir, "requirements.txt")
venv_dir = os.path.join(scraper_dir, "pyenv")
create_venv(venv_dir, requirements_file)
