from controller import script_tools
#from controller import logger
from controller.scripting import *
from scraper.items import LighthouseItem
from itemadapter import ItemAdapter
import random

@script_tools.visitor
def gather_data(response, logger):
    #print(f"Scraping data for battery_lighthouse at {response.url}")
    item = LighthouseItem()
    item['value'] = random.uniform(1,10)
    item['description'] = "Simulated price"
    return item
    # loop through each available table
    # for tbl in range(1, number_of_tables(response)+1):
    #     table = Table(response, tbl)
    #     # loop through the row/col values in the configuration
    #     for col in context.config['cols']:
    #         for row in context.config['rows']:
    #             # Try to retrieve value from the table
    #             value = table.get_value(col, row)
    #             if value is not None:
    #                 sample_item = LighthouseItem(value=value, description="{} {}".format(row, col))
    #                 # Send through pipeline
    #                 yield sample_item
    #             else:
    #                 logger.info(f"Unable to scrape item for row {row}, column {col}.")

@script_tools.pipeline
def process_data(item, logger):
    adapter = ItemAdapter(item)
    if (adapter.get('value')):
        logger.info("Scraped value for {}: {}".format(adapter.get('description'), adapter.get('value')))
        # update database            
    return item

@script_tools.messenger
def send_data(context):
    # this function will be invoked by run_messenger.py and fed a list of "unchecked" records
    # the user will be responsible for checking the thresholds and sending a notification if required
    pass
