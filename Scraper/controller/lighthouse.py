######################################################
# lighthouse.py                                      #
#                                                    #
# Wrapper for interacting with lighthouses in the DB #
######################################################

from .context import Context
import importlib

class Lighthouse:
    def __init__(self, internal_name, id, types, orm):
        self.internal_name = internal_name
        self.id = id
        self.types = types
        self._orm = orm
        self.mod = importlib.import_module("scripts." + internal_name + "_lighthouse",package="Scraper")

    # gets all functions annotated with "visitor" decoration
    def get_visitors(self):
        return [fnc for _, fnc in self.mod.__dict__.items() if callable(fnc) and getattr(fnc, "script_job", "") == "visitor"]
        

    # gets all functions decorated with "messenger"
    def get_messengers(self):
        return [fnc for _, fnc in self.mod.__dict__.items() if callable(fnc) and getattr(fnc, "script_job", "") == "messenger"]
    
    # gets all functions decorated with "pipeline"
    def get_pipelines(self):
        return [fnc for _, fnc in self.mod.__dict__.items() if callable(fnc) and getattr(fnc, "script_job", "") == "pipeline"]

    # Invokes all visitors if last crawl date was over `Frequency` seconds
    # Invokes all messengers if last message was sent over `MessageFrequency` seconds
    def run_jobs(self):
        context = Context(self)

        if False: # @TODO check if visitors can run based on timing
            for fnc in self.get_visitors():
                fnc(context)
            # @TODO update last run date

        if False: # @TODO check if messengers can run based on timing
            for fnc in self.get_messengers():
                fnc(context)
            # @TODO update last message date
    
    def update_log_index(self, ind):
        with self._orm.connection() as connection:
            with connection.cursor() as lh_writer:
                lh_writer.execute(("UPDATE Lighthouses "    
                                   "SET LatestLog = {ind} "
                                   f"WHERE Id = {self.id}"))
    
    def update_error_state(self, errorState):
        with self._orm.connection() as connection:
            with connection.cursor() as lh_writer:
                lh_writer.execute(("UPDATE Lighthouses "    
                                   f"SET HasError = {int(errorState)} "
                                   f"WHERE Id = {self.id}"))
                
    def get_urls(self):
        pass
    # Query google queries table, join GoogleQueries.LighthouseId = self.id
    # Query sites tables, 
    
    