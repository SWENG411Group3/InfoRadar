import argparse
import traceback
from controller.orm import from_env
from controller.logger import configure_logger, update_log
from controller.email import send_email
from datetime import datetime

parser = argparse.ArgumentParser(description='Script for running lighthouse messengers')

# Add positional argument for lighthouse ID
parser.add_argument('lighthouse_id', type=int, help='The ID of the lighthouse')

args = parser.parse_args()

# Retrieve the lighthouse
orm = from_env()
lh = orm.get_lighthouse(args.lighthouse_id)
# Configure the logger
configure_logger()

# Check if messengers exist for given lighthouse
messengers = lh.get_script_messengers()
template_messengers = lh.get_template_messengers()
messages = []
if len(messengers) > 0 or (lh.has_template and len(template_messengers) > 0):
    # Check if any new records have been written since last messenger run
    last_check = lh.get_last_sent_message()
    if last_check is None:
        unchecked_records = lh.get_unchecked_records(check_all=True)
    else:
        unchecked_records = lh.get_unchecked_records(timestamp=last_check)
    if len(unchecked_records) > 0:
        lh.logger.info(f"Found {len(unchecked_records)} new records since the last messenger run")
        lh.logger.info("Running messengers")
        # Run the messenger(s)
        try:
            for messenger in (template_messengers if lh.has_template else messengers):
                for record in unchecked_records:
                    # Check if lighthouse is using a template
                    if lh.has_template:
                        msg = messenger(record['values'], lh.template.payload, lh.logger)
                    else:
                        msg = messenger(record['values'], lh.logger)
                    if msg is not None: messages.append(msg)
            # No errors occurred, update the last message time to current utc
            lh.set_last_message(datetime.utcnow())
        except Exception as err:
            lh.logger.error(f"Error running messenger: {err}\t{traceback.format_exc()}")
            lh.set_error_state(True)
    else:
        lh.logger.info("No unchecked records found since last messenger run")
else:
    lh.logger.info(f"No configured messengers found for {lh.internal_name} lighthouse")

# email if any messages were populated
if len(messages) > 0:
    # Check for any email recipients
    email_list = lh.get_email_list()
    if len(email_list) > 0:
        lh.logger.info(f"Sending notification email(s) to {', '.join(email_list)}")
        body = "\n".join(messages)
        send_email(lh.internal_name, email_list, body)
    else:
        lh.logger.info(f"Messenger(s) have {len(messages)} notifications, but no email addresses are configured")
        lh.logger.info("Notification(s): " + "\n".join(messages))
else:
    if len(unchecked_records) > 0:
        # Messengers ran but didn't find anything
        lh.logger.info("No notifications were returned by the messenger(s)")

# Finally, update the log to reflect whatever just happened
update_log(lh)
