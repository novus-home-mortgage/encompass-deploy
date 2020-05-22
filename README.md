# encompass-deploy

[![Release Version](https://img.shields.io/github/release/novus-home-mortgage/encompass-deploy.svg)](https://github.com/novus-home-mortgage/encompass-deploy/releases/latest)
[![Build Status](https://dev.azure.com/novus-home-mortgage/Novus/_apis/build/status/encompass-deploy?branchName=master)](https://dev.azure.com/novus-home-mortgage/Novus/_build/latest?definitionId=5&branchName=master)

A command-line tool to help with automated deployment between environments of customizations to Ellie Mae's Encompass.

## Commands

`get-form`: Downloads a custom form (`.emfrm`) from Encompass

`import`: Uploads customization packages (`.empkg`) to an Encompass server

`pack`: Create a `.empkg` file based on a manifest.xml

`update-cdo`: Update a value inside of an XML Custom Data Object inside a package

`link-form`: Links a custom input form to the current version of a codebase assembly

You can run `encompass-deploy --help` for detailed arguments.

## Setup

The Encompass SmartClient will need to be installed on the host computer.

Download the [latest release](https://github.com/novus-home-mortgage/encompass-deploy/releases/latest) and extract the .zip to a folder of your choosing.

From your SmartClient folder (probably `C:\SmartClientCache\Apps\Ellie Mae\Encompass`), copy `EllieMae.Encompass.AsmResolver.dll` and `EllieMae.Encompass.Runtime.dll` into the folder where you extracted `encompass-deploy.exe`.
