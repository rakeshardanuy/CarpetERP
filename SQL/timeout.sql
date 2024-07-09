exec sp_updatestats

dbcc freeproccache


EXEC SP_CONFIGURE 'remote query timeout', 1800
reconfigure
EXEC sp_configure

EXEC SP_CONFIGURE 'show advanced options', 1
reconfigure
EXEC sp_configure

EXEC SP_CONFIGURE 'remote query timeout', 1800
reconfigure
EXEC sp_configure