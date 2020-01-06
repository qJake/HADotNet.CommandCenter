/// <reference path="typings/reconnecting-websocket.d.ts" />
/// <reference path="models/home-assistant-ws.ts" />

/** Defines the current state of the HA connection. */
enum HAConnectionState
{
    Closed,
    Opening,
    Auth,
    Open
}

// Simple event interface based on: https://gist.github.com/JasonKleban/50cee44960c225ac1993c922563aa540#file-liteevent-ts
interface IConnectionEvent<T>
{
    on(handler: { (data?: T): void }): void;
    off(handler: { (data?: T): void }): void;
}

class ConnectionEvent<T> implements IConnectionEvent<T>
{
    private handlers: { (data?: T): void; }[] = [];

    public on(handler: { (data?: T): void }): void
    {
        this.handlers.push(handler);
    }

    public off(handler: { (data?: T): void }): void
    {
        this.handlers = this.handlers.filter(h => h !== handler);
    }

    public invoke(data?: T)
    {
        this.handlers.slice(0).forEach(h => h(data));
    }

    public event(): IConnectionEvent<T>
    {
        return this;
    }
}


class HAConnection
{
    private PING_INTERVAL: number = 30 * 1000; // 30 seconds

    private ws: ReconnectingWebSocket;
    private state: HAConnectionState;
    private pingInterval: number;
    private msgId: number;
    private expectedResults: IHAExpectedResults;
    private expectedPromises: IHAExpectedPromises;

    private readonly evStateChanged = new ConnectionEvent<IHAStateChangedEvent>();

    // Events
    public get OnStateChanged() { return this.evStateChanged.event(); }
    public get ConnectionState() { return this.state; }

    constructor(private targetInstance: string)
    {
        this.expectedResults = {};
        this.expectedPromises = {};
        this.state = HAConnectionState.Closed;
    }

    public initialize(): void
    {
        this.state = HAConnectionState.Opening;
        this.msgId = 1;

        if (!this.ws || this.ws.readyState !== this.ws.OPEN)
        {
            this.ws = new ReconnectingWebSocket(this.parseSocketUrl(this.targetInstance), null, { automaticOpen: false });
        }

        this.ws.addEventListener('open', () => this.handleOpen());
        this.ws.addEventListener('close', () => this.handleClose());
        this.ws.addEventListener('message', e => this.handleMessage(e));

        this.ws.open();
    }

    public refreshAllStates(): void
    {
        this.sendStateRequest();
    }

    public getCameraImage(entity: string): Promise<IHAThumbnailResultMessage>
    {
        var p = new Promise<IHAThumbnailResultMessage>((res, rej) =>
        {
            this.sendCameraThumbnailRequest(entity, res);
        });
        return p;
    }

    public getMediaImage(entity: string): Promise<IHAThumbnailResultMessage>
    {
        var p = new Promise<IHAThumbnailResultMessage>((res, rej) =>
        {
            this.sendMediaThumbnailRequest(entity, res);
        });
        return p;
    }

    private handleMessage(e: MessageEvent): any
    {
        let msg = JSON.parse(e.data);
        if (this.isHAMessage(msg))
        {
            console.debug('-> RCV:' + msg.type, msg);

            switch (msg.type)
            {
                case HAMessageType.AuthRequired:
                    this.sendAuth();
                    return;
                case HAMessageType.AuthOK:
                    this.isReady();
                    return;
                case HAMessageType.Result:
                    let res = <IHAResultMessage>msg;
                    if (res.success)
                    {
                        if (this.handleHaExpectedResult(res))
                        {
                            delete this.expectedResults[res.id];
                        }
                        else
                        {
                            console.info('HA result OK', res);
                        }
                    }
                    else
                    {
                        this.handleHaError(res.error);
                    }
                    return;
                case HAMessageType.Event:
                    this.handleHaEvent(<IHAEventMessage>msg);
                    return;
                case HAMessageType.AuthInvalid:
                    console.error('Unable to authenticate with Home Assistant API. Check settings.');
                    this.ws.maxReconnectAttempts = 1; // Don't retry - nothing is going to change. They need to refresh the page.
                    this.ws.close();
                    return;
            }
        }
        else
        {
            console.warn('-> RCV', e.data);
        }
    }

    private isReady(): void
    {
        this.state = HAConnectionState.Open;

        // Set up ping
        this.pingInterval = window.setInterval(() => this.sendPing(), this.PING_INTERVAL);

        // Set up state change subscription
        this.sendEventSubscriptionRequest(HAEventType.StateChanged);
    }

