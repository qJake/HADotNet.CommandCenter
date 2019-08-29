declare class Skycons
{
    constructor(options?: ISkyconsOptions)

    public add(el: string | Element, draw: (() => void) | string): void
    public set(el: string | Element, draw: (() => void) | string): void
    public remove(el: string | Element): void
    public play(): void
    public pause(): void

    public static CLEAR_DAY: () => void
    public static CLEAR_NIGHT: () => void
    public static PARTLY_CLOUDY_DAY: () => void
    public static PARTLY_CLOUDY_NIGHT: () => void
    public static CLOUDY: () => void
    public static RAIN: () => void
    public static SLEET: () => void
    public static SNOW: () => void
    public static WIND: () => void
    public static FOG: () => void
}

declare interface ISkyconsOptions
{
    color?: string;
    resizeClear?: boolean;
}
