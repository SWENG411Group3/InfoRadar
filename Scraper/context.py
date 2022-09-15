###############################################################
# context.py                                                  #
#                                                             #
# Wrapper around lighthouse ORM object                        #
#                                                             #
# A context object is passed into user-defined functions so   #
# the script can easily gather and manipulate lighthouse data #
#                                                             #
# The context is used by the user scripts; the lighthouse is  #
# used by the scraper                                         #
###############################################################
 
class Context:
    def __init__(self, lighthouse):
        self.lighthouse = lighthouse

    # Fetches all URLS stored in the DB associated with this lighthouse
    # and returns their bodies depending on the expected data type in the 
    # DB.  This method should return either a list of "Site" objects
    # or a key value pairs tying a URL to its corresponding Site object
    # Use whichever one makes more sense for this app
    # (scaffold definition of Site below)
    def fetch_sites(self):
        pass

    # Fetches all stored Google queries from DB associated with and runs them
    # results are returned in a list in the order they were inserted 
    # into the DB 
    def google_queries(self):
        pass

    # inserts a record if record corresponds to lighthouse 
    # This record should conform to the lighthouse's type standards
    # if it doesn't throw an error
    def insert_record(self, record):
        pass

    # gets all records that were created after the most recent message was sent
    def unread_records(self):
        pass

    # gets all records within a specified range
    def record_range(self, start, end):
        pass
        
    # gets all records that were created within the last N seconds 
    def recent_reords(self, seconds):
        pass

# Object representing the data scraped from a particular URL
# This object should convey what kind of data was scraped, the actual data,
# and the URL was taken from
class Site:
    def __init__(self):
        self.type = None
        self.url = None
