class CommandCenter
{
    constructor()
    {
        $(() => this.init());
    }

    private init(): void
    {
        $('.ui.accordion').accordion();
        $('.ui.dropdown').dropdown({fullTextSearch: true});
    }
} 
var __app = new CommandCenter();