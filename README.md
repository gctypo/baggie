# Baggie

*D'awwww Baggie Waggie being all UwU and OwO on chat?
D'awwww wook he's so embawassed uWu Does Baggie Waggie need a
huggie wuggie poo? What about a kith? Baggie Waggie wan a kith?
UwU ehe te nandayo~*

-----

The infamous `!baggie` Twitch command stumbles its way onto Discord.

Written in F# because I've never done FP before and this seems like a place to start.

## Usage

Add bot to server, run `!baggie` in chat, and the copypasta will be summoned!

Required permissions: `3072`
* *Read Messages/View Channels* `1024`
* *Send Messages* `2048`

Privileged Intents:
* ***Message Content***

## Configuration

Has a configurable timeout per-server to prevent spamming. Configured with the `baggie.timeoutSec` config. Fallback value of 60 seconds.

Bot login token path configured with `discord.tokenpath` config. Expects plain-text file at that path containing discord bot login token.

-----

## FAQ

### #1: Why?

You know this had to be done.
