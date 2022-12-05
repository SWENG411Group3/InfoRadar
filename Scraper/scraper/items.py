# Define here the models for your scraped items
#
# See documentation in:
# https://docs.scrapy.org/en/latest/topics/items.html

import scrapy

# A generic lighthouse item consisting of a description and a value
class LighthouseItem(scrapy.Item):
    value = scrapy.Field()
    description = scrapy.Field()
    
# A "Price" item consisting of a description and a price
class PriceItem(scrapy.Item):
    price = scrapy.Field()
    description = scrapy.Field()
    
# A link item to use when extracting links from google
class LinkItem(scrapy.Item):
    url = scrapy.Field()
    text = scrapy.Field()
    fragment = scrapy.Field()
    nofollow = scrapy.Field()
    