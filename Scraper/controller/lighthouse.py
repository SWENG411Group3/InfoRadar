######################################################
# lighthouse.py                                      #
#                                                    #
# Wrapper for interacting with lighthouses in the DB #
######################################################

import importlib
import logging
from datetime import datetime
from controller.template import Template

class Lighthouse:
    def __init__(self, internal_name, id, types, orm):
        self.internal_name = internal_name
        self.id = id
        self.types = types
        self._orm = orm
        self.logger = logging.getLogger(f"{self.internal_name}_lighthouse")
        self.scriptmod = importlib.import_module("scripts." + internal_name + "_lighthouse", package="Scraper")
        try:
            self.templatemod = importlib.import_module("scripts.templates." + internal_name + "_lighthouse", package="Scraper")
            self.has_template = True
            self.template = self.get_template_config()
        except ModuleNotFoundError as err:
            self.has_template = False
            self.logger.info(f"No templates found for {self.internal_name} lighthouse")
        
    # gets all script functions annotated with "visitor" decoration
    def get_script_visitors(self):
        return self._get_scripts(self.scriptmod, 'script_job', 'visitor')
        
    # gets all script functions decorated with "messenger"
    def get_script_messengers(self):
        return self._get_scripts(self.scriptmod, 'script_job', 'messenger')
    
    # gets all script functions decorated with "pipeline"
    def get_script_pipelines(self):
        return self._get_scripts(self.scriptmod, 'script_job', 'pipeline')
    
        # gets all template functions annotated with "visitor" decoration
    def get_template_visitors(self):
        return self._get_scripts(self.templatemod, 'script_job', 'visitor') if self.has_template else None
        
    # gets all template functions decorated with "messenger"
    def get_template_messengers(self):
        return self._get_scripts(self.templatemod, 'script_job', 'messenger') if self.has_template else None
    
    # gets all template functions decorated with "pipeline"
    def get_template_pipelines(self):
        return self._get_scripts(self.templatemod, 'script_job', 'pipeline') if self.has_template else None
    
    def _get_scripts(self, mod, attr, name):
        return [fnc for _, fnc in mod.__dict__.items() if callable(fnc) and getattr(fnc, attr, "") == name]
    
    def set_log_index(self, ind):
        query = """UPDATE Lighthouses
                   SET LatestLog = ?
                   WHERE Id = ?"""
        self._execute_query(query, [ind, self.id])
    
    def set_error_state(self, errorState):
        query = """UPDATE Lighthouses
                   SET HasError = ?
                   WHERE Id = ?"""
        self._execute_query(query, [int(errorState), self.id])
                
    def get_urls(self):
        search_results = self._get_search_results()
        sites = self._get_sites()
        return search_results + sites if search_results is not None else sites
    
    def update_search_results(self, url):
        query = """INSERT INTO SearchResults (LighthouseId, Url)
                   VALUES (?, ?)"""
        self._execute_query(query, [self.id, url])
        self.logger.info(f"Added to Search Results: {url}")
        
    def get_google_queries(self):
        query = "SELECT Query FROM GoogleQueries WHERE LighthouseId = ?"
        google_queries = self._execute_query(query, [self.id], fetch=True)
        return [query[0] for query in google_queries]
        
    def notify_running(self, state):
        query = "UPDATE Lighthouses SET Running = ?"
        self._execute_query(query, [int(state)])
        
    def set_last_run(self,utc):
        query = """UPDATE Lighthouses
                   SET LastVisitorRun = ?
                   WHERE Id = ?"""
        self._execute_query(query, [datetime.utcnow(), self.id])
        
    # Retrieves the template configuration for this lighthouse.
    # This method assumes that the has_template property is True
    # and will set the error state to True if no template config is found.
    def get_template_config(self):
        query = """SELECT TemplateId, Payload
                   FROM TemplateConfigurations
                   WHERE LighthouseId = ?"""
        template = self._execute_query(query, [self.id], fetch=True)
        if template is not None:
            return Template(template[0][0], template[0][1])
        else:
            self.logger.error(f"Unable to load template configuration for lighthouse: {self.internal_name}, id: {self.id}")
            self.set_error_state(True)
    
    def _get_search_results(self):
        query = "SELECT Url FROM SearchResults WHERE LighthouseId = ?"
        results = self._execute_query(query, [self.id], fetch=True)
        if results is not None:            
            return [url[0] for url in results]
            
    def _get_sites(self):
        query = "SELECT Url FROM Sites WHERE LighthouseId = ?"
        sites = self._execute_query(query, [self.id], fetch=True)
        return [site[0] for site in sites]
    
    def _execute_query(self, query, params, fetch=False):
        with self._orm.connection() as connection:
            with connection.cursor() as lh_writer:
                try:
                    lh_writer.execute(query, params)
                    if fetch:
                        return lh_writer.fetchall()
                except Exception as err:
                    self.logger.error(f"{err}\t{query}")
                    self.update_error_state(True)
                    return False
        return True
                