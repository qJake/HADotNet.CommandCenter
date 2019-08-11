declare interface Position
{
    x: number;
    y: number;
}

declare interface DraggabillyOptions
{
    axis?: 'x' | 'y';
    containment?: Element | string | boolean;
    grid?: [number, number];
    handle?: string;
}

declare type DraggabillyClickEventName = 'dragStart' | 'dragEnd' | 'pointerDown' | 'pointerUp' | 'staticClick';

declare type DraggabillyMoveEventName = 'dragMove' | 'pointerMove';

declare class Draggabilly
{
    position: Position;

    constructor(element: Element | string, options?: DraggabillyOptions);

    on(eventName: DraggabillyClickEventName, listener: (event: Event, pointer: MouseEvent | Touch) => void): Draggabilly;

    on(eventName: DraggabillyMoveEventName, listener: (event: Event, pointer: MouseEvent | Touch, moveVector: Position) => void): Draggabilly;

    off(eventName: DraggabillyClickEventName, listener: (event: Event, pointer: MouseEvent | Touch) => void): Draggabilly;

    off(eventName: DraggabillyMoveEventName, listener: (event: Event, pointer: MouseEvent | Touch, moveVector: Position) => void): Draggabilly;

    once(eventName: DraggabillyClickEventName, listener: (event: Event, pointer: MouseEvent | Touch) => void): Draggabilly;

    once(eventName: DraggabillyMoveEventName, listener: (event: Event, pointer: MouseEvent | Touch, moveVector: Position) => void): Draggabilly;

    enable(): void;

    disable(): void;

    destroy(): void;
}
