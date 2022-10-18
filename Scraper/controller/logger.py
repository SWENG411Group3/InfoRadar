import logging
from logging.handlers import RotatingFileHandler
from scrapy.utils.log import configure_logging

def configure():
    configure_logging(install_root_handler=False)
    logging.basicConfig(
        filename = "temp_log.txt",
        filemode = 'a',
        format='%(asctime)-4s %(levelname)-8s %(message)s',
        level = logging.INFO)
    