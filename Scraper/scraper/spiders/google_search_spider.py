import scrapy
from scrapy import signals
from scrapy.linkextractors import LinkExtractor
from distutils.log import info
from controller import test
from scraper.items import LinkItem
import controller.logger as logger
from urllib.parse import urlparse
from controller.scripting import *
from controller import orm

class GoogleSearch(scrapy.Spider):
    name = "Google Spider"
    start_urls = []
    url_query = "https://www.google.com/url?q="
    excludes = ['https://accounts.google.', 
                'https://support.google.',
                'www.youtube.com']

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
        self.logger.info("Spider was opened")
        # Retrieve the lighthouse configuration from the controller
        self.config = test.fetch_lighthouse_info(self.lighthouse_id)
        db = orm.from_env()
        self.lighthouse = db.get_lighthouse(self.lighthouse_id)
        self.logger.info(f"Running {self.lighthouse.internal_name}_lighthouse")
        #Update the starting urls
        self.start_urls = [f'https://www.google.com/search?q={query}&num=10&start=0' for query in self.lighthouse.get_google_queries()]
    # Method called whenever a spider is closed
    def spider_closed(self, spider):
        # Handle any cleanup items here
        self.logger.info("Spider was closed")
        # Update the logs
        logger.update_log(self.lighthouse)
        
    # Method called when an error occurs within a spider
    def spider_error(self, failure, response, spider):
        # Handle errors here
        self.logger.error(f"Error occurred within {spider.name} spider: {failure}")
        self.lighthouse.set_error_state(True)
        
    # Method that parses the request for a given url
    def parse(self, response):
        xlink = LinkExtractor(deny=self.excludes, unique=True)
        links = xlink.extract_links(response)
        
        # filter links to get only the search results
        potential_links = [link for link in links if link.url.startswith(self.url_query)]
        filtered_links = []
        #for link in potential_links:
        #    filtered_links.append(urlparse(urlparse(link.url).query.strip("q=")).hostname)
            
        list_of_links = links[:10] if len(potential_links) > 10 else potential_links
        self.logger.info(f"Scraped {len(list_of_links)} links from {response.url}")
        for link in list_of_links:
           # print(link.url)
            if not link.nofollow:
                print(link.url)
                # Create new request and send response to follow_link prior to pipeline
                yield scrapy.Request(link.url, callback=self.follow_link, 
                                     errback=self.on_error, cb_kwargs={'url'  : link.url,
                                                                       'text' : link.text,
                                                                       'fragment' : link.fragment,
                                                                       'nofollow' : link.nofollow})
            
    # follow_link:
    # Generates and yields LinkItem to LinkPipeline
    def follow_link(self, response, url, text, fragment, nofollow):
        link = LinkItem()
        print("followed: "+url)
        link['url'] = url
        link['text'] = text
        link['fragment'] = fragment
        link['nofollow'] = nofollow
        link['latency'] = response.meta.get('download_latency')
        yield link
        
    def on_error(self, failure):
        self.logger.error(repr(failure))
        self.lighthouse.set_error_state(True)
