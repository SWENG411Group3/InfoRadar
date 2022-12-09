######################################################
# lighthouse.py                                      #
#                                                    #
# Wrapper for interacting with lighthouses in the DB #
######################################################

from .context import Context
import importlib
import logging

class Lighthouse:
    def __init__(self, internal_name, id, types, orm):
        self.internal_name = internal_name
        self.id = id
        self.types = types
        self._orm = orm
        self.mod = importlib.import_module("scripts." + internal_name + "_lighthouse", package="Scraper")
        self.logger = logging.getLogger(f"{self.internal_name}_lighthouse")
        
    # gets all functions annotated with "visitor" decoration
    def get_visitors(self):
        return [fnc for _, fnc in self.mod.__dict__.items() if callable(fnc) and getattr(fnc, "script_job", "") == "visitor"]

    # gets all functions decorated with "messenger"
    def get_messengers(self):
        return [fnc for _, fnc in self.mod.__dict__.items() if callable(fnc) and getattr(fnc, "script_job", "") == "messenger"]
    
    # gets all functions decorated with "pipeline"
    def get_pipelines(self):
        return [fnc for _, fnc in self.mod.__dict__.items() if callable(fnc) and getattr(fnc, "script_job", "") == "pipeline"]
    
    def set_log_index(self, ind):
        with self._orm.connection() as connection:
            with connection.cursor() as lh_writer:
                lh_writer.execute(("UPDATE Lighthouses "    
                                   "SET LatestLog = {ind} "
                                   f"WHERE Id = {self.id}"))
    
    def set_error_state(self, errorState):
        with self._orm.connection() as connection:
            with connection.cursor() as lh_writer:
                lh_writer.execute(("UPDATE Lighthouses "    
                                   f"SET HasError = {int(errorState)} "
                                   f"WHERE Id = {self.id}"))
                
    def get_urls(self):
        search_results = self._get_search_results()
        sites = self._get_sites()
        return search_results + sites if search_results is not None else sites
    
    def update_search_results(self, url):
        query = ("INSERT INTO SearchResults (LighthouseId, Url)"
                 f"VALUES ({self.id}, '{url}')")
        if self._execute_query(query):
            self.logger.info(f"Added to Search Results: {url}")
        
    def get_google_queries(self):
        query = f"SELECT Query FROM GoogleQueries WHERE LighthouseId = {self.id}"
        google_queries = self._execute_query(query, fetch=True)
        return [query[0] for query in google_queries]
        
    def notify_running(self, state):
        query = f"UPDATE Lighthouses SET Running = {int(state)}"
        
    def _get_search_results(self):
        query = f"SELECT Url FROM SearchResults WHERE LighthouseId = {self.id}"
        results = self._execute_query(query, fetch=True)
        if results is not None:            
            return [url[0] for url in results]
            
    def _get_sites(self):
        query = f"SELECT Url FROM Sites WHERE LighthouseId = {self.id}"
        sites = self._execute_query(query, fetch=True)
        return [site[0] for site in sites]
    
    def _execute_query(self, query, fetch=False):
        with self._orm.connection() as connection:
            with connection.cursor() as lh_writer:
                try:
                    lh_writer.execute(query)
                    if fetch:
                        return lh_writer.fetchall()
                except Exception as err:
                    self.logger.error(f"{err}\t{query}")
                    self.update_error_state(True)
                    return False
        return True
                