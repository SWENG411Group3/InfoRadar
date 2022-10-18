import scrapy
import w3lib.html
import logging
from logging import config
from scrapy import signals
from scraper.items import PriceItem
from scraper.xpath_methods import *
from controller import test
from controller import logger

class PriceMonitor(scrapy.Spider):
    name = "Price Monitor"
    start_urls = []
    config = {}
    logger.configure()
              
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
        # and copy or rename the temp_log.txt file to the correct path.
        # So far, the logging doesn't seem to be configurable from within the "spider_open" method,
        # but rather at the top of the class, which means the lighthouse_id is not available when the 
        # logger is configured. 
        pass
        
    # Method called when an error occurs within a spider
    def spider_error(self, failure, response, spider):
        # Handle errors here
        self.logger.error(failure)
        
    # Method that parses the request for a given url
    def parse(self, response):
        # Create a price item for the pipeline
        item = PriceItem()
        for table in range(0, number_of_tables(response)):
            for col in self.config['cols']:
                for row in self.config['rows']:
                    value = get_value_from_table(response, 1, col, row)
                    trimmed_val = w3lib.html.remove_tags(value).strip()
                    item['description'] = "{},{}".format(row, col)
                    item['price'] = trimmed_val
                    print(f'{row} {col}: {trimmed_val}')
                    # yield item for pipeline(s)
                    yield item
                    
        # Execute the template/user script
        #exec(self.config['script'])
        