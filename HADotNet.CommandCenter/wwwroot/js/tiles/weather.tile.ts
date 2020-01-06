/// <reference path="tile.ts" />
/// <reference path="../models/skycons.d.ts" />

class WeatherTile extends Tile
{
    private skycons: Skycons;
    private iconEl: Element;
    private tile: IWeatherTile;

    private windSpeed = '';
    private windDir = '';
    private hi = '';
    private lo = '';

    constructor(protected page: string, protected name: string, protected conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, true);

        this.iconEl = this.el.find('.condition-icon')[0];

        const fontColor = document.defaultView.getComputedStyle(this.el[0]).color;

        this.skycons = new Skycons({ color: fontColor, resizeClear: true });
        this.skycons.add(this.iconEl, Skycons.CLEAR_DAY);
    }

    public updateTile(t: ITile)
    {
        this.tile = <IWeatherTile>t;
        super.updateTile(t);
    }

    public updateState(state: IHAStateChangedData): void
    {       
        let value = state.new_state == null ? null : state.new_state.state;
        switch (state.entity_id)
        {
            case this.tile.entityId:
                if (state.new_state.attributes["unit_of_measurement"])
                {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                $(`#tile-${this.tile.name}`).find('span[value-temp]').text(value);
                break;

            case this.tile.highTempEntity:
                if (state.new_state.attributes["unit_of_measurement"])
                {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                this.hi = `<i class="mdi mdi-arrow-up-thick"></i> ${value}`;
                break;

            case this.tile.lowTempEntity:
                if (state.new_state.attributes["unit_of_measurement"])
                {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                this.lo = `<i class="mdi mdi-arrow-down-thick"></i> ${value}`;
                break;

            case this.tile.summaryEntity:
                $(`#tile-${this.tile.name}`).find('span[value-summary]').text(value);
                break;

            case this.tile.precipChanceEntity:
                if (state.new_state.attributes["unit_of_measurement"])
                {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                $(`#tile-${this.tile.name}`).find('span[value-rain]').text(`Rain: ${value}`);
                break;

            case this.tile.windSpeedEntity:
                this.windSpeed = this.tile.roundWindSpeed ? parseInt(value).toString() : value;
                if (state.new_state.attributes["unit_of_measurement"])
                {
                    this.windSpeed += state.new_state.attributes["unit_of_measurement"].toString();
                }
                break;

            case this.tile.windDirectionEntity:
                this.windDir = Utils.convertDegreesToCardinal(parseInt(value));
                this.windDir = `<i class="mdi mdi-${Utils.convertCardinalToIcon(this.windDir)}"></i> ${this.windDir}`
                break;

            case this.tile.iconEntity:
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

        // Update the compound values
        $(`#tile-${this.tile.name}`).find('span[value-hi-lo]').html(`${(this.hi && this.lo ? this.hi + ' / ' + this.lo : this.hi + this.lo)}`);
        $(`#tile-${this.tile.name}`).find('span[value-wind]').html(`Wind: ${(this.windSpeed + ' ' + this.windDir).trim()}`);
    }
}