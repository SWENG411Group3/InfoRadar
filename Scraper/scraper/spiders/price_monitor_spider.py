import scrapy
import logging
from logging import config
from scrapy import signals
from scraper.items import PriceItem
from controller import test
from controller import logger
from scraper.items import PriceItem
from scripting import *

class PriceMonitor(scrapy.Spider):
    name = "Price Monitor"
    start_urls = []
    config = {}
    
    @classmethod
    def from_crawler(cls, crawler, *args, **kwargs):
        spider = super(PriceMonitor, cls).from_crawler(crawler, *args, **kwargs)
        crawler.signals.connect(spider.spider_opened, signal=signals.spider_opened)
        crawler.signals.connect(spider.spider_closed, signal=signals.spider_closed)
        crawler.signals.connect(spider.spider_error,  signal=signals.spider_error)
        return spider
    
    # Method called whenever a spider is opened
    def spider_opened(self, spider):
        # Retrieve the lighthouse configuration from the controller
        self.config = test.fetch_lighthouse_info(self.lighthouse_id)
        #Update the starting urls
        self.start_urls = self.config['urls']
        self.logger.info("Spider opened: {}".format(spider.name))
        
    # Method called whenever a spider is closed
    def spider_closed(self, spider):
        # Handle any cleanup items here
        # TODO: retrieve the log file location from the config
        self.logger.info("Spider closed: {}".format(spider.name))
        logger.store_log(self.config)
        
    # Method called when an error occurs within a spider
    def spider_error(self, failure, response, spider):
        # Handle errors here
        self.logger.error(failure)
        
    # Method that parses the request for a given url
    def parse(self, response):
        # loop through each available table
        for tbl in range(1, number_of_tables(response)+1):
            table = Table(response, tbl)
            # loop through the row/col values in the configuration
            for col in self.config['cols']:
                for row in self.config['rows']:
                    # Try to retrieve value from the table
                    value = table.get_value(col, row)
                    if value is not None:
                        price_item = PriceItem(price=value, description="{} {}".format(row, col))
                        # Send through pipeline
                        yield price_item
                    else:
                        self.logger.info(f"Unable to scrape item for row {row}, column {col}.")
        # Execute the template/user script
        #exec(self.config['script'])
        