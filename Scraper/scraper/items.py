# Define here the models for your scraped items
#
# See documentation in:
# https://docs.scrapy.org/en/latest/topics/items.html

import scrapy

# A "Price" item consisting of a description and a price
class PriceItem(scrapy.Item):
    price = scrapy.Field()
    description = scrapy.Field()
    