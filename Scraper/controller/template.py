import json
import logging

class Template:
    def __init__(self, id, payload):
        self.id = id
        self.logger = logging.getLogger("template_loader")
        try:
            self.payload = json.loads(payload)
        except Exception as err:
            self.logger.error(f"Error retrieving JSON payload\t{err}")
