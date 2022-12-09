import scrapy
from scrapy import signals
from controller import logger
from controller import orm
import logging

class LighthouseSpider(scrapy.Spider):
    name = "Lighthouse Spider"
    
    @classmethod
    def from_crawler(cls, crawler, *args, **kwargs):
        spider = super(LighthouseSpider, cls).from_crawler(crawler, *args, **kwargs)
        crawler.signals.connect(spider.spider_opened, signal=signals.spider_opened)
        crawler.signals.connect(spider.spider_closed, signal=signals.spider_closed)
        crawler.signals.connect(spider.spider_error,  signal=signals.spider_error)
        return spider
    
    def start_requests(self):
        for url in self.start_urls:
            yield scrapy.Request(url, callback=self.parse,
                                 errback=self.errback)
    
    # Method called whenever a spider is opened
    def spider_opened(self, spider):
        self.logger.info("Spider opened")
        # Retrieve the lighthouse configuration from the controller
        db = orm.from_env()
        self.logger.info("Retrieving lighthouse")
        self.lighthouse = db.get_lighthouse(self.lighthouse_id)
        self.logger.info(f"Running {self.lighthouse.internal_name}_lighthouse")
        self.lighthouse.notify_running(True)
        self.start_urls = self.lighthouse.get_urls()
        # notify database that spider is running
        
    # Method called whenever a spider is closed
    def spider_closed(self, spider):
        self.logger.info("Spider closed")
        # Update the lighthouse logs
        logger.update_log(self.lighthouse)
        # notify database that spider is not running
        self.lighthouse.notify_running(False)
        
    # Method called when an error occurs within a spider
    def spider_error(self, failure, response, spider):
        # Handle errors here
        self.logger.error(failure)
        self.lighthouse.set_error_state(True)
    
    # Method that parses the request for a given url
    def parse(self, response):
        visitors = self.lighthouse.get_visitors()
        data = []
        for visitor in visitors:
            data.append(visitor(response, self.logger))
        # Make sure we have one full list of data
        for item in data:
            yield(item)
    
    # Callback method for handling errors that occur during the Request
    def errback(self, failure):
        # log all failures
        self.logger.error(repr(failure))
        