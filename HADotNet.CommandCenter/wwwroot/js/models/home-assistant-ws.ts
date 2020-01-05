enum HAMessageType
{
    Auth = 'auth',
    AuthRequired = 'auth_required',
    AuthOK = 'auth_ok',
    AuthInvalid = 'auth_invalid',
    GetStates = 'get_states',
    StateChanged = 'state_changed',
    SubscribeToEvents = 'subscribe_events',
    Result = 'result',
    Event = 'event',
    Ping = 'ping',
    Pong = 'pong'
}

enum HAErrorType
{
    IDReuse = 'id_reuse'
}

enum HAEventType
{
    StateChanged = 'state_changed'
}

enum HAResponseType
{
    StateList
}

interface IHAMessage
{
    type: HAMessageType
    id?: number
}

interface IHAAuthMessage extends IHAMessage
{
    access_token: string
}

interface IHAResultMessage extends IHAMessage
{
    success: boolean
    error: IHAErrorMessage
    result: any
}

interface IHAStateResultMessage extends IHAResultMessage
{
    result: IHAEntityState[]
}

interface IHAEventRequestMessage extends IHAMessage
{
    event_type?: HAEventType | string
}

interface IHAEventMessage extends IHAMessage
{
    event: IHABaseEvent
}

interface IHAExpectedResults
{
    [id: number]: HAResponseType
}

interface IHAEntityState
{
    entity_id: string
    state: string
    attributes: IHAAttributes
    last_changed: Date
    last_updated: Date
    context?: IHAContext
}

interface IHAContext
{
    id?: string
    parent_id?: string
    user_id?: string
}

interface IHABaseEvent
{
    event_type: HAEventType
    time_fired: Date
    origin: string
}

interface IHAStateChangedEvent extends IHABaseEvent
{
    data: IHAStateChangedData
}

interface IHAStateChangedData
{
    entity_id: string
    new_state: IHAEntityState
    old_state: IHAEntityState
}

interface IHAAttributes
{
    [name: string]: any
}

interface IHAErrorMessage
{
    code: HAErrorType
    message: string
}