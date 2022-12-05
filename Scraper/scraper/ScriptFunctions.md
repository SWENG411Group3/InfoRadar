
# Writing Custom Lighthouse Scripts

## When writing scripts for a custom lighthouse, a number of built-in classes and functions are available.

### Built-in Functions

| Function                            | Description                                                            | 
|-------------------------------------|------------------------------------------------------------------------|
| [contains_keyword()](#cont-keyword) | Returns true if at least 1 occurrence of "keyword" is found            |
| [has_element](#has-el)              | Returns true if "element" is present in response                       |
| [number_of_elements](#num-elements) | Returns the number of occurrences for a specific element               |
| [number_of_tables](#num-tables)     | Returns the total number of tables found in the response               |

### Built-in Classes
| Class                               | Description                                                            | 
|-------------------------------------|------------------------------------------------------------------------|
| [Table](#table)                     | Provides basic functionality for working with HTML tables              |

### <h3 id="cont-keyword"></h3>

` contains_keyword(response, keyword) `

Parameters:
- **response** (scrapy.http.Response) - the HTTP response available for processing
- **keyword** (str) - the keyword to search for

`Returns` a boolean value indicating whether the specified keyword was found in the HTTP response

---

### <h3 id="has-el"></h3>

` has_element(response, element) `

Parameters:
- **response** (scrapy.http.Response) - the HTTP response available for processing
- **element** (str) - the element to search for

`Returns` a boolean value indicating whether the specified element was found in the HTTP response

---

### <h3 id="num-elements"></h3>

` number_of_elements(response, keyword) `

Parameters:
- **response** (scrapy.http.Response) - the HTTP response available for processing
- **keyword** (str) - the keyword to search for

`Returns` a boolean value indicating whether the specified keyword was found in the HTTP response

---

### <h3 id="num-tables"></h3>

` number_of_tables(response) `

Parameters:
- **response** (scrapy.http.Response) - the HTTP response available for processing
- **keyword** (str) - the keyword to search for

`Returns` a boolean value indicating whether the specified keyword was found in the HTTP response

---

### <h3 id="table">Table</h3>

` Table `

Parameters:


> :memo: **Note:** Sunrises are beautiful.