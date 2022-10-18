from distutils.log import info
import scrapy
from scrapy import signals
import w3lib.html
from scrapy.linkextractors import LinkExtractor
import os, sys
import os.path as path
from controller import test
from xpath_methods import *

class GoogleSearch(scrapy.Spider):
    name = "Google"
    start_urls = []
    config = {}

    @classmethod
    def from_crawler(cls, crawler, *args, **kwargs):
        spider = super(GoogleSearch, cls).from_crawler(crawler, *args, **kwargs)
        crawler.signals.connect(spider.spider_opened, signal=signals.spider_opened)
        crawler.signals.connect(spider.spider_closed, signal=signals.spider_closed)
        crawler.signals.connect(spider.spider_error,  signal=signals.spider_error)
        return spider
    
    # Method called whenever a spider is opened
    def spider_opened(self, spider):
        # Retrieve the lighthouse configuration from the controller
        self.config = test.fetch_lighthouse_info(self.lighthouse_id)
        #Update the starting urls
        #self.start_urls = self.config['urls']
        self.start_urls = ['https://www.google.com/search?q={}'.format(self.keyword)]
    
    # Method called whenever a spider is closed
    def spider_closed(self, spider):
        # Handle any cleanup items here
        print("Closing Spider")
        
    # Method called when an error occurs within a spider
    def spider_error(self, spider):
        # Handle errors here
        print("Spider error")
        
    # Method that parses the request for a given url
    def parse(self, response):
        xlink = LinkExtractor()
        links = xlink.extract_links(response)
        # filter links until we have the search results
        for i in range(10):
            print(links[i])
            