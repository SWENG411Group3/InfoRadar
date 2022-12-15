# This module contains various helper classes / methods to be used within templates/scripts
import w3lib.html

def contains_keyword(response, keyword):
        return len(response.xpath(f'//text()[contains(., "{keyword}")]').getall()) > 0
    
def has_element(response, element):
    return len(response.xpath(f'//{element}')) > 0
    
def number_of_elements(response, element):
    return int(float(response.xpath(f'count(//{element})').get()))
    
def number_of_tables(response):
    return len(response.xpath('//table'))
    
class Table:
    def __init__(self, response, table_num):
        self._response = response
        self._table_num = table_num
        
    def number_of_rows(self):
        return len(self._response.xpath(f'//table[{self._table_num}]//tr'))
    
    def get_headers(self):
        return self._response.xpath(f'//table[{self._table_num}]//th')
    
    def _get_header_index(self, name):    
        headers = self.get_headers()
        if len(headers) == 0:
            # No table header, try the first row instead
            headers = self._response.xpath(f'//table[{self._table_num}]//tr[1]//td')
        for header in headers:
            if name in header.get():
                return headers.index(header)+1
        return -1
    
    def get_value(self, column, row, remove_tags=True):
        """get_value: Attempts to retrieve a value from the table at the 
           specified column and row.
        Args:
            column (str/int): The column header text OR the column number
            row (str/int): The row text OR the row number
            remove_tags (bool): Flag that specifies whether the return value should be stripped 
            of HTML tags. Default is True.
        Returns:
            str: The value if it is found, otherwise None
        """
        if isinstance(column, str):
            column = self._get_header_index(column)
        if column is not None:
            val = self._response.xpath(f'//table//tr[contains(., "{row}")]//td[{column}]').get()
            return w3lib.html.remove_tags(val).strip() if remove_tags else val
        else:
            return None

    
    