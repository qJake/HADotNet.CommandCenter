class CommandCenter
{
    constructor()
    {
        $(() => this.init());
    }

    private init(): void
    {
        $('.ui.accordion').accordion();
        $('.ui.sidebar').sidebar();
    }
} 
var __app = new CommandCenter();