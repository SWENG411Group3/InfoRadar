import scrapy
import logging
from logging import config
from scrapy import signals
from scraper.items import PriceItem
from controller import test
from controller import logger
from scraper.items import PriceItem
from controller import orm
from controller import context

class LighthouseSpider(scrapy.Spider):
    name = "Lighthouse Spider"
    start_urls = ['https://www.lme.com/en/Metals/EV/About-Lithium']
    config = {}
    lighthouse = None
    
    @classmethod
    def from_crawler(cls, crawler, *args, **kwargs):
        spider = super(LighthouseSpider, cls).from_crawler(crawler, *args, **kwargs)
        crawler.signals.connect(spider.spider_opened, signal=signals.spider_opened)
        crawler.signals.connect(spider.spider_closed, signal=signals.spider_closed)
        crawler.signals.connect(spider.spider_error,  signal=signals.spider_error)
        return spider
    
    # Method called whenever a spider is opened
    def spider_opened(self, spider):
        # Retrieve the lighthouse configuration from the controller
        #self.lighthouse = test.fetch_lighthouse_info(self.lighthouse_id)
        db = orm.from_env()
        self.lighthouse = db.get_lighthouse(self.lighthouse_id)
        
        #self.lighthouse = db.get_lighthouse(self.lighthouse_id)
        #Update the starting urls
        #self.start_urls = self.config['urls']
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
    
    # Method for retrieving the underlying lighthouse object from outside the spider    
    def get_lighthouse(self):
        return self.lighthouse
    
    # Method that parses the request for a given url
    def parse(self, response):
        print(self.lighthouse)
        # visitors = self.lighthouse.get_visitors()
        # for visitor in visitors:
        #     visitor(context(self.lighthouse), response)
        
