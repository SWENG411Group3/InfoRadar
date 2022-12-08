from controller import script_tools
from controller import logger
from controller.scripting import *
from scraper.items import LighthouseItem
from itemadapter import ItemAdapter


@script_tools.visitor
def gather_data(context, response):
    # loop through each available table
    for tbl in range(1, number_of_tables(response)+1):
        table = Table(response, tbl)
        # loop through the row/col values in the configuration
        for col in context.config['cols']:
            for row in context.config['rows']:
                # Try to retrieve value from the table
                value = table.get_value(col, row)
                if value is not None:
                    sample_item = LighthouseItem(value=value, description="{} {}".format(row, col))
                    # Send through pipeline
                    yield sample_item
                else:
                    logger.info(f"Unable to scrape item for row {row}, column {col}.")

@script_tools.messenger
def send_data(context):
    # this function will be invoked by run_messenger.py and fed a list of "unchecked" records
    # the user will be responsible for checking the thresholds and sending a notification if required
    pass

@script_tools.pipeline
def process_data(item):
    adapter = ItemAdapter(item)
    if (adapter.get('value')):
        logger.info("Scraped price for {}: {}".format(adapter.get('description'), adapter.get('price')))
        # Here we can reach out to the controller and check any thresholds
        # for now just return            
    return item

def a_secret_third_thing():
    pass