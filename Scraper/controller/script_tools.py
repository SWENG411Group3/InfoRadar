##############################################################
# script_tools.py                                            #
#                                                            #
# Contains functions used in user-defined lighthouse scripts #
# Lighthouse scripts should include this module so that they #
# can classify functions appropriately                       #
##############################################################

# Python decorator for designating visitor functions
def visitor(fnc):
    fnc.script_job = "visitor"
    return fnc

# Python decorator for designating messenger functions
def messenger(fnc):
    fnc.script_job = "messenger"
    return fnc

# Python decorator for designating pipeline functions
def pipeline(fnc):
    fnc.script_job = "pipeline"
    return fnc
