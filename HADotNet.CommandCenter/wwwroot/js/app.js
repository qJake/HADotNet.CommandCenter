var HAMessageType;
(function (HAMessageType) {
    HAMessageType["Auth"] = "auth";
    HAMessageType["AuthRequired"] = "auth_required";
    HAMessageType["AuthOK"] = "auth_ok";
    HAMessageType["AuthInvalid"] = "auth_invalid";
    HAMessageType["GetStates"] = "get_states";
    HAMessageType["StateChanged"] = "state_changed";
    HAMessageType["SubscribeToEvents"] = "subscribe_events";
    HAMessageType["CameraThumbnail"] = "camera_thumbnail";
    HAMessageType["MediaThumbnail"] = "media_player_thumbnail";
    HAMessageType["Result"] = "result";
    HAMessageType["Event"] = "event";
    HAMessageType["Ping"] = "ping";
    HAMessageType["Pong"] = "pong";
})(HAMessageType || (HAMessageType = {}));
var HAErrorType;
(function (HAErrorType) {
    HAErrorType["IDReuse"] = "id_reuse";
})(HAErrorType || (HAErrorType = {}));
var HAEventType;
(function (HAEventType) {
    HAEventType["StateChanged"] = "state_changed";
})(HAEventType || (HAEventType = {}));
var HAResponseType;
(function (HAResponseType) {
    HAResponseType[HAResponseType["StateList"] = 0] = "StateList";
})(HAResponseType || (HAResponseType = {}));
/// <reference path="typings/reconnecting-websocket.d.ts" />
/// <reference path="models/home-assistant-ws.ts" />
/** Defines the current state of the HA connection. */
var HAConnectionState;
(function (HAConnectionState) {
    HAConnectionState[HAConnectionState["Closed"] = 0] = "Closed";
    HAConnectionState[HAConnectionState["Opening"] = 1] = "Opening";
    HAConnectionState[HAConnectionState["Auth"] = 2] = "Auth";
    HAConnectionState[HAConnectionState["Open"] = 3] = "Open";
})(HAConnectionState || (HAConnectionState = {}));
class ConnectionEvent {
    constructor() {
        this.handlers = [];
    }
    on(handler) {
        this.handlers.push(handler);
    }
    off(handler) {
        this.handlers = this.handlers.filter(h => h !== handler);
    }
    invoke(data) {
        this.handlers.slice(0).forEach(h => h(data));
    }
    event() {
        return this;
    }
}
class HAConnection {
    constructor(targetInstance) {
        this.targetInstance = targetInstance;
        this.PING_INTERVAL = 30 * 1000; // 30 seconds
        this.evStateChanged = new ConnectionEvent();
        this.evConnectionStateChanged = new ConnectionEvent();
        this.expectedResults = {};
        this.expectedPromises = {};
        this.state = HAConnectionState.Closed;
    }
    // Events
    get OnStateChanged() { return this.evStateChanged.event(); }
    get OnConnectionStateChanged() { return this.evConnectionStateChanged.event(); }
    get ConnectionState() { return this.state; }
    initialize() {
        this.state = HAConnectionState.Opening;
        this.evConnectionStateChanged.invoke(this.state);
        this.msgId = 1;
        if (!this.ws || this.ws.readyState !== this.ws.OPEN) {
            this.ws = new ReconnectingWebSocket(this.parseSocketUrl(this.targetInstance), null, { automaticOpen: false });
        }
        this.ws.addEventListener('open', () => this.handleOpen());
        this.ws.addEventListener('close', () => this.handleClose());
        this.ws.addEventListener('message', e => this.handleMessage(e));
        this.ws.open();
    }
    refreshAllStates() {
        this.sendStateRequest();
    }
    getCameraImage(entity) {
        var p = new Promise((res, rej) => {
            this.sendCameraThumbnailRequest(entity, res);
        });
        return p;
    }
    getMediaImage(entity) {
        var p = new Promise((res, rej) => {
            this.sendMediaThumbnailRequest(entity, res);
        });
        return p;
    }
    handleMessage(e) {
        let msg = JSON.parse(e.data);
        if (this.isHAMessage(msg)) {
            console.debug('-> RCV:' + msg.type, msg);
            switch (msg.type) {
                case HAMessageType.AuthRequired:
                    this.sendAuth();
                    return;
                case HAMessageType.AuthOK:
                    this.isReady();
                    return;
                case HAMessageType.Result:
                    let res = msg;
                    if (res.success) {
                        if (this.handleHaExpectedResult(res)) {
                            delete this.expectedResults[res.id];
                        }
                        else {
                            console.info('HA result OK', res);
                        }
                    }
                    else {
                        this.handleHaError(res.error);
                    }
                    return;
                case HAMessageType.Event:
                    this.handleHaEvent(msg);
                    return;
                case HAMessageType.AuthInvalid:
                    console.error('Unable to authenticate with Home Assistant API. Check settings.');
                    this.ws.maxReconnectAttempts = 1; // Don't retry - nothing is going to change. They need to refresh the page.
                    this.ws.close();
                    return;
            }
        }
        else {
            console.warn('-> RCV', e.data);
        }
    }
    isReady() {
        this.state = HAConnectionState.Open;
        this.evConnectionStateChanged.invoke(this.state);
        // Set up ping
        this.pingInterval = window.setInterval(() => this.sendPing(), this.PING_INTERVAL);
        // Set up state change subscription
        this.sendEventSubscriptionRequest(HAEventType.StateChanged);
    }
    handleHaEvent(msg) {
        if (this.isHAEventStateChanged(msg.event)) {
            this.eventStateChanged(msg.event);
        }
        // else if (isOther(...)) { ... }
    }
    handleHaExpectedResult(msg) {
        let er = this.expectedResults[msg.id];
        if (typeof er !== 'undefined') {
            switch (er) {
                case HAResponseType.StateList:
                    this.resultStateList(msg.result);
                    break;
                default:
                    console.warn('Unhandled response type for this message.', msg);
            }
            return true;
        }
        let promise = this.expectedPromises[msg.id];
        if (typeof promise !== 'undefined') {
            promise(msg);
            return true;
        }
        return false;
    }
    resultStateList(states) {
        for (var s of states) {
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
    eventStateChanged(ev) {
        var _a, _b;
        console.info(`HA State Changed [${ev.data.entity_id}] ${(_b = (_a = ev.data.old_state) === null || _a === void 0 ? void 0 : _a.state, (_b !== null && _b !== void 0 ? _b : '<NULL>'))} -> ${ev.data.new_state.state}`);
        this.evStateChanged.invoke(ev);
    }
    sendAuth() {
        this.send({
            type: HAMessageType.Auth,
            access_token: window.ccOptions.token
        });
    }
    sendStateRequest() {
        let responseId = this.send({ type: HAMessageType.GetStates });
        this.expectedResults[responseId] = HAResponseType.StateList;
    }
    sendCameraThumbnailRequest(entity, res) {
        let responseId = this.send({ type: HAMessageType.CameraThumbnail, entity_id: entity });
        this.expectedPromises[responseId] = res;
    }
    sendMediaThumbnailRequest(entity, res) {
        let responseId = this.send({ type: HAMessageType.MediaThumbnail, entity_id: entity });
        this.expectedPromises[responseId] = res;
    }
    sendEventSubscriptionRequest(type) {
        this.send({
            type: HAMessageType.SubscribeToEvents,
            event_type: type
        });
    }
    handleHaError(err) {
        console.error('HA API Error [' + err.code + '] ' + err.message);
    }
    sendPing() {
        this.send({
            type: HAMessageType.Ping
        });
    }
    send(data) {
        console.debug('<- SND:' + data.type, data);
        if (this.state !== HAConnectionState.Closed && this.state !== HAConnectionState.Opening) {
            if (this.state === HAConnectionState.Open) {
                // Set message ID only if connection is open, and auth was previously successful
                data.id = this.msgId++;
            }
            this.ws.send(JSON.stringify(data));
            return data.id;
        }
        else {
            console.warn('Tried to send socket message, but connection isn\'t ready.', data);
            return -1;
        }
    }
    handleOpen() {
        this.state = HAConnectionState.Auth;
        this.evConnectionStateChanged.invoke(this.state);
    }
    handleClose() {
        this.state = HAConnectionState.Closed;
        this.evConnectionStateChanged.invoke(this.state);
        if (this.pingInterval) {
            window.clearInterval(this.pingInterval);
            this.pingInterval = 0;
        }
    }
    parseSocketUrl(baseUrl) {
        if (/core\/websocket/i.test(baseUrl)) {
            return baseUrl;
        }
        let aTag = document.createElement('a');
        aTag.href = baseUrl;
        return `${(aTag.protocol.toLowerCase() === 'https:' ? 'wss' : 'ws')}://${aTag.host}/api/websocket`;
    }
    isHAMessage(msg) {
        return msg && typeof msg.type === 'string';
    }
    isHAEventStateChanged(msg) {
        return msg && typeof msg.event_type === 'string' && msg.event_type === HAEventType.StateChanged;
    }
}
class PageUtils {
    static ConfirmDelete(e) {
        if (!confirm('This item will be permanently deleted. This action cannot be undone.\n\nAre you sure?')) {
            e.preventDefault();
            return false;
        }
        return true;
    }
}
/// <reference path="entityState.ts" />
var PageMode;
(function (PageMode) {
    PageMode[PageMode["User"] = 0] = "User";
    PageMode[PageMode["Admin"] = 1] = "Admin";
})(PageMode || (PageMode = {}));
/**
 * A general utility class for miscellaneous helper functions.
 */
class Utils {
    /**
     * Introduces a delay in a promise chain.
     * @param duration The duration, in ms, of the desired delay.
     * @param args Any args to pass on through to the next promise in the chain.
     */
    static delayPromise(duration, ...args) {
        return new Promise(resolve => setTimeout(() => resolve(args), duration));
    }
    /**
     * Causes the current thread to sleep for the specified number of milliseconds.
     * @param ms The number of milliseconds to sleep for.
     */
    static sleep(ms) {
        const date = Date.now();
        let now = null;
        do {
            now = Date.now();
        } while (now - date < ms);
    }
    /**
     * Resolves an asset URL to a fully-qualified path.
     * @param primaryUrl The primary URL.
     * @param overrideUrl An override URL. Can be null.
     * @param relativePath The relative path to append at the end.
     */
    static resolveAssetUrl(primaryUrl, overrideUrl, relativePath) {
        primaryUrl = !primaryUrl.endsWith('/') ? primaryUrl : primaryUrl.substr(0, primaryUrl.length - 1);
        overrideUrl = overrideUrl && !overrideUrl.endsWith('/') ? overrideUrl : overrideUrl.substr(0, overrideUrl.length - 1);
        relativePath = !relativePath.startsWith('/') ? relativePath : relativePath.substr(1, relativePath.length - 1);
        return `${(overrideUrl && overrideUrl.length ? overrideUrl : primaryUrl)}/${relativePath}`;
    }
    /**
     * Resolves various icon options to display the correct one.
     * @param defaultIcon The icon defined in Home Assistant.
     * @param overrideIcon The user's override icon choice.
     */
    static resolveIcon(defaultIcon, overrideIcon) {
        if (overrideIcon && overrideIcon.length) {
            return overrideIcon;
        }
        if (defaultIcon && defaultIcon.length && /^mdi:/i.test(defaultIcon)) {
            return defaultIcon.replace('mdi:', '');
        }
        return '';
    }
    /**
     * Preloads an image and notifies when done via a promise.
     * @param src The image URL to load.
     */
    static preloadImage(src) {
        return new Promise((resolve, reject) => {
            let img = new Image();
            img.onload = () => resolve(src);
            img.onerror = e => reject(e);
            img.src = src;
        });
    }
    /**
     * Converts a degree number to a compass's cardinal direction.
     * @param deg The degrees to convert.
     */
    static convertDegreesToCardinal(deg) {
        return ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"][(Math.floor((deg / 22.5) + 0.5) % 16)];
    }
    /**
     * Converts a cardinal direction to an arrow icon.
     * @param dir The direction to convert.
     */
    static convertCardinalToIcon(dir) {
        return {
            N: 'arrow-up-thick',
            NNE: 'arrow-up-thick',
            NE: 'arrow-top-right-thick',
            ENE: 'arrow-right-thick',
            E: 'arrow-right-thick',
            ESE: 'arrow-right-thick',
            SE: 'arrow-bottom-right-thick',
            SSE: 'arrow-down-thick',
            S: 'arrow-down-thick',
            SSW: 'arrow-down-thick',
            SW: 'arrow-bottom-left-thick',
            WSW: 'arrow-left-thick',
            W: 'arrow-left-thick',
            WNW: 'arrow-left-thick',
            NW: 'arrow-top-left-thick',
            NNW: 'arrow-up-thick'
        }[dir];
    }
}
/// <reference path="../../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />
/// <reference path="../utils.ts" />
/// <reference path="tileoptions.ts" />
class Tile {
    constructor(page, name, conn, haConn, options) {
        var _a;
        this.page = page;
        this.name = name;
        this.conn = conn;
        this.canLoad = options.canLoad;
        this.canClick = options.canClick;
        this.entityIds = [];
        this.loaded = !this.canLoad;
        this.el = $(`.tiles .tile[data-tile-name="${name}"]`);
        if (this.canClick) {
            this.el.click(() => {
                this.onClick();
            });
        }
        var entityList = this.el.data('tile-entityid');
        if (typeof entityList === 'object' && Array.isArray(entityList)) {
            this.entityIds = entityList;
        }
        else {
            this.entityIds.push((_a = entityList) === null || _a === void 0 ? void 0 : _a.toString());
        }
        conn.on('SendSystemConfig', (tname, cfg) => {
            if (name == tname) {
                console.debug(`Received "SendSystemConfig" for tile: ${tname}`);
                this.config = cfg;
                // New config = re-request state
                if (this.canLoad) {
                    //why?
                    //this.requestState();
                }
            }
        });
        conn.on('SendTile', t => {
            if (name == t.name) {
                console.debug(`Received "SendTile" for tile: ${t.name}`);
                this.updateTile(t);
            }
        });
        conn.on('SendCalendarInfo', (t, s, e) => {
            if (name == t.name) {
                //console.debug(`Received: "SendCalendarInfo" for tile: ${(t as ITile).name}`);
                this.updateCalendar(s, e);
            }
        });
        conn.on('SendWarning', msg => console.warn(msg));
        conn.on('SendDateTime', (tile, d, t) => {
            if (name == tile.name) {
                //console.debug(`Received: "SendDateTime" for tile: ${(tile as ITile).name}`);
                this.updateDateTime(tile, d, t);
            }
        });
        this.requestConfig(page);
        if (this.canLoad) {
            this.requestState();
        }
    }
    onClick() {
        return this.conn.invoke('OnTileClicked', this.page, this.name);
    }
    updateTile(tile) {
        this.loaded = true;
        this.disableLoading();
    }
    updateState(state) { }
    updateCalendar(state, events) { }
    updateDateTime(tile, ...args) { }
    requestState(debounce) {
        this.conn.invoke('RequestTileState', this.page, this.name);
    }
    requestConfig(page) {
        this.conn.invoke('RequestConfig', page, this.name);
    }
    disableLoading() {
        if (this.loadingDebouncer) {
            window.clearTimeout(this.loadingDebouncer);
        }
        this.loadingDebouncer = null;
        this.el.removeClass("tile-loading");
    }
    getEntityIds() {
        return this.entityIds;
    }
}
/// <reference path="tile.ts" />
class BlankTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: false, canLoad: false });
    }
}
/// <reference path="tile.ts" />
class LabelTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: false, canLoad: false });
    }
}
/// <reference path="tile.ts" />
class DateTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: false, canLoad: true });
    }
    updateDateTime(tile, date, time) {
        $(`#tile-${tile.name}`).find('span[value-date]').text(date);
        $(`#tile-${tile.name}`).find('span[value-time]').text(time);
        super.updateDateTime();
        setTimeout(() => {
            this.requestState(9500);
        }, 10000);
    }
}
/// <reference path="tile.ts" />
class StateTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: false, canLoad: false });
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateState(state) {
        //console.log("State received for: " + tile.name, state);
        let label = state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        let value = this.tile.roundDecimals ? parseInt(state.new_state.state).toString() : state.new_state.state;
        if (this.tile.displayTextOff && value.toLowerCase() === 'off') {
            value = this.tile.displayTextOff;
        }
        else if (this.tile.displayTextOn && value.toLowerCase() === 'on') {
            value = this.tile.displayTextOn;
        }
        if (state.new_state.attributes["unit_of_measurement"]) {
            value += state.new_state.attributes["unit_of_measurement"].toString();
        }
        $(`#tile-${this.tile.name}`).find('span[value-state]').text(value);
    }
}
/// <reference path="tile.ts" />
class LightTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateState(state) {
        //console.log("State received for: " + this.tile.name, state);
        let label = state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-icon]')
            .removeClass(`mdi-${this.tile.displayIcon} mdi-${this.tile.displayOffIcon}`)
            .addClass(`mdi mdi-${state.new_state.state.toLowerCase() === "on" ? Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayIcon) : Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayOffIcon || this.tile.displayIcon)}`);
        // TODO: Add custom on/off state keywords
        $(`#tile-${this.tile.name}`)
            .find('span[value-icon]')
            .removeClass("state-off state-on")
            .addClass(state.new_state.state.toLowerCase() === "on" ? "state-on" : "state-off");
        if (this.tile.onColor && state.new_state.state.toLowerCase() === "on") {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.onColor);
        }
        if (this.tile.offColor && state.new_state.state.toLowerCase() !== "on") {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.offColor);
        }
    }
}
/// <reference path="tile.ts" />
class SwitchTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateState(state) {
        var _a;
        //console.log("State received for: " + tile.name, state);
        let label = state.new_state.attributes["friendly_name"].toString();
        if ((_a = this.tile) === null || _a === void 0 ? void 0 : _a.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-icon]')
            .removeClass(`mdi-${this.tile.displayIcon} mdi-${this.tile.displayOffIcon}`)
            .addClass(`mdi mdi-${state.new_state.state.toLowerCase() === "on" ? Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayIcon) : Utils.resolveIcon(state.new_state.attributes["icon"], this.tile.displayOffIcon || this.tile.displayIcon)}`);
        // TODO: Add custom on/off state keywords
        $(`#tile-${this.tile.name}`)
            .find('span[value-icon]')
            .removeClass("state-off state-on")
            .addClass(state.new_state.state.toLowerCase() === "on" ? "state-on" : "state-off");
        if (this.tile.onColor && state.new_state.state.toLowerCase() === "on") {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.onColor);
        }
        if (this.tile.offColor && state.new_state.state.toLowerCase() !== "on") {
            $(`#tile-${this.tile.name} .value`).css('color', this.tile.offColor);
        }
        super.updateState(state);
    }
}
/// <reference path="tile.ts" />
class PersonTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: false, canLoad: false });
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateState(state) {
        //console.log("State received for: " + tile.name, state);
        let picture = state.new_state.attributes['entity_picture'] ? state.new_state.attributes['entity_picture'].toString() : '';
        let location = state.new_state.state.replace('_', ' ');
        let label = state.new_state.attributes['friendly_name'].toString();
        if (this.tile.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        let isHome = location.toLowerCase() === 'home';
        // Adjust base URL
        if (!picture.toLowerCase().startsWith('http')) {
            picture = Utils.resolveAssetUrl(window.ccOptions.baseUrl, window.ccOptions.overrideAssetUrl, picture);
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-location]').text(location);
        $(`#tile-${this.tile.name}`).find('span[value-picture]').css('background-image', `url(${picture})`).removeClass('bw');
        if (!isHome) {
            $(`#tile-${this.tile.name}`).find('span[value-picture]').addClass('bw');
        }
    }
}
/// <reference path="tile.ts" />
/// <reference path="../models/skycons.d.ts" />
class WeatherTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canLoad: true, canClick: false });
        this.page = page;
        this.name = name;
        this.conn = conn;
        this.windSpeed = '';
        this.windDir = '';
        this.hi = '';
        this.lo = '';
        this.iconEl = this.el.find('.condition-icon')[0];
        const fontColor = document.defaultView.getComputedStyle(this.el[0]).color;
        this.skycons = new Skycons({ color: fontColor, resizeClear: true });
        this.skycons.add(this.iconEl, Skycons.CLEAR_DAY);
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateState(state) {
        let value = state.new_state == null ? null : state.new_state.state;
        switch (state.entity_id) {
            case this.tile.entityId:
                if (state.new_state.attributes["unit_of_measurement"]) {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                $(`#tile-${this.tile.name}`).find('span[value-temp]').text(value);
                break;
            case this.tile.highTempEntity:
                if (state.new_state.attributes["unit_of_measurement"]) {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                this.hi = `<i class="mdi mdi-arrow-up-thick"></i> ${value}`;
                break;
            case this.tile.lowTempEntity:
                if (state.new_state.attributes["unit_of_measurement"]) {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                this.lo = `<i class="mdi mdi-arrow-down-thick"></i> ${value}`;
                break;
            case this.tile.summaryEntity:
                $(`#tile-${this.tile.name}`).find('span[value-summary]').text(value);
                break;
            case this.tile.precipChanceEntity:
                if (state.new_state.attributes["unit_of_measurement"]) {
                    value += state.new_state.attributes["unit_of_measurement"].toString();
                }
                $(`#tile-${this.tile.name}`).find('span[value-rain]').text(`Rain: ${value}`);
                break;
            case this.tile.windSpeedEntity:
                this.windSpeed = this.tile.roundWindSpeed ? parseInt(value).toString() : value;
                if (state.new_state.attributes["unit_of_measurement"]) {
                    this.windSpeed += state.new_state.attributes["unit_of_measurement"].toString();
                }
                break;
            case this.tile.windDirectionEntity:
                this.windDir = Utils.convertDegreesToCardinal(parseInt(value));
                this.windDir = `<i class="mdi mdi-${Utils.convertCardinalToIcon(this.windDir)}"></i> ${this.windDir}`;
                break;
            case this.tile.iconEntity:
                if (value) {
                    this.skycons.set(this.iconEl, value);
                    this.skycons.play();
                }
                else {
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
/// <reference path="tile.ts" />
class CameraTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: false, canLoad: true });
        this.haConn = haConn;
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
        this.updateCameraFeed();
        window.setInterval(() => this.updateCameraFeed(), (this.tile.refreshRate > 0 ? this.tile.refreshRate : 1) * 1000);
    }
    updateCameraFeed() {
        this.haConn.getCameraImage(this.tile.entityId)
            .then(msg => {
            let imageSize = this.tile.imageCropMode.toLowerCase() === 'cover' || this.tile.imageCropMode.toLowerCase() === 'contain'
                ? this.tile.imageCropMode.toLowerCase()
                : '100% 100%';
            let imagePosition = this.tile.imageCropMode.toLowerCase() === 'cover' || this.tile.imageCropMode.toLowerCase() === 'contain'
                ? '50% 50%'
                : '0 0';
            $(`#tile-${this.tile.name}`).css({
                backgroundImage: `url('data:${msg.result.content_type};base64,${msg.result.content}')`,
                backgroundRepeat: 'no-repeat',
                backgroundPosition: imagePosition,
                backgroundSize: imageSize
            });
        });
    }
}
/// <reference path="tile.ts" />
class SceneTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateState(state) {
        let label = state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        $(`#tile-${this.tile.name}`).find('span[value-icon]').addClass(`mdi mdi-${this.tile.displayIcon || 'filmstrip'}`);
        $(`#tile-${this.tile.name} .value`).css('color', this.tile.iconColor);
    }
}
/// <reference path="tile.ts" />
class MediaTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
        this.haConn = haConn;
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
        this.updateMediaImage();
        window.setInterval(() => this.updateMediaImage(), (this.tile.refreshRate > 0 ? this.tile.refreshRate : 1) * 1000);
    }
    updateMediaImage() {
        var _a;
        if (!((_a = this.state) === null || _a === void 0 ? void 0 : _a.new_state)) {
            return;
        }
        let label = this.state.new_state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        if (!this.state.new_state.attributes["entity_picture"]) {
            label += ` (${this.state.new_state.state})`;
        }
        $(`#tile-${this.tile.name}`).toggleClass('media-idle', this.state.new_state.attributes['media_title'] === 'Nothing playing' || ((this.state.new_state.state == 'paused' || this.state.new_state.state == 'idle') && !this.state.new_state.attributes["entity_picture"]));
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(this.tile.showLabel ? label : '');
        $(`#tile-${this.tile.name}`).find('span[value-title]').text(this.tile.showTitle && this.state.new_state.attributes['media_title'] && this.state.new_state.attributes['media_title'] !== 'Nothing playing' ? this.state.new_state.attributes['media_title'].toString() : '');
        if (!this.state.new_state.attributes["entity_picture"]) {
            return;
        }
        this.haConn.getMediaImage(this.tile.entityId)
            .then(msg => {
            let imageSize = this.tile.imageCropMode.toLowerCase() === 'cover' || this.tile.imageCropMode.toLowerCase() === 'contain'
                ? this.tile.imageCropMode.toLowerCase()
                : '100% 100%';
            let imagePosition = this.tile.imageCropMode.toLowerCase() === 'cover' || this.tile.imageCropMode.toLowerCase() === 'contain'
                ? '50% 50%'
                : '0 0';
            $(`#tile-${this.tile.name}`).css({
                backgroundImage: `url('data:${msg.result.content_type};base64,${msg.result.content}')`,
                backgroundRepeat: 'no-repeat',
                backgroundPosition: imagePosition,
                backgroundSize: imageSize
            });
        });
    }
    updateState(s) {
        this.state = s;
    }
}
/// <reference path="tile.ts" />
class NavigationTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canClick: true, canLoad: false });
    }
    updateTile(tile) {
        this.navTile = tile;
        $(`#tile-${tile.name}`).find('span[value-name]').text(this.navTile.label);
        $(`#tile-${tile.name}`).find('span[value-icon]').addClass(`mdi mdi-${this.navTile.displayIcon}`);
        super.updateTile();
    }
    onClick() {
        switch (this.navTile.mode.toLowerCase().trim()) {
            case 'home':
                window.location.href = '/d/';
                return;
            case 'refresh':
                window.location.reload();
                return;
            case 'nav':
                window.location.href = `/d/${this.navTile.target}`;
                return;
        }
    }
}
/// <reference path="tile.ts" />
/// <reference path="../models/eventData.ts" />
/// <reference path="../typings/moment.d.ts" />
class CalendarTile extends Tile {
    constructor(page, name, conn, haConn) {
        super(page, name, conn, haConn, { canLoad: true, canClick: false });
        this.eventContainer = $(`#tile-${name} div.calendar-events`);
    }
    updateTile(t) {
        this.tile = t;
        super.updateTile(t);
    }
    updateCalendar(state, events) {
        let label = state.attributes["friendly_name"].toString();
        if (this.tile.overrideLabel) {
            label = this.tile.overrideLabel;
        }
        $(`#tile-${this.tile.name}`).find('span[value-name]').text(label);
        this.refreshEvents(events);
        super.updateCalendar();
        if (this.tile.refreshRate > 0) {
            setTimeout(() => {
                this.requestState(1000);
            }, this.tile.refreshRate * 1000);
        }
    }
    refreshEvents(events) {
        var _a;
        this.eventContainer.empty();
        if (!events.length) {
            this.eventContainer.append('<span class="no-events">No events!</span>');
        }
        else {
            let lastGroup = '';
            for (let i = 0; i < events.length; i++) {
                const evt = events[i];
                const thisGroup = this.getEventHeader(evt);
                if (lastGroup != thisGroup) {
                    // Write header
                    this.eventContainer.append(`<h3>${thisGroup}</h3>`);
                    lastGroup = thisGroup;
                }
                this.eventContainer.append(`<p><span class="summary">${evt.summary}</span><span class="time">${(!evt.start.dateTime ? 'All Day' : moment((_a = evt.start.dateTime, (_a !== null && _a !== void 0 ? _a : evt.start.date))).format('LT'))}</span></p>`);
            }
        }
    }
    getEventHeader(event) {
        var _a;
        const today = moment();
        const tomorrow = moment().add(1, 'day');
        let todayHeader = this.formatHeader(today);
        let tomorrowHeader = this.formatHeader(tomorrow);
        const mt = moment((_a = event.start.dateTime, (_a !== null && _a !== void 0 ? _a : event.start.date)));
        let header = this.formatHeader(mt);
        if (header === todayHeader) {
            header += ' (Today)';
        }
        else if (header === tomorrowHeader) {
            header += ' (Tomorrow)';
        }
        return header;
    }
    formatHeader(mt) {
        return mt.format('ddd') + ', ' + mt.format('ll');
    }
}
/// <reference path="tile.ts" />
/// <reference path="blank.tile.ts" />
/// <reference path="label.tile.ts" />
/// <reference path="date.tile.ts" />
/// <reference path="state.tile.ts" />
/// <reference path="light.tile.ts" />
/// <reference path="switch.tile.ts" />
/// <reference path="person.tile.ts" />
/// <reference path="weather.tile.ts" />
/// <reference path="camera.tile.ts" />
/// <reference path="scene.tile.ts" />
/// <reference path="media.tile.ts" />
/// <reference path="navigation.tile.ts" />
/// <reference path="calendar.tile.ts" />
class TileMap {
}
TileMap.ClassMap = {
    'Blank': BlankTile,
    'Label': LabelTile,
    'Date': DateTile,
    'State': StateTile,
    'Light': LightTile,
    'Switch': SwitchTile,
    'Person': PersonTile,
    'Weather': WeatherTile,
    'Camera': CameraTile,
    'Scene': SceneTile,
    'Media': MediaTile,
    'Navigation': NavigationTile,
    'Calendar': CalendarTile
};
/// <reference path="models/models.ts" />
/// <reference path="typings/window-options.d.ts" />
/// <reference path="typings/draggabilly.d.ts" />
/// <reference path="typings/packery.d.ts" />
/// <reference path="typings/packery.jquery.d.ts" />
/// <reference path="../../node_modules/@aspnet/signalr/dist/esm/index.d.ts" />
/// <reference path="tiles/tilemap.ts" />
/// <reference path="PageFunctions.ts" />
class CommandCenter {
    constructor() {
        this.tiles = [];
        $(() => this.init());
    }
    init() {
        window.ccOptions.mode == PageMode.Admin
            ? this.initAdmin()
            : this.initUser();
        this.initializeMdiPreview();
        this.initializeColorPreview();
        this.initializeNightlyRefresh();
    }
    initAdmin() {
        $(window).on('beforeunload', e => {
            if (this.pageIsDirty && $(e.target.activeElement).prop('type') !== 'submit') {
                return 'You have unsaved changes. Are you sure you want to leave?';
            }
        });
        $('#importTheme, #importConfig').click(() => {
            if (confirm('WARNING: This will OVERWRITE your current settings. Export first if you want to save what you have now! Continue?')) {
                $('#importBrowser').click();
            }
        });
        $('#importBrowser').change(() => {
            $('#importForm').submit();
        });
        $('#resetConfig').click(e => {
            if (!confirm("WARNING: This will COMPLETELY RESET your HACC installation and PERMANENTLY DELETE all of your tiles, themes, and settings. Are you sure you want to reset your config?")) {
                e.preventDefault();
                return false;
            }
            return true;
        });
        $('.ui.accordion').accordion();
        $('.ui.checkbox').checkbox();
        $('.ui.dropdown').not('.no-placeholder').dropdown({ fullTextSearch: true });
        $('.ui.no-placeholder.dropdown').dropdown({ placeholder: false });
        // Font dropdown with real font previews.
        $('#Page_PageFontFace option').each(function (_, e) {
            $(e).parent().siblings('.menu').find('.item[data-value="' + $(e).text() + '"]').css('font-family', $(e).text());
        });
        // Only init Packery stuff if we detect we have the preview grid on the page
        if ($('.preview-layout-grid').length) {
            $('#auto-layout').click(() => {
                if (confirm('This will reset the layout for this page and attempt to automatically arrange all tiles evenly.\n\nYour tiles will all probably change locations. You have been warned. :)\n\nAre you sure you want to reset the layout?')) {
                    this.pk.layout();
                }
            });
            // For some reason Draggabilly takes the first element as the grid size, so inject a temporary invisible "fake" one
            $('.preview-layout-grid').prepend(`<div class="preview-layout-item" style="opacity: 0; position: absolute; top: ${window.ccOptions.tilePreviewPadding}px; left: ${window.ccOptions.tilePreviewPadding}px; width: ${window.ccOptions.tilePreviewSize}px; height: ${window.ccOptions.tilePreviewSize}px;" id="grid__tmp"></div>`);
            if (window.ccOptions) {
                this.pk = new Packery('.preview-layout-grid', {
                    itemSelector: '.preview-layout-item',
                    columnWidth: window.ccOptions.tilePreviewSize,
                    rowHeight: window.ccOptions.tilePreviewSize,
                    gutter: window.ccOptions.tilePreviewPadding,
                    initLayout: false
                });
            }
            else {
                this.pk = new Packery('.preview-layout-grid', {
                    itemSelector: '.preview-layout-item',
                    columnWidth: '.preview-layout-item',
                    rowHeight: '.preview-layout-item',
                    gutter: window.ccOptions.tilePreviewPadding,
                    initLayout: false
                });
            }
            this.pk.on('layoutComplete', () => this.writeItemLayout());
            this.pk.on('dragItemPositioned', () => {
                // Things get kinda glitchy if we don't add a slight pause
                setTimeout(() => {
                    this.writeItemLayout();
                    this.pageIsDirty = true;
                }, 25);
            });
            this.writeItemLayout();
            if (typeof Draggabilly === 'function') {
                $('.preview-layout-item').each((_, e) => this.pk.bindDraggabillyEvents(new Draggabilly(e, { containment: '.preview-layout-grid' })));
            }
            else {
                console.warn("Draggabilly is not available - drag and drop interface will not work.");
            }
            $('#grid__tmp').remove();
            this.pk.initShiftLayout(Array.from(document.querySelectorAll('.preview-layout-grid > .preview-layout-item')));
        }
    }
    initUser() {
        if (window.ccOptions.socketUrl) {
            this.conn = new HAConnection(window.ccOptions.socketUrl);
        }
        this.conn.OnConnectionStateChanged.on(state => {
            if (state == HAConnectionState.Closed) {
                $('#alerts').show().find('.alert-message').text('[H] Connection lost, reconnecting...');
            }
            else if (state == HAConnectionState.Open) {
                $('#alerts').hide();
            }
        });
        this.conn.OnStateChanged.on(state => {
            var tiles = this.findTilesByEntityId(state.data.entity_id);
            for (var t of tiles) {
                t.updateState(state.data);
                console.info(`Updating tile for entity "${state.data.entity_id}" to state "${state.data.new_state.state}".`);
            }
        });
        this.conn.initialize();
        this.tileConn = new signalR.HubConnectionBuilder().withUrl('/hubs/tile').build();
        this.tileConn.onclose(e => {
            $('#alerts').show().find('.alert-message').text('[S] Connection lost, reconnecting...');
            window.setTimeout(() => {
                window.location.reload();
            }, 10000);
        });
        this.tileConn.start().then(() => {
            $('.tiles .tile').each((_, e) => {
                try {
                    let tile = new TileMap.ClassMap[$(e).data('tile-type').toString()](window.ccOptions.pageId, $(e).data('tile-name'), this.tileConn, this.conn);
                    this.tiles.push(tile);
                }
                catch (ex) {
                    console.error('Error instantiating class "' + ($(e).data('tile-type') || '__MISSING__') + 'Tile". Was it added to the tile type map?', ex, e);
                }
            });
            if (!this.initHandle) {
                this.initHandle = window.setInterval(() => this.waitAndPerformInit(), 25);
            }
            if (window.ccOptions.autoReturn > 0) {
                window.setTimeout(() => window.location.href = '/d/', window.ccOptions.autoReturn * 1000);
            }
        });
    }
    waitAndPerformInit() {
        if (!this.conn) {
            console.warn('Cancelling tile initialization - connection is not set up.');
            window.clearInterval(this.initHandle);
        }
        if (this.conn.ConnectionState !== HAConnectionState.Open)
            return;
        if (this.tiles.filter(t => t.loaded).length !== this.tiles.length)
            return;
        // Now, refresh all of them at once
        this.conn.refreshAllStates();
        window.clearInterval(this.initHandle);
    }
    findTilesByEntityId(entityId) {
        return this.tiles.filter(t => {
            let definedIds = t.getEntityIds();
            return definedIds.some(e => e.toLowerCase() === entityId.toLowerCase());
        });
    }
    initializeNightlyRefresh() {
        // Workaround to get around SignalR hub being kinda crappy :/
        window.setInterval(() => {
            if (new Date().getHours() == 2) {
                window.setTimeout(() => {
                    window.location.reload();
                }, 3600000); // 1 hour
            }
        }, 3500000); // ~58 minutes
    }
    initializeMdiPreview() {
        $('.mdi-icon-placeholder + input').each((_, e) => {
            $(e).keyup((el) => {
                this.refreshDynamicIcon(el.currentTarget);
            });
            this.refreshDynamicIcon(e);
        });
    }
    refreshDynamicIcon(target) {
        $(target).parent().children('.mdi-icon-placeholder').attr('class', 'large icon mdi-icon-placeholder').addClass(`mdi mdi-${$(target).val()}`);
    }
    initializeColorPreview() {
        $('.color-preview + input').each((_, e) => {
            $(e).keyup((el) => {
                this.refreshDynamicColor(el.currentTarget);
            });
            this.refreshDynamicColor(e);
        });
    }
    refreshDynamicColor(target) {
        $(target).parent().children('.color-preview').css('color', `${$(target).val()}`);
    }
    writeItemLayout() {
        var positions = [];
        var tiles = this.pk.getItemElements();
        for (let i = 0; i < tiles.length; i++) {
            let $tile = $(tiles[i]);
            positions.push({
                index: i,
                x: parseInt($tile.css('left').replace('px', '')),
                y: parseInt($tile.css('top').replace('px', '')),
                name: $tile.data('tile-name')
            });
        }
        $('#layout-serialized').val(JSON.stringify(positions));
    }
}
var __app = new CommandCenter();
/**
 * Initializes a pre-existing layout from the element's current position.
 */
