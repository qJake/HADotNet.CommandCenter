/** A small JavaScript library that decorates the WebSocket API to provide a WebSocket connection that will automatically reconnect if the connection is dropped. */
declare interface IReconnectingWebSocketOptions
{
    /** Whether this instance should log debug messages or not. Debug messages are printed to console.debug(). */
    debug?: boolean

    /** Whether or not the websocket should attempt to connect immediately upon instantiation. The socket can be manually opened or closed at any time using ws.open() and ws.close(). */
    automaticOpen?: boolean

    /** The number of milliseconds to delay before attempting to reconnect. */
    reconnectInterval?: number

    /** The maximum number of milliseconds to delay a reconnection attempt. */
    maxReconnectInterval?: number

    /** The rate of increase of the reconnect delay. Allows reconnect attempts to back off when problems persist. */
    reconnectDecay?: number

    /** The maximum time in milliseconds to wait for a connection to succeed before closing and retrying. */
    timeoutInterval?: number

    /** The maximum number of reconnection attempts that will be made before giving up. If null, reconnection attempts will be continue to be made forever. */
    maxReconnectAttempts?: number
}

declare interface ReconnectingWebSocket extends IReconnectingWebSocketOptions, WebSocket
{
    // Open source: https://github.com/joewalnes/reconnecting-websocket
    // Author: @joewalnes

    open(): void
}

declare var ReconnectingWebSocket: {
    prototype: ReconnectingWebSocket;
    new(url: string, protocols?: string | string[]): ReconnectingWebSocket;
    new(url: string, protocols: string | string[], options?: IReconnectingWebSocketOptions): ReconnectingWebSocket;
}