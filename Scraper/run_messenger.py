import argparse
from controller.orm import from_env

parser = argparse.ArgumentParser(description='Script for running lighthouse messengers')

# Add positional argument for lighthouse ID
parser.add_argument('lighthouse_id', type=int, help='The ID of the lighthouse')

args = parser.parse_args()

# Retrieve the lighthouse
orm = from_env()
lh = orm.get_lighthouse(args.lighthouse_id)
print(lh.has_template)