    private handleHaEvent(msg: IHAEventMessage)
    {
        if (this.isHAEventStateChanged(msg.event))
        {
            this.eventStateChanged(msg.event)
        }
        // else if (isOther(...)) { ... }
    }

    private handleHaExpectedResult(msg: IHAResultMessage): boolean
    {
        let er = this.expectedResults[msg.id];
        if (typeof er !== 'undefined')
        {
            switch (er)
            {
                case HAResponseType.StateList:
                    this.resultStateList(<IHAEntityState[]>msg.result);
                    break;
                default:
                    console.warn('Unhandled response type for this message.', msg);
            }
            return true;
        }

        let promise = this.expectedPromises[msg.id];
        if (typeof promise !== 'undefined')
        {
            promise(<IHAThumbnailResultMessage>msg)
            return true;
        }
        return false;
    }

    private resultStateList(states: IHAEntityState[])
    {
        for (let s of states)
        {
            this.evStateChanged.invoke({
                data: {
                    entity_id: s.entity_id,
                    new_state: s,
                    old_state: null
                },
                event_type: HAEventType.StateChanged,
                origin: null,
                time_fired: null
            });
        }
    }

    private eventStateChanged(ev: IHAStateChangedEvent): void
    {
        console.info(`HA State Changed [${ev.data.entity_id}] ${(ev.data.old_state?.state ?? '<NULL>')} -> ${ev.data.new_state.state}`);
        this.evStateChanged.invoke(ev);
    }

    private sendAuth(): void
    {
        this.send(<IHAAuthMessage>{
            type: HAMessageType.Auth,
            access_token: window.ccOptions.token
        });
    }

    private sendStateRequest(): void
    {
        let responseId = this.send(<IHAMessage>{ type: HAMessageType.GetStates });
        this.expectedResults[responseId] = HAResponseType.StateList;
    }

    private sendCameraThumbnailRequest(entity: string, res: (value?: IHAThumbnailResultMessage | PromiseLike<IHAThumbnailResultMessage>) => void): void
    {
        let responseId = this.send(<IHAEntityMessage>{ type: HAMessageType.CameraThumbnail, entity_id: entity });
        this.expectedPromises[responseId] = res;
    }

    private sendMediaThumbnailRequest(entity: string, res: (value?: IHAThumbnailResultMessage | PromiseLike<IHAThumbnailResultMessage>) => void): void
    {
        let responseId = this.send(<IHAEntityMessage>{ type: HAMessageType.MediaThumbnail, entity_id: entity });
        this.expectedPromises[responseId] = res;
    }

    private sendEventSubscriptionRequest(type: HAEventType | string)
    {
        this.send(<IHAEventRequestMessage>{
            type: HAMessageType.SubscribeToEvents,
            event_type: type
        });
    }

    private handleHaError(err: IHAErrorMessage): void
    {
        console.error('HA API Error [' + err.code + '] ' + err.message);
    }

    private sendPing(): void
    {
        this.send(<IHAMessage>{
            type: HAMessageType.Ping
        });
    }

    private send(data: IHAMessage): number
    {
        console.debug('<- SND:' + data.type, data);
        if (this.state !== HAConnectionState.Closed && this.state !== HAConnectionState.Opening)
        {
            if (this.state === HAConnectionState.Open)
            {
                // Set message ID only if connection is open, and auth was previously successful
                data.id = this.msgId++;
            }

            this.ws.send(JSON.stringify(data));

            return data.id;
        }
        else
        {
            console.warn('Tried to send socket message, but connection isn\'t ready.', data);

            return -1;
        }
    }

    private handleOpen(): any
    {
        this.state = HAConnectionState.Auth;
    }

    private handleClose(): any
    {
        this.state = HAConnectionState.Closed;
        if (this.pingInterval)
        {
            window.clearInterval(this.pingInterval);
            this.pingInterval = 0;
        }
    }

    private parseSocketUrl(baseUrl: string): string
    {
        let aTag = document.createElement('a');
        aTag.href = baseUrl;

        return `${(aTag.protocol.toLowerCase() === 'https:' ? 'wss' : 'ws')}://${aTag.host}/api/websocket`;
    }

    private isHAMessage(msg: any): msg is IHAMessage
    {
        return msg && typeof msg.type === 'string';
    }

    private isHAEventStateChanged(msg: IHABaseEvent): msg is IHAStateChangedEvent
    {
        return msg && typeof msg.event_type === 'string' && msg.event_type === HAEventType.StateChanged;
    }
}