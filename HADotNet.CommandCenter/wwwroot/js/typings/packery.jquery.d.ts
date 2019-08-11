/// <reference types="jquery" />
/// <reference path="packery.d.ts" />
/// <reference path="draggabilly.d.ts" />

interface JQuery
{
    packery(config?: PackeryOptions): Packery;

    packery(method: 'shiftLayout' | 'layoutItems' | 'fit' | 'stamp' | 'unstamp' | 'appended' | 'prepended' | 'addItems' | 'remove' | 'reloadItems' | 'destroy' | 'getItemElements', ...args: any): Packery;

    draggabilly(config?: DraggabillyOptions): Draggabilly;
}
