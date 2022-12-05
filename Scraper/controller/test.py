
# This file is not a part of the controller and will be deleted eventually.
# It is being used solely for debugging.

def fetch_lighthouse_info(id):
    config1 = {'rows' : ["Pittsburgh Penguins"],
              'cols' : ["Wins","Losses"],
              'keywords' : [],
              'urls' : [
                        #'https://www.lme.com/en/Metals/EV/About-Lithium'  # No headers
                        'https://www.scrapethissite.com/pages/forms/'     # Normal
                        #'https://www.lme.com/en/Market-data/LME-reference-prices/LME-Official-Price' # Normal
                        #'https://www.gobankingrates.com/money/side-gigs/aluminum-can-prices/' # Duplicate headers
                       ],
              'script' : script,
              'log' : "lighthouse_1_log_1.txt",
              'log-backup-count': 10,
              'max-log-size': 1024,
              'allow-duplicates' : False
                }
    config2 = {'rows' : ["Pittsburgh Penguins"],
              'cols' : ["Wins","Losses"],
              'keywords' : [],
              'urls' : [
                        #'https://www.lme.com/en/Metals/EV/About-Lithium'  # No headers
                        'https://www.scrapethissite.com/pages/forms/'     # Normal
                        #'https://www.lme.com/en/Market-data/LME-reference-prices/LME-Official-Price' # Normal
                        #'https://www.gobankingrates.com/money/side-gigs/aluminum-can-prices/' # Duplicate headers
                       ],
              'script' : script,
              'log' : "lighthouse_1_log_3.txt",
              'log-backup-count': 10,
              'max-log-size': 1024,
              'allow-duplicates' : False
    }
    if int(id) == 1:
        return config1
    else:
        return config2
        


script = """
item = PriceItem()
for table in range(0, number_of_tables(response)):
    for col in self.config['cols']:
        for row in self.config['rows']:
            value = get_value_from_table(response, 1, col, row)
            trimmed_val = w3lib.html.remove_tags(value).strip()
            item['price'] = trimmed_val
            print(f'{row} {col}: {trimmed_val}')
            yield item
"""
