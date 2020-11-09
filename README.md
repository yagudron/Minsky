# Minsky
Simple bot for a DCS-themed Discord community.<br />
Helps users find out server DCS and SRS servers info and status.<br />
## Commands
### General info
- `?server`- gets server name and address
- `?status` - return status of SRS and DCS servers.
### Commands available to users with Dev Staff role (DevStaffRoleId is specified in the config)
- `?start` - start server
- `?restart` - restart server
- `?stop` - stop server.

Can be run as a windows service but the admin commands should be disabled as they won't work if the bot runs without UI.</br>
Built using .NET Core Background Worker and [Discord.Net](https://github.com/discord-net/Discord.Net).
