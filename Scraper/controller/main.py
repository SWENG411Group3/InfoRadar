import importlib
import orm
from dotenv import load_dotenv

load_dotenv()
importlib.invalidate_caches()

db = orm.from_env()

print("Listing lighthouses:")

lh = db.get_lighthouses()

# print(len(lighthouses))
# for lighthouse in lighthouses:
#     print("--------------------")
#     print("Name:", lighthouse.internal_name)
#     print("Id:", lighthouse.id)
#     print("Types:", lighthouse.types)
#     print("Visitors:", lighthouse.get_visitors())
#     print("Messengers:", lighthouse.get_messengers())
