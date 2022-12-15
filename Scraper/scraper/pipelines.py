# Define your item pipelines here
#
# Don't forget to add your pipeline to the ITEM_PIPELINES setting
# See: https://docs.scrapy.org/en/latest/topics/item-pipeline.html

# useful for handling different item types with a single interface
import json
from itemadapter import ItemAdapter
from scrapy.exceptions import DropItem
import logging
from scraper.items import *

class LighthouseItemPipeline:
    saved_items = []
    def process_item(self, item, spider):
        if isinstance(item, LighthouseItem):
            if spider.lighthouse.has_template:
                for pipeline in spider.lighthouse.get_template_pipelines():
                    self.saved_items.append(pipeline(item, spider.lighthouse.logger))
            else:
                for pipeline in spider.lighthouse.get_script_pipelines():
                    self.saved_items.append(pipeline(item, spider.lighthouse.logger))
    
    def open_spider(self, spider):
        if spider.name == "Lighthouse Spider":
            self.saved_items.clear()
            
    def close_spider(self, spider):
        if spider.name == "Lighthouse Spider":
            filtered_items = list(filter(lambda item: item is not None, self.saved_items))
            for item in filtered_items:
                adapter = ItemAdapter(item)
                value = adapter.get('value')
                if isinstance(value, list):
                    for row in value:
                        spider.lighthouse.insert_row(row)
                else:
                    #TODO: extend functionality to handle various types of user-created items 
                    pass
                
class LinkPipeline:
    MAX_LATENCY = 1.5
    saved_links = []
    def process_item(self, item, spider):
        if isinstance(item, LinkItem):
            adapter = ItemAdapter(item)
            # process the google search result link here
            # any links that will be saved need to be appended
            # to the links list
            latency = adapter.get('latency')
            if latency < self.MAX_LATENCY:
                self.saved_links.append(adapter.get('url'))
        return item

    def open_spider(self, spider):
        if spider.name == "Google Spider":
            self.saved_links.clear()

    def close_spider(self, spider):
        if spider.name == "Google Spider":
            if len(self.saved_links) > 0:
                spider.logger.info(f"{len(self.saved_links)} links passed latency requirement")
            else:
                spider.logger.info("Either no links passed the latency requirement or there were no Google queries specified for this lighthouse")
            for link in self.saved_links:
                spider.lighthouse.update_search_results(link)
