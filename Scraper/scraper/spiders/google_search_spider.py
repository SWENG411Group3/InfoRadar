import scrapy
from scrapy import signals
from scrapy.linkextractors import LinkExtractor
from distutils.log import info
from controller import test
from scraper.items import LinkItem
import controller.logger as logger
from urllib.parse import urlparse
from scripting import *

class GoogleSearch(scrapy.Spider):
    name = "Google"
    start_urls = []
    url_query = "https://www.google.com/url?q="
    excludes = ['https://accounts.google.', 
                'https://support.google.',
                'www.youtube.com',
                'en.wikipedia.org']

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
        self.start_urls = [f'https://www.google.com/search?q={self.keyword}&num={self.num}&start={self.start}']
        self.logger.info(f"{spider.name} spider was opened")
    
    # Method called whenever a spider is closed
    def spider_closed(self, spider):
        # Handle any cleanup items here
        self.logger.info(f"{spider.name} spider was closed")
        # Update the logs
        logger.store_log(self.config)
        
    # Method called when an error occurs within a spider
    def spider_error(self, failure, response, spider):
        # Handle errors here
        self.logger.error(f"Error occurred within {spider.name} spider: {failure}")
        
    # Method that parses the request for a given url
    def parse(self, response):
        xlink = LinkExtractor(deny=self.excludes, unique=True)
        links = xlink.extract_links(response)
        
        # filter links to get just the search results
        potential_links = [link for link in links if link.url.startswith(self.url_query)]
#        query_list = []
#        if not self.config['allow-duplicates']:
#            for link in potential_links:
#                query_list.append(urlparse(urlparse(link.url).query.strip("q=")).hostname)
            
        self.logger.info(f"Scraped {len(potential_links)} links from query {self.start_urls[0]}")
        for link in potential_links:
            link_item = LinkItem(url=link.url, text=link.text,
                                 fragment=link.fragment, nofollow=link.nofollow)
            yield link_item

            