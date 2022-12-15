import os
import logging
from scrapy.utils.log import configure_logging

_temp_log = "temp_log.txt"
_max_log_size = (1024 * 1000) * 10 #MB

def configure_logger():
    if not os.path.isdir("logs"):
        os.mkdir("logs")
    if os.path.exists(_temp_log):
        os.remove(_temp_log)
    configure_logging(install_root_handler=False)
    logging.basicConfig(
        filename = _temp_log,
        filemode = 'a',
        format='%(asctime)-4s [%(name)s] %(levelname)-8s %(message)s',
        level = logging.INFO)
    
def update_log(lighthouse):
    current_log = get_latest_log(lighthouse.internal_name)
    log_size = os.path.getsize(current_log)
    append_size = os.path.getsize(_temp_log)
    combined_size = log_size + append_size
    if (combined_size) > _max_log_size:
        # Need a new file, but first write what we can
        remaining_bytes = _max_log_size - log_size
        with (open(current_log, 'a') as log, open(_temp_log,'r') as temp):
            lines = []
            # Write log lines until we reach the threshold
            while len("".join(lines).encode()) < remaining_bytes:
                lines.append(temp.readline())
            # Discard the last entry since it's over the threshold and 
            # store the proper file position for the next file
            last = lines.pop()
            start_pos = temp.tell() - len(last.encode()) - 1
            log.write("".join(lines))
        # Log is filled, rollover to next file
        current_log = _increment_log(lighthouse.internal_name)
        # Write remaining logs to the newer file
        with (open(current_log, 'a') as log, open(_temp_log,'r') as temp):
            temp.seek(start_pos)
            log.write(temp.read())
        # Update the database 
        lighthouse.set_log_index(_get_log_index(current_log))
    else:
        # We have room still, write entire temp log
        with (open(current_log, 'a') as log, open(_temp_log,'r') as temp):
            log.write(temp.read())

def get_latest_log(lighthouse_name):
    log_dir = os.path.join("logs", lighthouse_name)
    # Check if log directory exists
    if not os.path.isdir(log_dir):
        os.mkdir(log_dir)
    logs = os.listdir(log_dir)
    # Check if log_0 already exists
    if len(logs) == 0:
        latest_log = _create_log(lighthouse_name)
    else:
        latest_log = os.path.join(log_dir, logs[-1])
    return latest_log
    
def _create_log(lh_name):
    log_dir = os.path.join("logs", lh_name)
    log_file = os.path.join(log_dir, "log_0.txt")
    with open(os.path.join(log_file), 'w'):
        pass
    return log_file

def _increment_log(lh_name):
    # Get current log name
    current = os.path.basename(get_latest_log(lh_name))
    new_ind = _get_log_index(current) + 1
    # Create the new file
    new_log_file = os.path.join("logs", lh_name, f"log_{new_ind}.txt")
    with open(new_log_file, 'w'):
        pass
    return new_log_file

def _get_log_index(log):
    return int(os.path.basename(log).strip('.txt').split('_')[-1])
    