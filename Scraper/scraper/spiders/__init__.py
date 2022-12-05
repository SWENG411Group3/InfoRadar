# This package will contain the spiders of your Scrapy project
#
# Please refer to the documentation for information on how to create and manage
# your spiders.

import os
import sys
import logging

# Establish the root path
_spider_dir = os.path.dirname(__file__)
_scraper_sub_dir = os.path.join(os.path.dirname(_spider_dir))
_scraper_root = os.path.join(os.path.dirname(_scraper_sub_dir))

# Update sys.path if required and import controller
if _scraper_sub_dir not in sys.path: sys.path.append(_scraper_sub_dir)
if _scraper_root not in sys.path: sys.path.append(_scraper_root)

# This prevents scrapy log messages from propagating to the log file
logging.getLogger('scrapy').propagate = False

# Configure the default logger to be used by all spiders/pipelines
from controller import logger
logger.configure()