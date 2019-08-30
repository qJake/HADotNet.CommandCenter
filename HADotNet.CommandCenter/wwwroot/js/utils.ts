/**
 * A general utility class for miscellaneous helper functions.
 */
class Utils
{
    /**
     * Introduces a delay in a promise chain.
     * @param duration The duration, in ms, of the desired delay.
     * @param args Any args to pass on through to the next promise in the chain.
     */
    public static delayPromise(duration: number, ...args: any): Promise<any>
    {
        return new Promise(resolve => setTimeout(() => resolve(args), duration));
    }

    /**
     * Resolves various icon options to display the correct one.
     * @param defaultIcon The icon defined in Home Assistant.
     * @param overrideIcon The user's override icon choice.
     */
    public static resolveIcon(defaultIcon: string, overrideIcon: string): string
    {
        if (overrideIcon && overrideIcon.length)
        {
            return overrideIcon;
        }
        if (defaultIcon && defaultIcon.length && /^mdi:/i.test(defaultIcon))
        {
            return defaultIcon.replace('mdi:', '');
        }
        return '';
    }

    /**
     * Preloads an image and notifies when done via a promise.
     * @param src The image URL to load.
     */
    public static preloadImage(src: string): Promise<string>
    {
        return new Promise((resolve, reject) =>
        {
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
    public static convertDegreesToCardinal(deg: number): string
    {
        return ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"][(Math.floor((deg / 22.5) + 0.5) % 16)];
    }

    /**
     * Converts a cardinal direction to an arrow icon.
     * @param dir The direction to convert.
     */
    public static convertCardinalToIcon(dir: string): string
    {
        return (<StringDictionary>{
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
        })[dir];
    }
}