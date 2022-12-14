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
    item['field'] = 'Value1'
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
    if (adapter.get('value')) > 3:
        logger.info("Scraped value for {}: {}".format(adapter.get('description'), adapter.get('value')))
        # update database            
        return 
    else:
        logger.info(f"Dropping value: {adapter.get('value')} because it's below my made-up threshold")


@script_tools.messenger
def send_data(values, logger):
    messages = []
    val1 = values.get('Value1')
    val2 = values.get('Value2')
    if val1 > 500:
        messages.append(f"Sample notification from an actual messenger:\nValue1 exceeded the imaginary threshold of 500. Actual value: {val1}")
    if val2 < 250:
        messages.append(f"Another notification from an actual messenger:\nValue2 is less than the imaginary threshold of 250. Actual value: {val2}")
    return "\n".join(messages) if len(messages) > 0 else None
