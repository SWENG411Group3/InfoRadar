:: CLI args:
:: 1. Anaconda path
:: 2. Lighthouse id
:: 3. Number of sites

call "batch_scripts\header.bat" %1
if %3 == 0 (echo No queries to run) else (scrapy crawl "Google Spider" -a lighthouse_id=%2)
scrapy crawl "Lighthouse Spider" -a lighthouse_id=%2