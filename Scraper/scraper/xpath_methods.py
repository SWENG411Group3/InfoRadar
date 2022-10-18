# This module contains various helper methods to be used within templates/scripts

def contains_keyword(response, keyword):
        return len(response.xpath(f'//text()[contains(., "{keyword}")]').getall()) > 0
    
def has_element(response, element):
    return len(response.xpath(f'//{element}')) > 0
    
def number_of_elements(response, element):
    return int(float(response.xpath(f'count(//{element})').get()))
    
def number_of_tables(response):
    return len(response.xpath('//table'))
    
def number_of_rows(response, table):
    return len(response.xpath(f'//table[{table}]//tr'))
    
def number_of_headers(response, table):
    return len(response.xpath(f'//table[{table}]//th'))
    
def get_header_index(response, table, name):    
    headers = response.xpath(f'//table[{table}]//th')
    if len(headers) == 0:
        # No table header, try the first row instead
        headers = response.xpath(f'//table[{table}]//tr[1]//td')
        print(headers.get())
    for header in headers:
        if name in header.get():
            return headers.index(header)+1
    return -1
    
def get_value_from_table(response, table, column, row):
    if isinstance(column, str):
        column = get_header_index(response, table, column)
    return response.xpath(f'//table//tr[contains(., "{row}")]//td[{column}]').get()
