/// <reference path="../../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />
/// <reference path="../utils.ts" />

abstract class Tile
{
    protected el: JQuery<HTMLElement>;

    private loadingDebouncer: number;

    /**
     * Gets or sets the debounce time (in ms) for the loading animation on this tile.
     */
    protected debounceTimeMs: number;

    protected config: IHaccConfig;

    protected entityIds: string[];

    public loaded: boolean;

    constructor(protected page: string, protected name: string, protected conn: signalR.HubConnection, haConn: HAConnection, protected canLoad: boolean = true)
    {
        this.entityIds = [];
        this.loaded = !canLoad;
        this.el = $(`.tiles .tile[data-tile-name="${name}"]`);

        if (canLoad)
        {
            this.el.click(() =>
            {
                this.onClick();
            });
        }

        var entityList = this.el.data('tile-entityid');
        if (typeof entityList === 'object' && Array.isArray(entityList))
        {
            this.entityIds = entityList;
        }
        else
        {
            this.entityIds.push(entityList?.toString());
        }

        conn.on('SendSystemConfig', (tname, cfg) =>
        {
            if (name == tname)
            {
                console.debug(`Received "SendSystemConfig" for tile: ${tname}`);
                this.config = cfg;

                // New config = re-request state
                if (this.canLoad)
                {
                    //why?
                    //this.requestState();
                }
            }
        });
        conn.on('SendTile', t =>
        {
            if (name == (t as ITile).name)
            {
                console.debug(`Received "SendTile" for tile: ${(t as ITile).name}`);
                this.updateTile(t);
            }
        });
        conn.on('SendCalendarInfo', (t, s, e) =>
        {
            if (name == (t as ITile).name)
            {
                //console.debug(`Received: "SendCalendarInfo" for tile: ${(t as ITile).name}`);
                this.updateCalendar(s, e);
            }
        });
        conn.on('SendWarning', msg => console.warn(msg));
        conn.on('SendDateTime', (tile, d, t) =>
        {
            if (name == (tile as ITile).name)
            {
                //console.debug(`Received: "SendDateTime" for tile: ${(tile as ITile).name}`);
                this.updateDateTime(tile, d, t);
            }
        });

        this.requestConfig(page);

        if (this.canLoad)
        {
            this.requestState();
        }
    }

    protected onClick(): Promise<any>
    {
        return this.conn.invoke('OnTileClicked', this.page, this.name);
    }

    protected updateTile(tile?: ITile)
    {
        this.loaded = true;
        this.disableLoading();
    }

    public updateState(state?: IHAStateChangedData): void { }

    protected updateCalendar(state?: IEntityState, events?: ICalendarEvent[]): void { }

    protected updateDateTime(tile?: ITile, ...args: any): void { }

    protected requestState(debounce?: number): void
    {
        this.conn.invoke('RequestTileState', this.page, this.name);
    }

    protected requestConfig(page: string): void
    {
        this.conn.invoke('RequestConfig', page, this.name);
    }

    protected disableLoading(): void
    {
        if (this.loadingDebouncer)
        {
            window.clearTimeout(this.loadingDebouncer);
        }
        this.loadingDebouncer = null;
        this.el.removeClass("tile-loading");
    }

    public getEntityIds(): string[]
    {
        return this.entityIds;
    }
}