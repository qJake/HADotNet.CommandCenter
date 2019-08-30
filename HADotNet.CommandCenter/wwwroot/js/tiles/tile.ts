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

    constructor(protected name: string, protected conn: signalR.HubConnection, protected canLoad: boolean = true)
    {
        this.el = $(`.tiles .tile[data-tile-name="${name}"]`);

        if (canLoad)
        {
            this.el.click(() =>
            {
                this.onClick()
                    .then(() => Utils.delayPromise(500))
                    .then(() =>
                    {
                        this.requestState(1000);
                    });
            });
        }

        conn.on('SendSystemConfig', (tname, cfg) =>
        {
            if (name == tname)
            {
                this.config = cfg;
            }
        });
        conn.on('SendTileState', (t, s) =>
        {
            if (name == (t as ITile).name)
            {
                this.updateState(t, s);
            }
        });
        conn.on('SendTileStates', (t, s) =>
        {
            if (name == (t as ITile).name)
            {
                this.updateStates(t, s);
            }
        });
        conn.on('SendWarning', msg => console.warn(msg));
        conn.on('SendDateTime', (tile, d, t) =>
        {
            if (name == (tile as ITile).name)
            {
                this.updateState(tile, d, t);
            }
        });
        if (this.canLoad)
        {
            this.requestState();
        }
    }

    protected onClick(): Promise<any>
    {
        return this.conn.invoke("OnTileClicked", this.name);
    }

    protected updateState(tile?: ITile, ...args: any): void
    {
        this.disableLoading();
    }

    protected updateStates(tile?: ITile, ...args: any): void
    {
        this.disableLoading();
    }

    protected requestState(debounce?: number): void
    {
        this.enableLoading(debounce);
        this.conn.invoke('RequestTileState', this.name);
    }

    protected requestConfig(): void
    {
        this.conn.invoke('RequestConfig', this.name);
    }

    protected enableLoading(debounce?: number): void
    {
        if (this.el.hasClass("tile-loading") || this.loadingDebouncer)
        {
            return;
        }
        if (!debounce && !this.debounceTimeMs)
        {
            this.el.addClass("tile-loading");
        }
        else
        {
            this.loadingDebouncer = window.setTimeout(() =>
            {
                this.el.addClass("tile-loading");
            }, debounce || this.debounceTimeMs);
        }
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
}