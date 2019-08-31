# Home Assistant Command Center (HACC)

![release](https://img.shields.io/github/v/release/qjake/HADotNet.CommandCenter?color=%2300CC00&logo=github&sort=semver) ![ci-badge](https://github.com/qJake/HADotNet.CommandCenter/workflows/CI%20Build/badge.svg) ![maintain](https://img.shields.io/maintenance/yes/2019) ![lic](https://img.shields.io/github/license/qJake/HADotNet.CommandCenter?color=lightgray)

![issues](https://img.shields.io/github/issues-raw/qJake/HADotNet.CommandCenter?logo=github) ![prs](https://img.shields.io/github/issues-pr-raw/qjake/HADotNet.CommandCenter?logo=github)

![docker-pulls](https://img.shields.io/docker/pulls/qjake/hacc?logo=docker) ![docker-stars](https://img.shields.io/docker/stars/qjake/hacc?logo=docker)

A web-based, wall-mountable command center for Home Assistant.

![dashboard](https://raw.githubusercontent.com/qJake/HADotNet.CommandCenter/master/Assets/screenshot-dashboard.png)

*View more screenshots [#screenshots](below).*

# Features

* Point-and-click admin interface for configuring tiles, layout, themes, and more
* Drag and drop layout editor
* Dashboard updates via JavaScript (no page reloading)

## Tile Types

### Home Assistant

#### State

Display the state of any entity.

#### Switch and Light

Display a switch, group, or light with an icon. Tap to toggle it on/off.

#### Person

Display a user's current location (zone name) and profile picture, if set.

#### Camera

Display a camera feed from a connected compatible camera. Customize the way the feed is displayed on the dashboard.

#### Weather

Display current weather information (temperature, high/low, conditions, rain, and wind. (For best results, enable the [Dark Sky component](https://www.home-assistant.io/components/weather.darksky/) for use with this tile.)

### Coming Soon

* Support for multiple pages and navigation
* Support for additional HA entity types
* Additional weather attributes (humidity, forecast, etc)
* Theme browser

# Installing

HACC is an ASP.NET Core 2.2.x project. Therefore, it should be able to run on any environment that supports the ASP.NET 2.2 Runtime (Windows, Linux, and macOS).

## Via Docker

*Note: At this time, only Docker in Linux mode is supported. Docker in Windows mode is coming soon.*

A public Docker image is published for this project: [**qjake/hacc**](https://hub.docker.com/r/qjake/hacc)

```cmd
docker pull qjake/hacc
docker run -p 8095:8095 qjake/hacc
```

Your HACC instance will be available at: http://localhost:8095/

## Manually

Download, build, and publish the project. Then deploy it to a server or into IIS.

# Screenshots

## Editing Themes

![dashboard](https://raw.githubusercontent.com/qJake/HADotNet.CommandCenter/master/Assets/screenshot-theme-editor.png)

## Editing Tile Layout

![dashboard](https://raw.githubusercontent.com/qJake/HADotNet.CommandCenter/master/Assets/screenshot-layout.png)

## Browsing Tiles

![dashboard](https://raw.githubusercontent.com/qJake/HADotNet.CommandCenter/master/Assets/screenshot-tiles.png)

## New Tile Selection

![dashboard](https://raw.githubusercontent.com/qJake/HADotNet.CommandCenter/master/Assets/screenshot-new-tile.png)

# Credits

Proudly made with the following software:

* [Semantic UI](https://semantic-ui.com/)
* [Packery](https://packery.metafizzy.co/)
* [Material Design Icons](https://materialdesignicons.com/)
* [Font Awesome](https://fontawesome.com/)
