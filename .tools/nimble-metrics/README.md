# NimbleMetrics

This directory holds the repository-local configuration for
[NimbleMetrics](https://docs.nimbletools.io), a tool that measures C# code complexity,
change risk (CRAP), build/test timing, and code "concerns" over your repository's
history so you can find and track the riskiest code.

Learn more at https://docs.nimbletools.io.

## Contents

- `settings.json` — the settings the `nm` CLI reads. It ships with
  the default configuration; edit it to enable history rebuild, concern collection, and
  other options. A `settings.toml` placed here overrides values in this file.
