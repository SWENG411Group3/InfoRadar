from controller import script_tools
from scraper.items import LighthouseItem
from itemadapter import ItemAdapter

# This is a custom Lighthouse visitor function. It's responsible for all of the web scraping logic
# and will be executed against each URL contained in the Lighthouse list of Sites/Search Results.
# This function is expected to scrape data and return a LighthouseItem, populated with the data
# and description. The returned item is automatically sent to the custom pipeline function which is defined below
@script_tools.visitor
def scrape_table(response, logger):
    # The row indices for the table we want to scrape
    valid_rows_ind = [3,4,5,7,8]
    
    table_rows = []
    table_row = []
    for row in valid_rows_ind:
        # Get the battery technology string
        logger.info(f"Scraping battery technology from row {row}")
        table_row.append({'Battery_Technology': response.xpath(f'//table//tr[{row}]//td[1]/text()').get()})

        # Electrode Materials
        logger.info(f"Scraping Electrode Materials from row {row}")
        table_row.append({'Electrode_Materials': response.xpath(f'//table//tr[{row}]//td[2]/text()').get()})

        # Energy Density min/max
        logger.info(f"Scraping Density min/max from row {row}")
        density = response.xpath(f'//table//tr[{row}]//td[3]/text()').get().strip('~*').split('-')
        density_min = float(density[0])
        density_max = float(density[1]) if len(density) > 1 else None
        table_row.append({'Energy_Density_Min': density_min})
        table_row.append({'Energy_Density_Max': density_max})

        # Power Density/Temp
        logger.info(f"Scraping Power Density info from row {row}")
        power = response.xpath(f'//table//tr[{row}]//td[4]/text()').get().strip('~C').split('@')
        power_density = float(power[0])
        power_temp = float(power[1]) if len(power) > 1 else None
        table_row.append({'Power_Density': power_density})
        table_row.append({'Power_Density_Temp': power_temp})

        # Calculated Time to Fully charge
        logger.info(f"Scraping Calculated TTC from row {row}")
        table_row.append({'Calculated_TTC': response.xpath(f'//table//tr[{row}]//td[5]/text()').get()})
    
        table_rows.append(table_row.copy())
        table_row.clear()
    
    item = LighthouseItem()
    item['value'] = table_rows
    item['description'] = 'A table'
    return item
    
# This is a custom user pipeline function. It is responsible for receiving LighthouseItems from 
# the visitor function. The pipeline function can validate/modify any of the scraped data, and 
# ultimately contains the logic for what data will be placed in the database.
# Items returned by this function are sent to the main pipeline to be stored in the database.
@script_tools.pipeline
def process_data(item, logger):
    adapter = ItemAdapter(item)
    table = adapter.get('value')
    
    # For the sake of this demo, we'll just return the full table rather
    return item

# This is a custom messenger function. It is responsible for handling the notification logic for
# values that have been scraped. It is called with a list of values that correspond to the the 
# column names in the database. Values can be extracted using the get() method and passing in
# a valid table column name for the lighthouse.
@script_tools.messenger
def send_data(values, logger):
    messages = []
    DENSITY_THRESHOLD = 199
    if values.get('Energy_Density_Min') > DENSITY_THRESHOLD:
        messages.append(f'Energy density for {values.get("Battery_Technology")} has exceeded {DENSITY_THRESHOLD}!!')
    
    # Convert the messages to a single string separated by a newline and return
    return "\n".join(messages) if len(messages) > 0 else None
