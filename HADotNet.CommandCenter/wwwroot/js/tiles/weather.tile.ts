/// <reference path="tile.ts" />
/// <reference path="../models/skycons.d.ts" />

class WeatherTile extends Tile
{
    private skycons: Skycons;
    private iconEl: Element;

    constructor(protected name: string, protected conn: signalR.HubConnection, protected canLoad: boolean = true)
    {
        super(name, conn, canLoad);

        this.iconEl = this.el.find('.condition-icon')[0];

        const fontColor = document.defaultView.getComputedStyle(this.el[0]).color;

        this.skycons = new Skycons({ color: fontColor, resizeClear: true });
        this.skycons.add(this.iconEl, Skycons.CLEAR_DAY);
    }

    public updateStates(tile: ITile, states: StateDictionary): void
    {
        // Some are combination variables
        let windSpeed = '';
        let windDir = '';
        let hi = '';
        let lo = '';

        for (let state in states)
        {
            let value = states[state] == null ? null : states[state].state;
            switch (state)
            {
                case WeatherTileEntities.entityId:
                    if (states[state].attributes["unit_of_measurement"])
                    {
                        value += states[state].attributes["unit_of_measurement"].toString();
                    }
                    $(`#tile-${tile.name}`).find('span[value-temp]').text(value);
                    break;

                case WeatherTileEntities.highTempEntity:
                    if (states[state].attributes["unit_of_measurement"])
                    {
                        value += states[state].attributes["unit_of_measurement"].toString();
                    }
                    hi = `<i class="mdi mdi-arrow-up-thick"></i> ${value}`;
                    break;

                case WeatherTileEntities.lowTempEntity:
                    if (states[state].attributes["unit_of_measurement"])
                    {
                        value += states[state].attributes["unit_of_measurement"].toString();
                    }
                    lo = `<i class="mdi mdi-arrow-down-thick"></i> ${value}`;
                    break;

                case WeatherTileEntities.summaryEntity:
                    $(`#tile-${tile.name}`).find('span[value-summary]').text(value);
                    break;

                case WeatherTileEntities.precipChanceEntity:
                    if (states[state].attributes["unit_of_measurement"])
                    {
                        value += states[state].attributes["unit_of_measurement"].toString();
                    }
                    $(`#tile-${tile.name}`).find('span[value-rain]').text(`Rain: ${value}`);
                    break;

                case WeatherTileEntities.windSpeedEntity:
                    if (states[state].attributes["unit_of_measurement"])
                    {
                        value += states[state].attributes["unit_of_measurement"].toString();
                    }
                    windSpeed = value;
                    break;

                case WeatherTileEntities.windDirectionEntity:
                    windDir = Utils.convertDegreesToCardinal(parseInt(value));
                    windDir = `<i class="mdi mdi-${Utils.convertCardinalToIcon(windDir)}"></i> ${windDir}`
                    break;

                case WeatherTileEntities.iconEntity:
                    if (value)
                    {
                        this.skycons.set(this.iconEl, value);
                        this.skycons.play();
                    }
                    else
                    {
                        this.skycons.remove(this.iconEl);
                        $(this.iconEl).hide();
                    }
                    break;
            }
        }

        // Update the compound values
        $(`#tile-${tile.name}`).find('span[value-hi-lo]').html(`${(hi && lo ? hi + ' / ' + lo : hi + lo)}`);
        $(`#tile-${tile.name}`).find('span[value-wind]').html(`Wind: ${(windSpeed + ' ' + windDir).trim()}`);
            
        super.updateState();

        if (tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState(2000);
            }, tile.refreshRate * 1000);
        }
    }
}