import scrapy
from scrapy import signals
from controller import logger
from controller import orm
from datetime import datetime
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
        # Update the url list for scrapy
        self.start_urls = self.lighthouse.get_urls()
        # Notify that spider is running
        self.lighthouse.notify_running(True)
        
    # Method called whenever a spider is closed
    def spider_closed(self, spider):
        self.logger.info("Spider closed")
        # Update the lighthouse logs
        logger.update_log(self.lighthouse)
        # Notify that spider is not running
        self.lighthouse.notify_running(False)
        # Update the last visitor run time
        self.lighthouse.set_last_run(datetime.utcnow())
        
    # Method called when an error occurs within a spider
    def spider_error(self, failure, response, spider):
        # Handle errors here
        self.logger.error(failure)
        self.lighthouse.set_error_state(True)
        self.lighthouse.notify_running(False)
    
    # Method that parses the request for a given url
    def parse(self, response):
        data = []
        # Check for a template
        if self.lighthouse.has_template:
            for visitor in self.lighthouse.get_template_visitors():
                data.append(visitor(response, self.lighthouse.template.payload, self.logger))
        # No template found, run user script
        else:
            for visitor in self.lighthouse.get_script_visitors():
                data.append(visitor(response, self.logger))
        for item in data:
            yield(item)
    
    # Callback method for handling errors that occur during the Request
    def errback(self, failure):
        # log all failures
        self.logger.error(repr(failure))
        