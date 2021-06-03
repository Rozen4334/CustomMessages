# CustomMessages

A plugin for TShock for Terraria. This plugin was originally created by @nyan-ko, and upgraded to be available for public use by me.

## Commands & Permissions:

'custommessages.add' - /addcommand
'custommessages.reload' - /reloadmsgs

## How to use:

A command is created by reading the 'commands.json' file in \tshock. This file will add commands with the "addcommand" command, and it is reloaded with the "reloadmsgs" command.

A custom command is formed like this: /addcommand command "response with quotes so it takes the entire message and pushes it to the config".

You can edit/add commands in config. Format them like this:

```{
  "command": "response with spaces",
  "next command": "response with spaces and hex codes for custom coloring"
}```

Exclusively code commands in this format, never forget to add a ',' to every line excluding the bottom one!
