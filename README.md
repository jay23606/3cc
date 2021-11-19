# 3cc
A command line interface to create/update 3commas bots along with some experimental settings
Instructions:

Create empty c# console project
Add files
Add XCommas.Net and Stock.Indicators nuget packages
Will need to create a secrets.txt file in debug/netcore* folder with:

API key (first line)

API secret (second line)

0 _or_ 1 (0 for Paper account and 1 for Real account - third line)

Exchange market code (e.g. paper_trading, binance_us, ftx_us, gdx, kucoin - fourth line)

email_token (retrieve from your 3c account in custom TV start condition JSON - fifth line)



This is a toy project so have fun with it
