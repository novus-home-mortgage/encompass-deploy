# encompass-deploy

[![Release Version](https://img.shields.io/github/release/panorama-mortgage-group/encompass-deploy.svg)](https://github.com/panorama-mortgage-group/encompass-deploy/releases/latest)
[![Build Status](https://dev.azure.com/panorama-mortgage-group/Panorama%20Apps/_apis/build/status/encompass-deploy?branchName=master)](https://dev.azure.com/panorama-mortgage-group/Panorama%20Apps/_build/latest?definitionId=5&branchName=master)

A command-line tool to help with automated deployment between environments of customizations to Ellie Mae's Encompass.

## Commands

`get-form`: Downloads a custom form (`.emfrm`) from Encompass

`import`: Uploads customization packages (`.empkg`) to an Encompass server

`pack`: Create a `.empkg` file based on a manifest.xml

You can run `encompass-deploy --help` for detailed arguments.

The Encompass SmartClient or SDK will need to be installed on the host computer.