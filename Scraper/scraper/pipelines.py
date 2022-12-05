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


class LinkPipeline:
    saved_links = []
    def process_item(self, item, spider):
        if isinstance(item, LinkItem):
            adapter = ItemAdapter(item)
            # process the google search result link here
            # any links that will be saved need to be appended
            # to the links list
            self.saved_links.append(item['url'])
        return item
    
    def close_spider(self, spider):
        if spider.name == "Google":
            for link in self.saved_links:
                logging.info(link)

class PricePipeline:
    def process_item(self, item, spider):
        if isinstance(item, PriceItem):
            adapter = ItemAdapter(item)
            if (adapter.get('price')):
                logging.info("Scraped price for {}: {}".format(adapter.get('description'), adapter.get('price')))
                # Here we can reach out to the controller and check any thresholds
                # for now just return            
        return item
    
class JsonWriterPipeline:
    def open_spider(self, spider):
        self.file = open('items.jsonl', 'w')

    def close_spider(self, spider):
        self.file.close()

    def process_item(self, item, spider):
        if item is not None:
            line = json.dumps(ItemAdapter(item).asdict()) + "\n"
            self.file.write(line)
            return item
    