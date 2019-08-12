/**
 * Initializes a pre-existing layout from the element's current position.
 */
Packery.prototype.initShiftLayout = function (elements)
{
    this._resetLayout();

    // set item order and horizontal position from saved positions
    this.items = elements.map(function (e)
    {
        var item = this.getItem(e);

        let x = parseInt((e as HTMLElement).style.left.replace('px', ''));
        let y = parseInt((e as HTMLElement).style.top.replace('px', ''));
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