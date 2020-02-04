/// <reference path="tile.ts" />
/// <reference path="../models/eventData.ts" />
/// <reference path="../typings/moment.d.ts" />

class CalendarTile extends Tile
{
    private eventContainer: JQuery
    private tile: ITile;

    constructor(page: string, name: string, conn: signalR.HubConnection, haConn: HAConnection)
    {
        super(page, name, conn, haConn, { canLoad: true, canClick: false });

        this.eventContainer = $(`#tile-${name} div.calendar-events`);
    }

    public updateTile(t: ITile)
    {
        this.tile = t;
        super.updateTile(t);
    }

    public updateCalendar(state: IEntityState, events: ICalendarEvent[]): void
    {
        let label = state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel)
        {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);

        this.refreshEvents(events);

        super.updateCalendar();

        if (this.tile.refreshRate > 0)
        {
            setTimeout(() =>
            {
                this.requestState(1000);
            }, this.tile.refreshRate * 1000);
        }
    }

    private refreshEvents(events: ICalendarEvent[]): void
    {
        this.eventContainer.empty();

        if (!events.length)
        {
            this.eventContainer.append('<span class="no-events">No events!</span>');
        }
        else
        {
            let lastGroup = '';
            for (let i = 0; i < events.length; i++)
            {
                const evt = events[i];

                const thisGroup = this.getEventHeader(evt);
                if (lastGroup != thisGroup)
                {
                    // Write header
                    this.eventContainer.append(`<h3>${thisGroup}</h3>`);
                    lastGroup = thisGroup;
                }

                this.eventContainer.append(`<p><span class="summary">${evt.summary}</span><span class="time">${(!evt.start.dateTime ? 'All Day' : moment(evt.start.dateTime ?? evt.start.date).format('LT'))}</span></p>`);
            }
        }
    }

    private getEventHeader(event: ICalendarEvent): string
    {
        const today = moment();
        const tomorrow = moment().add(1, 'day');

        let todayHeader = this.formatHeader(today);
        let tomorrowHeader = this.formatHeader(tomorrow);

        const mt = moment(event.start.dateTime ?? event.start.date);

        let header = this.formatHeader(mt);
        if (header === todayHeader)
        {
            header += ' (Today)'
        }
        else if (header === tomorrowHeader)
        {
            header += ' (Tomorrow)'
        }
        return header;
    }

    private formatHeader(mt: moment.Moment): string
    {
        return mt.format('ddd') + ', ' + mt.format('ll');
    }
}