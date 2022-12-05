import script_tools
from scripting import *
from controller import logger

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
                    price_item = PriceItem(price=value, description="{} {}".format(row, col))
                    # Send through pipeline
                    yield price_item
                else:
                    logger.info(f"Unable to scrape item for row {row}, column {col}.")

@script_tools.messenger
def send_data(context):
    pass

@script_tools.pipeline

def a_secret_third_thing():
    pass