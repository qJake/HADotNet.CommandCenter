/// <reference path="window-options.d.ts" />
/// <reference path="draggabilly.d.ts" />
/// <reference path="packery.d.ts" />
/// <reference path="packery.jquery.d.ts" />

class CommandCenter
{
    private pk: Packery;

    constructor()
    {
        $(() => this.init());
    }

    private init(): void
    {
        $('.ui.accordion').accordion();
        $('.ui.dropdown').dropdown({ fullTextSearch: true });

        this.pk = new Packery('.preview-layout-grid', {
            itemSelector: '.preview-layout-item',
            columnWidth: '.preview-layout-item',
            rowHeight: '.preview-layout-item',
            gutter: window.ccOptions.tilePreviewPadding
        });
        this.pk.layout();

        if (typeof Draggabilly === 'function')
        {
            $('.preview-layout-item').each((_, e) =>
            {
                this.pk.bindDraggabillyEvents(new Draggabilly(e));
            });
        }
    }
}
var __app = new CommandCenter();