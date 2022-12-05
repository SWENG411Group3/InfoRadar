import os
import logging
from logging.handlers import RotatingFileHandler
from scrapy.utils.log import configure_logging

_temp_log = "temp_log.txt"
_max_log_size = (1024 * 1000) * 10 #MB

def configure():
    if os.path.exists(_temp_log):
        os.remove(_temp_log)
    configure_logging(install_root_handler=False)
    logging.basicConfig(
        filename = _temp_log,
        filemode = 'a',
        format='%(asctime)-4s %(levelname)-8s %(message)s',
        level = logging.INFO)
    
def store_log(config):
    #TODO: 
    # Check <file> for size constraints. If <file> is not over the size limit then append
    # "temp_log.txt" to <file>. If <file> is over the limit, create a new file following whatever
    # naming convention we choose.
    # Question for group:
    # Is the database expected to have the log file name for a given lighthouse, or is it created here using the lighthouse internal name?
    log_size = os.path.getsize(config['log']) if os.path.exists(config['log']) else 0
    append_size = os.path.getsize(_temp_log)
    if (log_size + append_size) >= config['max-log-size']:
        pass
        # Create the new file here for
        # Question: what naming convention did we pick again?  !! <-----
    
    # Update lighthouse log
    with (open(config['log'], 'a') as real_log, open(_temp_log, 'r') as temp_log):
        real_log.write(temp_log.read())    
    