Packery.prototype.initShiftLayout = function (elements) {
    this._resetLayout();
    // set item order and horizontal position from saved positions
    this.items = elements.map(function (e) {
        var item = this.getItem(e);
        let x = parseInt(e.style.left.replace('px', ''));
        let y = parseInt(e.style.top.replace('px', ''));
        let w = e.clientWidth;
        let h = e.clientHeight;
        item.rect.x = x - this.gutter;
        item.rect.y = y;
        item.rect.height = h;
        item.rect.width = w;
        item.position.x = x - this.gutter;
        item.position.y = y;
        return item;
    }, this);
    this.shiftLayout();
};
var WeatherTileEntities;
(function (WeatherTileEntities) {
    WeatherTileEntities["entityId"] = "entityId";
    WeatherTileEntities["iconEntity"] = "iconEntity";
    WeatherTileEntities["summaryEntity"] = "summaryEntity";
    WeatherTileEntities["precipChanceEntity"] = "precipChanceEntity";
    WeatherTileEntities["highTempEntity"] = "highTempEntity";
    WeatherTileEntities["lowTempEntity"] = "lowTempEntity";
    WeatherTileEntities["windSpeedEntity"] = "windSpeedEntity";
    WeatherTileEntities["windDirectionEntity"] = "windDirectionEntity";
})(WeatherTileEntities || (WeatherTileEntities = {}));
