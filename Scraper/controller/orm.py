#######################################
# orm.py                              #
#                                     #
# Wrapper for interacting with the DB #
#######################################

import pyodbc
import os
from .lighthouse import Lighthouse
from dotenv import load_dotenv
from importlib import invalidate_caches

class Orm:
    def __init__(self, server, database, username, password):
        self.server = server
        self.database = database
        self.username = username
        self.password = password
        
    def connection(self):        
        return pyodbc.connect("DRIVER={ODBC Driver 17 for SQL Server};SERVER=" +
            self.server + ";DATABASE=" + self.database + ";UID=" + self.username +
            ";PWD=" + self.password + ";Trusted_Connection=yes;")

    def get_lighthouse(self, lighthouse_id):
        items = []
        db_name = os.environ.get("DB_NAME")

        with self.connection() as connection:
            with connection.cursor() as lh_reader:
                lh_reader.execute((
                    "SELECT id, InternalName AS internal,"
                    "STRING_AGG(COLUMN_NAME, ',') AS col_names,"
                    "STRING_AGG(DATA_TYPE, ',') AS col_types FROM lighthouses "
                    "JOIN INFORMATION_SCHEMA.COLUMNS ON "
                    "table_name = lighthouses.InternalName + '_lighthouse' "
                    f"WHERE id = {lighthouse_id}"
                    "AND COLUMN_NAME LIKE 'Field_%' GROUP BY Id, InternalName"
                ))
                row = lh_reader.fetchone()
                types = {}
                for (name, ty) in zip(row.col_names.split(","), row.col_types.split(",")):
                    types[name[6:].lower()] = ty
                return Lighthouse(row.internal, row.id, types, self)
        
    def get_lighthouses(self):
        items = []
        db_name = os.environ.get("DB_NAME")

        with self.connection() as connection:
            with connection.cursor() as lh_reader:
                lh_reader.execute((
                    "SELECT id, InternalName AS internal,"
                    "STRING_AGG(COLUMN_NAME, ',') AS col_names,"
                    "STRING_AGG(DATA_TYPE, ',') AS col_types FROM lighthouses "
                    "JOIN INFORMATION_SCHEMA.COLUMNS ON "
                    "table_name = lighthouses.InternalName + '_lighthouse' "
                    "AND COLUMN_NAME LIKE 'Field_%' GROUP BY Id, InternalName"
                ))
                for row in lh_reader.fetchall():
                    types = {}
                    for (name, ty) in zip(row.col_names.split(","), row.col_types.split(",")):
                        types[name[6:].lower()] = ty
                    items.append(Lighthouse(row.internal, row.id, types, self))
        return items

def from_env():
    pyodbc.pooling = True
    load_dotenv()
    invalidate_caches()
    return Orm(
        os.environ.get("DB_HOST"),
        os.environ.get("DB_NAME"),
        os.environ.get("DB_USER"),
        os.environ.get("DB_PASSWORD")
    )
